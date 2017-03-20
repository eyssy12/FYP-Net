(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .controller('AccountController', AccountController);

    AccountController.$inject = ['$scope', 'SidebarService', 'EnumerationsService', 'PersonService', 'AccountService']; 

    function AccountController($scope, SidebarService, EnumerationsService, PersonService, AccountService) {
        $scope.title = 'AccountController';

        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedAccount = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.tempAccount = {};
        $scope.sidebar = SidebarService;
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;

        PersonService.getAccountlessPersons()
            .then(function (response) {
                $scope.persons = response.data;
            }, function (error) {
            });

        EnumerationsService.getUserRoles()
            .then(function (response) {
                $scope.roles = response.data;
            }, function (error) {
            });

        $scope.getUserRoleAsString = function (id) {
            for (var i = 0; i < $scope.roles.length; i++) {
                var userRole = $scope.roles[i];

                if (id == userRole.Id) {
                    return userRole.Value;
                }
            }
        }

        $scope.prepareModal = function (action, account) {
            $scope.action = action;

            if (account != null) {
                $scope.selectedAccount = account;

                $scope.tempAccount.UserRole = account.UserRole;
                $scope.tempAccount.UserName = account.UserName;
                $scope.tempAccount.Email = account.Email;
                $scope.tempAccount.Person = account.Person;
                $scope.tempAccount.Password = account.Password;
            }
            else
            {
                $scope.tempAccount.UserRole = '';
                $scope.tempAccount.UserName = '';
                $scope.tempAccount.Password = '';
                $scope.tempAccount.Email = '';
                $scope.tempAccount.Person = {};
            }
        }

        $scope.onCreateModifyFormSubmit = function (actionForm) {

            if ($scope.action == 'Create') {
                $scope.onCreate($scope.tempAccount, actionForm);
            }
            else if ($scope.action == 'Modify') {
                $scope.onModify($scope.tempAccount, $scope.selectedAccount);
            }
        };

        $scope.resetForm = function (actionForm) {
            actionForm.autoValidateFormOptions.resetForm();
        };

        $scope.getUserPersonName = function (person) {
            var formatted = '';

            formatted = person.FirstName + " " + person.LastName;

            return formatted;
        };

        $scope.onCreate = function (account, actionForm) {
            $scope.submitting = true;
            $('#accountModal').modal('hide');

            $scope.tempAccount.PersonId = account.Person.Id;

            AccountService.post(account)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.tempAccount.UserRole = '';
                    $scope.tempAccount.accounts = [];
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Account created!';
                    $scope.lastResponse = response;
                    $scope.applicationUsers.push(response.data);

                    $scope.resetForm(actionForm);

                }, function (error) {
                    console.log(error);
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    $scope.resetForm(actionForm);

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Account with that name already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the account with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            // make get reuqest
            AccountService.get()
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.applicationUsers = response.data;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'accounts fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 401)
                    {
                        $scope.statusMessage.message = 'Insecure protocol is used - make use to access the site using https:// prefix';
                    }
                    else
                    {
                        $scope.statusMessage.message = 'There was a problem fecthing the accounts with a status code of ' + error.status;
                    }
                });
        };

        $scope.onDelete = function (account, index) {
            $scope.submitting = true;

            // make get reuqest
            AccountService.delete(account.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'account "' + account.Name + '" deleted.';
                    $scope.applicationUsers.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'account "' + account.Name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempAccount, selectedAccount) {
            $scope.submitting = true;
            $('#accountModal').modal('hide');

            tempAccount.Id = selectedAccount.Id;

            if (selectedAccount.Person == undefined || selectedAccount.Person == null)
            {
                tempAccount.PersonId = tempAccount.Person.Id;
            }
            else
            {
                tempAccount.PersonId = selectedAccount.Person.Id;
            }

            AccountService.put(tempAccount.Id, tempAccount)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'account with ID ' + selectedAccount.Id + ' modified!';

                    selectedAccount.UserName = tempAccount.UserName;
                    selectedAccount.Email = tempAccount.Email;
                    selectedAccount.Password = tempAccount.Password;
                    selectedAccount.UserRole = tempAccount.UserRole;
                    selectedAccount.Person = tempAccount.Person;
                    selectedAccount.PersonId = tempAccount.Person.Id;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'account with a name "' + tempAccount.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'account with ID ' + selectedAccount.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeAccounts = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.applicationUsers, function (account) {
                if (account.isSelected) {
                    idArrayViewModel.Ids.push(account.Id);
                }
            });

            AccountService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++) {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.applicationUsers.length; j++) {
                            var account = $scope.applicationUsers[j];

                            if (id == account.Id) {
                                $scope.applicationUsers.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'account(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'account(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.applicationUsers.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.applicationUsers, function (account) {
                    account.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.applicationUsers.length; i++) {
                if ($scope.applicationUsers[i].isSelected) {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.onGet();
    }
})();
