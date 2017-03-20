(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .controller('LecturerController', LecturerController);

    LecturerController.$inject = ['$scope', 'SidebarService', 'PersonService', 'LecturerService']; 

    function LecturerController($scope, SidebarService, PersonService, LecturerService) {
        $scope.title = 'LecturerController';

        $scope.sidebar = SidebarService;
        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedLecturer = {};
        $scope.tempLecturer = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;
        $scope.hasPersonSelected = true;
        $scope.courses = [];

        PersonService.getAvailablePersons()
            .then(function (response) {
                $scope.persons = response.data;
            });

        $scope.prepareModal = function (action, lecturer) {
            $scope.action = action;

            if (lecturer != null) {
                $scope.selectedLecturer = lecturer;

                $scope.tempLecturer.FirstName = lecturer.FirstName;
                $scope.tempLecturer.LastName = lecturer.LastName;

                $scope.tempLecturer.HireDate = lecturer.HireDate;
                $scope.tempLecturer.Person = lecturer.Person;
                $scope.tempLecturer.Modules = lecturer.Modules;
            }
            else {
                $scope.resetTempLecturer();
            }
        }

        $scope.onCreateModifyFormSubmit = function (actionForm) {

            if ($scope.action == 'Create') {
                $scope.onCreate($scope.tempLecturer, actionForm);
            }
            else if ($scope.action == 'Modify') {
                $scope.onModify($scope.tempLecturer, $scope.selectedLecturer);
            }
        };

        $scope.prepareHireDate = function (date) {

            var startTime = moment(date);

            var dateFormatted = startTime.format('LL');

            return dateFormatted;
        };

        $scope.prepareLecturerName = function (person) {
            var formatted = '';

            formatted = person.FirstName + " " + person.LastName;

            return formatted;
        };

        $scope.clearForm = function (actionForm) {

            actionForm.autoValidateFormOptions.resetForm();
            $scope.hasPersonSelected = true;

        };

        $scope.onCreate = function (lecturer, actionForm) {

            if (lecturer == undefined || lecturer.Person == undefined || lecturer.Person == null || jQuery.isEmptyObject(lecturer.Person))
            {
                $scope.hasPersonSelected = false;
                return;
            }
            else
            {
                $scope.hasPersonSelected = true;
            }

            $scope.submitting = true;
            $('#lecturerModal').modal('hide');


            lecturer.HireDate = new Date(lecturer.HireDateString);
            lecturer.PersonId = lecturer.Person.Id;

            LecturerService.post(lecturer)
                .then(function (response) {
                    $scope.submitting = false;

                    $scope.resetTempLecturer();

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Lecturer created!';
                    $scope.lastResponse = response;

                    $scope.lecturers.push(response.data);

                    actionForm.autoValidateFormOptions.resetForm();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Could not create a lecturer due';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the lecturer with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            LecturerService.get()
                .then(function (response) {
                    $scope.submitting = false;

                    var temps = response.data;

                    angular.forEach(temps, function (temp) {
                        temp.HireDateString = moment(temp.HireDate).format('LL');
                    });

                    $scope.lecturers = temps;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Lecturers fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'There was a problem fecthing the lecturer with a status code of ' + error.status;
                });
        };

        $scope.onDelete = function (lecturer, index) {
            $scope.submitting = true;

            var name = lecturer.LecturerPerson.Person.FirstName + " " + lecturer.LecturerPerson.Person.LastName;

            // make get reuqest
            LecturerService.delete(lecturer.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Lecturer "' + name + '" deleted.';
                    $scope.lecturers.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Lecturer "' + name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempLecturer, selectedLecturer) {
            $scope.submitting = true;
            $('#lecturerModal').modal('hide');

            tempLecturer.Id = selectedLecturer.Id;
            tempLecturer.PersonId = selectedLecturer.Person.Id;

            LecturerService.put(tempLecturer.Id, tempLecturer)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Lecturer with ID ' + selectedLecturer.Id + ' modified!';

                    var lecturerResponse = response.data;

                    selectedLecturer.HireDate = lecturerResponse.HireDate;
                    selectedLecturer.HireDateString = lecturerResponse.HireDateString;
                    selectedLecturer.Person = lecturerResponse.Person;

                    $scope.resetTempLecturer();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'Lecturer with a name "' + tempLecturer.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'Lecturer with ID ' + selectedLecturer.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeLecturers = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.lecturers, function (lecturer) {
                if (lecturer.isSelected) {
                    idArrayViewModel.Ids.push(lecturer.Id);
                }
            });

            LecturerService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++) {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.lecturers.length; j++) {
                            var Class = $scope.lecturers[j];

                            if (id == Class.Id) {
                                $scope.lecturers.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Lecturer(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Lecturer(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.lecturers.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.lecturers, function (lecturer) {
                    lecturer.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.lecturers.length; i++) {
                if ($scope.lecturers[i].isSelected) {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.resetTempLecturer = function () {
            $scope.tempLecturer.HireDate = {};
            $scope.tempLecturer.HireDateString = '';
            $scope.tempLecturer.Person = {};
            $scope.tempLecturer.Modules = [];
        }

        $scope.onGet();
    }
})();
