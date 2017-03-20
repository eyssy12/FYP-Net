(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .controller('ModuleController', ModuleController);

    ModuleController.$inject = ['$scope', '$filter', 'SidebarService', 'EnumerationsService', 'SemesterService', 'LecturerService', 'ModuleService'];

    function ModuleController($scope, $filter, SidebarService, EnumerationsService, SemesterService, LecturerService, ModuleService) {

        $scope.title = 'ModuleController';

        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedModule = {};
        $scope.tempModule = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;
        $scope.courses = [];
        $scope.sidebar = SidebarService;
        $scope.lecturersMultiSelect = [];
        $scope.lecturersSelected = [];
        $scope.lecturersCopy = [];

        EnumerationsService.getModuleTypes()
            .then(function (response) {
                $scope.moduleTypes = response.data;
            }, function (error) {

            });

        SemesterService.get()
            .then(function (response)
            {
                $scope.semesters = response.data;

                var filteredSemesters = $filter('unique')($scope.semesters, 'Course.Name');

                angular.forEach(filteredSemesters, function (filtered) {
                    $scope.courses.push(filtered.Course);
                });

                $scope.courses;

            }, function (error)
            {

            });

        LecturerService.get()
            .then(function (response){
                $scope.lecturersCopy = response.data;

                angular.forEach($scope.lecturersCopy, function (lecturer) {

                    $scope.lecturersMultiSelect.push({
                        name: lecturer.LecturerPerson.Person.FirstName + " " + lecturer.LecturerPerson.Person.LastName,
                        ticked: false,
                        Id: lecturer.Id
                    });

                });
            });

        $scope.prepareModal = function (action, module) {
            $scope.action = action;

            // default the ticked array in the multi select
            angular.forEach($scope.lecturersMultiSelect, function (lecturer) {
                lecturer.ticked = false;
            });

            if (module != null) {
                $scope.selectedCourseForModule = module.Semester.Course;
                $scope.selectedModule = module;

                $scope.tempModule.Name = module.Name;
                $scope.tempModule.Credits = module.Credits;
                $scope.tempModule.ModuleType = module.ModuleType;
                $scope.tempModule.Semester = module.Semester;
                $scope.courseSemesters = $scope.prepareSemestersForModule();

                for (var i = 0; i < $scope.selectedModule.LecturerModules.length; i++) {
                    var lecturerModule = $scope.selectedModule.LecturerModules[i];

                    for (var j = 0; j < $scope.lecturersMultiSelect.length; j++)
                    {

                        var multiSelectLecturer = $scope.lecturersMultiSelect[j];

                        if (lecturerModule.Lecturer.Id == multiSelectLecturer.Id)
                        {
                            multiSelectLecturer.ticked = true;
                            break;
                        }

                    }
                }

            }
            else {
                $scope.resetTempModule();
            }
        }

        $scope.changeTempModule = function () {
            $scope.tempModule.Semester
        };

        $scope.onCreateModifyFormSubmit = function (actionForm) {

            if ($scope.action == 'Create')
            {
                $scope.onCreate($scope.tempModule, actionForm);
            }
            else if ($scope.action == 'Modify')
            {
                $scope.onModify($scope.tempModule, $scope.selectedModule);
            }
        };

        $scope.getModuleTypeAsString = function (id)
        {
            for (var i = 0; i < $scope.moduleTypes.length; i++)
            {
                var moduleType = $scope.moduleTypes[i];

                if (id == moduleType.Id)
                {
                    return moduleType.Value;
                }
            }
        };

        $scope.clearSelectedCourse = function (actionForm) {
            $scope.selectedCourseForModule = undefined;

            actionForm.autoValidateFormOptions.resetForm(); // reset the state of the form
        };

        $scope.prepareSemestersForModule = function () {
            
            // var whereClause = "{ Course.Name: '" + $scope.selectedCourseForModule.Name + "' }";

            var temp = [];

            angular.forEach($scope.semesters, function (semester) {
                if (semester.Course.Name == $scope.selectedCourseForModule.Name)
                {
                    temp.push(semester);
                }
            });

            $scope.courseSemesters = temp;

        };

        $scope.onCreate = function (module, actionForm) {
            $scope.submitting = true;
            $('#moduleModal').modal('hide');

            if (module.Semester.Id == undefined || module.Semester.Id == null)
            {
                module.SemesterId = module.Semester;
            }

            module.SemesterId = module.Semester.Id;
            module.LecturerIds = [];
            angular.forEach($scope.lecturersSelected, function (selectedLecturer) {
                module.LecturerIds.push(selectedLecturer.Id);
            });

            ModuleService.post(module)
                .then(function (response) {
                    $scope.submitting = false;

                    $scope.resetTempModule();

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Module created!';
                    $scope.lastResponse = response;
                    $scope.modules.push(response.data);

                    $scope.selectedCourseForModule = undefined; 
                    actionForm.autoValidateFormOptions.resetForm(); // reset the state of the form

                }, function (error) {
                    console.log(error);
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Could not create a Module due';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the Module with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            ModuleService.get()
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.modules = response.data;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Modules fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'There was a problem fecthing the Modules with a status code of ' + error.status;
                });
        };

        $scope.onDelete = function (module, index) {
            $scope.submitting = true;

            // make get reuqest
            ModuleService.delete(module.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Module "' + module.Name + '" deleted.';
                    $scope.modules.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Module "' + module.Name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempModule, selectedModule) {
            $scope.submitting = true;
            $('#moduleModal').modal('hide');

            tempModule.Id = selectedModule.Id;
            tempModule.SemesterId = selectedModule.Semester.Id;
            tempModule.CourseId = $scope.selectedCourseForModule.Id;

            tempModule.LecturerIds = [];
            angular.forEach($scope.lecturersSelected, function (selectedLecturer) {
                tempModule.LecturerIds.push(selectedLecturer.Id);
            });

            ModuleService.put(tempModule.Id, tempModule)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Module with ID ' + selectedModule.Id + ' modified!';

                    var moduleResponse = response.data;

                    selectedModule.Name = moduleResponse.Name;
                    selectedModule.Credits = moduleResponse.Credits;
                    selectedModule.ModuleType = moduleResponse.ModuleType;
                    selectedModule.Semester = moduleResponse.Semester;
                    selectedModule.LecturerModules = moduleResponse.LecturerModules;
                    selectedModule.StudentModules = moduleResponse.StudentModules;

                    $scope.resetTempModule();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'Module with a name "' + tempModule.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'Module with ID ' + selectedModule.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeModules = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.modules, function (module) {
                if (module.isSelected) {
                    idArrayViewModel.Ids.push(module.Id);
                }
            });

            ModuleService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++) {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.modules.length; j++) {
                            var module = $scope.modules[j];

                            if (id == module.Id) {
                                $scope.modules.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Module(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Module(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.modules.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.modules, function (semester) {
                    semester.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.modules.length; i++) {
                if ($scope.modules[i].isSelected) {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.resetTempModule = function () {
            $scope.tempModule.Name = '';
            $scope.tempModule.Credits = '';
            $scope.tempModule.ModuleType = '';
            $scope.tempModule.Semester = '';
            $scope.tempModule.SemesterId = '';
            $scope.tempModule.LecturerModules = [];
            $scope.tempModule.StudentModules = [];
        }

        $scope.onGet();

    }
})();