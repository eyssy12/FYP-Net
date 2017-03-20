(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .controller('CourseController', CourseController);

    CourseController.$inject = ['$scope', 'SidebarService', 'EnumerationsService', 'DepartmentService', 'CourseService'];

    function CourseController($scope, SidebarService, EnumerationsService, DepartmentService, CourseService) {
        $scope.title = 'CourseController';


        EnumerationsService.getAwardTypes()
            .then(function (response)
            {
                $scope.degreeTypes = response.data
            },
            function (error)
            {
            });

        DepartmentService.getWithoutComplexTypes()
            .then(function (response)
            {
                $scope.departments = response.data
            },
            function (error)
            {
            });

        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedCourse = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.tempCourse = {};
        $scope.sidebar = SidebarService;
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;

        $scope.getDegreeTypeAsString = function (id) 
        {
            for (var i = 0; i < $scope.degreeTypes.length; i++)
            {
                var degreeType = $scope.degreeTypes[i];

                if (id == degreeType.Id)
                {
                    return degreeType.Value;
                }
            }
        }

        $scope.prepareModal = function (action, course) {
            $scope.action = action;

            if (course != null) {
                $scope.selectedCourse = course;

                $scope.tempCourse.Name = course.Name;
                $scope.tempCourse.Code = course.Code;
                $scope.tempCourse.AwardType = course.AwardType;
                $scope.tempCourse.Department = course.Department;
                $scope.tempCourse.Semesters = course.Semesters;
            }
            else
            {
                $scope.tempCourse.Name = '';
                $scope.tempCourse.Code = '';
                $scope.tempCourse.AwardType = '';
                $scope.tempCourse.Department = {};
                $scope.tempCourse.DepartmentId = '';
                $scope.tempCourse.Semesters = [];
            }
        }

        $scope.onCreate = function (course) {
            $scope.submitting = true;
            $('#courseModal').modal('hide');

            $scope.tempCourse.DepartmentId = course.Department.Id;

            CourseService.post(course)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Course "' + course.Name + '" created!';
                    $scope.lastResponse = response;
                    $scope.courses.push(response.data);
                }, function (error) {
                    console.log(error);
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Course with that name already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the course with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            // make get reuqest
            CourseService.get()
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.courses = response.data;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'courses fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;


                    if (error.status == 401) {
                        $scope.statusMessage.message = 'Insecure protocol is used - make use to access the site using https:// prefix';
                    }
                    else {
                        $scope.statusMessage.message = 'There was a problem fecthing the courses with a status code of ' + error.status;
                    }
                });
        };

        $scope.onDelete = function (course, index) {
            $scope.submitting = true;

            // make get reuqest
            CourseService.delete(course.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Course "' + course.Name + '" deleted.';
                    $scope.courses.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Course "' + course.Name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempCourse, selectedCourse) {
            $scope.submitting = true;
            $('#courseModal').modal('hide');

            tempCourse.Id = selectedCourse.Id;
            tempCourse.DepartmentId = tempCourse.Department.Id;

            CourseService.put(tempCourse.Id, tempCourse)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Course with ID ' + selectedCourse.Id + ' modified!';

                    selectedCourse.Name = tempCourse.Name;
                    selectedCourse.Code = tempCourse.Code;
                    selectedCourse.Courses = tempCourse.Courses;
                    selectedCourse.AwardType = tempCourse.AwardType;
                    selectedCourse.Department = tempCourse.Department;
                    selectedCourse.DepartmentId = tempCourse.Department.Id;

                    $scope.tempCourse.Name = '';
                    $scope.tempCourse.Courses = [];

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'Course with a name "' + tempCourse.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'Course with ID ' + selectedCourse.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeCourses = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.courses, function (course) {
                if (course.isSelected) {
                    idArrayViewModel.Ids.push(course.Id);
                }
            });

            CourseService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++) {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.courses.length; j++) {
                            var course = $scope.courses[j];

                            if (id == course.Id) {
                                $scope.courses.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Course(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'Course(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.courses.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.courses, function (course) {
                    course.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.courses.length; i++) {
                if ($scope.courses[i].isSelected) {
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
