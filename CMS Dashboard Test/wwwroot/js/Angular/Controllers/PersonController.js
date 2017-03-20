(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .constant('moment', moment)
        .controller('PersonController', PersonController);

    PersonController.$inject = ['$scope', 'moment', 'SidebarService', 'PersonService']; 

    function PersonController($scope, moment, SidebarService, PersonService) {
        $scope.title = 'PersonController';

        $scope.sidebar = SidebarService;
        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedPerson = {};
        $scope.tempPerson = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;
        $scope.courses = [];

        $scope.prepareModal = function (action, person) {
            $scope.action = action;

            if (person != null)
            {
                $scope.selectedPerson = person;

                $scope.tempPerson.FirstName = person.FirstName;
                $scope.tempPerson.LastName = person.LastName;
                $scope.tempPerson.BirthDate = person.BirthDate;
                $scope.tempPerson.HomePhone = person.HomePhone;
                $scope.tempPerson.MoilePhone = person.MoilePhone;
                $scope.tempPerson.Address = person.Address;
            }
            else
            {
                $scope.resetTempPerson();
            }
        }

        $scope.onCreateModifyFormSubmit = function (actionForm) {

            if ($scope.action == 'Create') {
                $scope.onCreate($scope.tempPerson, actionForm);
            }
            else if ($scope.action == 'Modify') {
                $scope.onModify($scope.tempPerson, $scope.selectedPerson);
            }
        };

        $scope.prepareDateTime = function (date) {

            var startTime = moment(date);

            var startDateFormatted = startTime.format('LL');

            return startDateFormatted;
        };

        $scope.onCreate = function (person, actionForm) {
            $scope.submitting = true;
            $('#personModal').modal('hide');

            person.BirthDate = new Date(person.BirthDateString);

            PersonService.post(person)
                .then(function (response) {
                    $scope.submitting = false;

                    $scope.resetTempPerson();

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Person created!';
                    $scope.lastResponse = response;
                    $scope.persons.push(response.data);

                    actionForm.autoValidateFormOptions.resetForm();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Could not create a Person due';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the Person with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            PersonService.get()
                .then(function (response) {
                    $scope.submitting = false;

                    var temps = response.data;

                    angular.forEach(temps, function (temp) {
                        temp.BirthDateString = moment(temp.BirthDate).format('LL');
                    });

                    $scope.persons = temps;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Persons fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'There was a problem fecthing the Person with a status code of ' + error.status;
                });
        };

        $scope.onDelete = function (person, index) {
            $scope.submitting = true;

            var name = person.Name;

            // make get reuqest
            PersonService.delete(person.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Person "' + name + '" deleted.';
                    $scope.persons.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Person "' + name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempPerson, selectedPerson) {
            $scope.submitting = true;
            $('#personModal').modal('hide');

            tempPerson.Id = selectedPerson.Id;

            PersonService.put(tempPerson.Id, tempPerson)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Person with ID ' + selectedPerson.Id + ' modified!';

                    var personResponse = response.data;

                    selectedPerson.FirstName = personResponse.FirstName;
                    selectedPerson.LastName = personResponse.LastName;
                    selectedPerson.BirthDate = personResponse.BirthDate;
                    selectedPerson.MoilePhone = personResponse.MoilePhone;
                    selectedPerson.Address1 = personResponse.Address1;
                    selectedPerson.Address2 = personResponse.Address2;
                    selectedPerson = personResponse.Address3;
                    selectedPerson = personResponse.Address4;

                    $scope.resetTempPerson();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'Person with a name "' + tempPerson.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'Person with ID ' + selectedPerson.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removePersons = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.persons, function (person) {
                if (person.isSelected) {
                    idArrayViewModel.Ids.push(person.Id);
                }
            });

            PersonService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++) {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.persons.length; j++) {
                            var Class = $scope.persons[j];

                            if (id == Class.Id) {
                                $scope.persons.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Person(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Person(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.persons.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.persons, function (person) {
                    person.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.persons.length; i++) {
                if ($scope.persons[i].isSelected) {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.resetTempPerson = function () {
            $scope.tempPerson.FirstName = '';
            $scope.tempPerson.LastName = '';
            $scope.tempPerson.BirthDate = {};
            $scope.tempPerson.BirthDateString = '';
            $scope.tempPerson.MoilePhone = '';
            $scope.tempPerson.Address = '';
        }

        $scope.onGet();
    }
})();