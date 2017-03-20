(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .controller('SemesterController', SemesterController);

    SemesterController.$inject = ['$scope', 'SidebarService', 'SemesterService', 'CourseService']; 

    function SemesterController($scope, SidebarService, SemesterService, CourseService)
    {
        $scope.title = 'SemesterController';
        $scope.sidebar = SidebarService;

        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedSemester = {};
        $scope.tempSemester = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.selectAll = false;       
        $scope.selectedForDeletion = false;

        CourseService.get()
            .then(function (response)
            {
                $scope.courses = response.data;
            }, function (error)
            {

            });

        $scope.prepareModal = function (action, semester) {
            $scope.action = action;

            if (semester != null) {
                $scope.selectedSemester = semester;

                $scope.tempSemester.Number = semester.Number;
                $scope.tempSemester.Course = semester.Course;
            }
            else
            {
                $scope.resetTempSemester();
            }
        }

        $scope.onCreate = function (semester) {
            $scope.submitting = true;
            $('#semesterModal').modal('hide');

            semester.CourseId = semester.Course.Id;

            SemesterService.post(semester)
                .then(function (response) {
                    $scope.submitting = false;

                    $scope.resetTempSemester();

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Semester created!';
                    $scope.lastResponse = response;
                    $scope.semesters.push(response.data);
                }, function (error) {
                    console.log(error);
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Could not create a Semester due';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the Semester with a status code of ' + error.status;
                    }
                });
        };

        $scope.changeTempSemester = function () {
            $scope.selectedSemester;
            $scope.tempSemester;
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            SemesterService.get()
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.semesters = response.data;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Semesters fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'There was a problem fecthing the Semesters with a status code of ' + error.status;
                });
        };

        $scope.onDelete = function (semester, index) {
            $scope.submitting = true;

            // make get reuqest
            SemesterService.delete(semester.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Semester "' + semester.Name + '" deleted.';
                    $scope.semesters.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Semester "' + semester.Name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempSemester, selectedSemester) {
            $scope.submitting = true;
            $('#semesterModal').modal('hide');

            tempSemester.Id = selectedSemester.Id;

            if (selectedSemester.Course == undefined)
            {
                tempSemester.CourseId = tempSemester.Course.Id;
            }
            else {
                tempSemester.CourseId = selectedSemester.Course.Id;
            }
            

            SemesterService.put(tempSemester.Id, tempSemester)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Semester with ID ' + selectedSemester.Id + ' modified!';

                    var semesterResponse = response.data;

                    selectedSemester.Number = semesterResponse.Number;
                    selectedSemester.Credits = semesterResponse.Credits;
                    selectedSemester.Course = semesterResponse.Course;
                    selectedSemester.Classes = semesterResponse.Classes;
                    selectedSemester.Modules = semesterResponse.Modules;

                    $scope.resetTempSemester();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'Semester with a name "' + tempSemester.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'Semester with ID ' + selectedSemester.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeSemesters = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.semesters, function (semester) {
                if (semester.isSelected) {
                    idArrayViewModel.Ids.push(semester.Id);
                }
            });

            SemesterService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++)
                    {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.semesters.length; j++)
                        {
                            var Semester = $scope.semesters[j];

                            if (id == Semester.Id)
                            {
                                $scope.semesters.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Semester(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Semester(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.semesters.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.semesters, function (semester) {
                    semester.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.semesters.length; i++) {
                if ($scope.semesters[i].isSelected) {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.resetTempSemester = function ()
        {
            $scope.tempSemester.Number = '';
            $scope.tempSemester.Course = {};
            $scope.tempSemester.CourseId = '';
            $scope.tempSemester.Classes = [];
            $scope.tempSemester.Modules = [];
        }

        $scope.onGet();
    }
})();
