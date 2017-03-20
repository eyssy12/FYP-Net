(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .constant('moment', moment)
        .controller('ClassController', ClassController);

    ClassController.$inject = ['$scope', '$filter', 'moment', 'SidebarService', 'EnumerationsService', 'SemesterService', 'ClassService']; 

    function ClassController($scope, $filter, moment, SidebarService, EnumerationsService, SemesterService, ClassService)
    {
        $scope.title = 'ClassController';

        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedClass = {};
        $scope.tempClass = {};
        $scope.lastResponse = {};
        $scope.sidebar = SidebarService;
        $scope.action = '';
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;
        $scope.classes = [];
        $scope.courses = [];

        EnumerationsService.getEnrollmentStages()
            .then(function (response) {
                $scope.enrollmentStages = response.data;
            }, function (error) {
            });

        SemesterService.get()
            .then(function (response) {
                $scope.semesters = response.data;

                var filteredSemesters = $filter('unique')($scope.semesters, 'Course.Name');

                angular.forEach(filteredSemesters, function (filtered) {
                    $scope.courses.push(filtered.Course);
                });

                $scope.courses;

            }, function (error) {

            });

        $scope.prepareModal = function (action, classObj) {
            $scope.action = action;

            if (classObj != null) {
                $scope.selectedCourseForClass = classObj.Semester.Course;
                $scope.selectedClass = classObj;

                $scope.tempClass.Name = classObj.Name;
                $scope.tempClass.YearCommenced = classObj.YearCommenced;
                $scope.tempClass.EnrollmentStage = classObj.EnrollmentStage;
                $scope.tempClass.Timetable = classObj.Timetable;
                $scope.tempClass.Semester = classObj.Semester;
                $scope.courseSemesters = $scope.prepareSemestersForClass();
            }
            else {
                $scope.resetTempClass();
            }
        }

        $scope.onCreateModifyFormSubmit = function (actionForm) {

            if ($scope.action == 'Create') {
                $scope.onCreate($scope.tempClass, actionForm);
            }
            else if ($scope.action == 'Modify') {
                $scope.onModify($scope.tempClass, $scope.selectedClass);
            }
        };

        $scope.changeTempClass = function (semester) {
            semester;
            $scope.tempClass.Semester;
        };

        $scope.prepareSemestersForClass = function () {

            var temp = [];

            angular.forEach($scope.semesters, function (semester) {
                if (semester.Course.Name == $scope.selectedCourseForClass.Name) {
                    temp.push(semester);
                }
            });

            $scope.courseSemesters = temp;

        };

        $scope.prepareDateTime = function (date) {

            var startTime = moment(date);

            var startDateFormatted = startTime.format('LL');

            return startDateFormatted;
        };

        $scope.getEnrollmentStageAsString = function (enumId) {
            for (var i = 0; i < $scope.enrollmentStages.length; i++) {
                var enrollmentStage = $scope.enrollmentStages[i];

                if (enumId == enrollmentStage.Id) {
                    return enrollmentStage.Value;
                }
            }
        };

        $scope.onCreate = function (classObj) {
            $scope.submitting = true;
            $('#classModal').modal('hide');

            var time = new Date(classObj.YearCommencedString);

            classObj.SemesterId = classObj.Semester.Id;
            classObj.YearCommenced = time;
            classObj.TimetableId = 0;

            ClassService.post(classObj)
                .then(function (response) {
                    $scope.submitting = false;

                    $scope.resetTempClass();

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Class created!';
                    $scope.lastResponse = response;
                    $scope.classes.push(response.data);

                    actionForm.autoValidateFormOptions.resetForm();
                }, function (error) {
                    console.log(error);
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Could not create a Class due';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the Class with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            ClassService.get()
                .then(function (response) {
                    $scope.submitting = false;

                    var temps = response.data;

                    angular.forEach(temps, function (temp) {
                        temp.YearCommencedString = moment(temp.YearCommenced).format('LL');
                    });

                    $scope.classes = temps;

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Classs fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 401) {
                        $scope.statusMessage.message = 'Insecure protocol is used - make use to access the site using https:// prefix';
                    }
                    else {
                        $scope.statusMessage.message = 'There was a problem fecthing the Class with a status code of ' + error.status;
                    }
                });
        };

        $scope.onDelete = function (classObj, index) {
            $scope.submitting = true;

            // make get reuqest
            ClassService.delete(classObj.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Class "' + classObj.Name + '" deleted.';
                    $scope.classes.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Class "' + classObj.Name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempClass, selectedClass) {
            $scope.submitting = true;
            $('#classModal').modal('hide');

            tempClass.Id = selectedClass.Id;
            tempClass.SemesterId = selectedClass.Semester.Id;
            tempClass.CourseId = $scope.selectedCourseForClass.Id;

            ClassService.put(tempClass.Id, tempClass)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Class with ID ' + selectedClass.Id + ' modified!';

                    var classObjResponse = response.data;

                    selectedClass.Name = classObjResponse.Name;
                    selectedClass.YearCommenced = classObjResponse.YearCommenced;
                    selectedClass.EnrollmentStage = classObjResponse.EnrollmentStage;
                    selectedClass.Timetable = classObjResponse.Timetable;
                    selectedClass.Semester = classObjResponse.Semester;

                    $scope.resetTempClass();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'Class with a name "' + tempClass.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'Class with ID ' + selectedClass.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeClasss = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.classes, function (classObj) {
                if (classObj.isSelected) {
                    idArrayViewModel.Ids.push(classObj.Id);
                }
            });

            ClassService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++) {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.classes.length; j++) {
                            var Class = $scope.classes[j];

                            if (id == Class.Id) {
                                $scope.classes.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Class(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Class(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.classes.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.classes, function (classObj) {
                    classObj.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.classes.length; i++) {
                if ($scope.classes[i].isSelected) {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.resetTempClass = function () {
            $scope.tempClass.Name = '';
            $scope.tempClass.YearCommenced = {};
            $scope.tempClass.YearCommencedString = '';
            $scope.tempClass.EnrollmentStage = '';
            $scope.tempClass.Timetable = {};
            $scope.tempClass.Semester = {};
        }

        $scope.clearForm = function (actionForm) {
            actionForm.autoValidateFormOptions.resetForm();
        };

        $scope.onGet();
    }
})();

