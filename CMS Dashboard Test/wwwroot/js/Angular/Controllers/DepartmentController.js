(function ()
{
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .controller('DepartmentController', DepartmentController);

    DepartmentController.$inject = ['$scope', 'SidebarService', 'DepartmentService']; 

    function DepartmentController($scope, SidebarService, DepartmentService)
    {
        $scope.title = 'DepartmentController';
        $scope.sidebar = SidebarService;

        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedDepartment = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.tempDepartment = {};
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;

        $scope.prepareModal = function (action, department) {
            $scope.action = action;

            if (department != null)
            {
                $scope.selectedDepartment = department;

                $scope.tempDepartment.Name = department.Name;
            }
            else
            {
                $scope.tempDepartment.Name = '';
            }
        }


        $scope.onCreateModifyFormSubmit = function () {

            if ($scope.action == 'Create') {
                $scope.onCreate($scope.tempDepartment);
            }
            else if ($scope.action == 'Modify') {
                $scope.onModify($scope.tempDepartment, $scope.selectedDepartment);
            }
        };

        $scope.onCreate = function (department) {
            $scope.submitting = true;
            $('#departmentModal').modal('hide');

            DepartmentService.post(department)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.tempDepartment.Name = '';
                    $scope.tempDepartment.Courses = [];
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Department created!';
                    $scope.lastResponse = response;
                    $scope.departments.push(response.data);
                }, function (error) {
                    console.log(error);
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Department with that name already exists.';
                    }
                    else if (error.status == 401)
                    {
                        $scope.statusMessage.message = 'Insecure protocol is used - make use to access the site using https:// prefix';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the department with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            DepartmentService.get()
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.departments = response.data;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Departments fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'There was a problem fecthing the Departments with a status code of ' + error.status;
                });
        };

        $scope.onDelete = function (department, index) {
            $scope.submitting = true;

            // make get reuqest
            DepartmentService.delete(department.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Department "' + department.Name + '" deleted.';
                    $scope.departments.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Department "' + department.Name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempDepartment, selectedDepartment) {
            $scope.submitting = true;
            $('#departmentModal').modal('hide');

            tempDepartment.Id = selectedDepartment.Id;

            DepartmentService.put(tempDepartment.Id, tempDepartment)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Department with ID ' + selectedDepartment.Id + ' modified!';

                    selectedDepartment.Name = tempDepartment.Name;

                    $scope.tempDepartment.Name = '';

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'Department with a name "' + tempDepartment.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'Department with ID ' + selectedDepartment.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeDepartments = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.departments, function (department) {
                if (department.isSelected)
                {
                    idArrayViewModel.Ids.push(department.Id);
                }
            });

            DepartmentService.post(idArrayViewModel)
                .then(function (response) 
                {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++)
                    {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.departments.length; j++)
                        {
                            var department = $scope.departments[j];

                            if (id == department.Id)
                            {
                                $scope.departments.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Department(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error)
                {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Department(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function ()
        {
            if ($scope.departments.length != 0)
            {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.departments, function (department) {
                    department.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function ()
        {
            for (var i = 0; i < $scope.departments.length; i++)
            {
                if ($scope.departments[i].isSelected)
                {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else
                {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.onGet();
    }
})();