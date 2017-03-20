(function () {
    'use strict';

    var actionSuccess = 'Success',
        actionError = 'Error';

    angular
        .module(angularAppName)
        .controller('StudentController', StudentController);

    StudentController.$inject = ['$scope', 'SidebarService', 'EnumerationsService', 'PersonService', 'ClassService', 'StudentService'];

    function StudentController($scope, SidebarService, EnumerationsService, PersonService, ClassService, StudentService) {

        $scope.title = 'StudentController';

        $scope.sidebar = SidebarService;
        $scope.statusMessage = {};
        $scope.submitting = false;
        $scope.selectedStudent = {};
        $scope.tempStudent = {};
        $scope.lastResponse = {};
        $scope.action = '';
        $scope.selectAll = false;
        $scope.selectedForDeletion = false;
        $scope.courses = [];

        EnumerationsService.getEnrollmentStages()
            .then(function (response) {
                $scope.enrollmentStages = response.data;
            });

        PersonService.getAvailablePersons()
            .then(function (response) {
                $scope.persons = response.data;
            });

        ClassService.get()
            .then(function (response) {
                $scope.classes = response.data;
            });

        $scope.prepareModal = function (action, student) {
            $scope.action = action;

            if (student != null) {
                $scope.selectedStudent = student;

                $scope.tempStudent.FirstName = student.StudentPerson.Person.FirstName;
                $scope.tempStudent.LastName = student.StudentPerson.Person.LastName;

                $scope.tempStudent.EnrollmentDate = student.EnrollmentDate;
                $scope.tempStudent.Person = student.StudentPerson.Person;
                $scope.tempStudent.Class = student.Class;
            }
            else {
                $scope.resetTempStudent();
            }
        }

        $scope.onCreateModifyFormSubmit = function (actionForm) {

            if ($scope.action == 'Create') {
                $scope.onCreate($scope.tempStudent, actionForm);
            }
            else if ($scope.action == 'Modify') {
                $scope.onModify($scope.tempStudent, $scope.selectedStudent);
            }
        };

        $scope.getEnrollmentStageAsString = function (id) {
            for (var i = 0; i < $scope.enrollmentStages.length; i++) {
                var enrollmentStage = $scope.enrollmentStages[i];

                if (id == enrollmentStage.Id) {
                    return enrollmentStage.Value;
                }
            }
        };

        $scope.prepareEnrollmentDate = function (date) {

            var startTime = moment(date);

            var dateFormatted = startTime.format('LL');

            return dateFormatted;
        };

        $scope.prepareStudentName = function (person) {
            var formatted = '';

            formatted = person.FirstName + " " + person.LastName;

            return formatted;
        };

        $scope.onCreate = function (student, actionForm) {
            $scope.submitting = true;
            $('#studentModal').modal('hide');

            student.EnrollmentDate = new Date(student.EnrollmentDateString);
            student.ClassId = student.Class.Id;
            student.PersonId = student.Person.Id;

            StudentService.post(student)
                .then(function (response) {
                    $scope.submitting = false;

                    $scope.resetTempStudent();

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'Student created!';
                    $scope.lastResponse = response;

                    $scope.students.push(response.data);

                    actionForm.autoValidateFormOptions.resetForm();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.lastResponse = error;
                    $scope.statusMessage.type = actionError;

                    if (error.status === 409) {
                        $scope.statusMessage.message = 'Could not create a student due';
                    }
                    else {
                        $scope.statusMessage.message = 'There was an error creating the student with a status code of ' + error.status;
                    }
                });
        };

        $scope.onGet = function () {
            $scope.submitting = true;

            StudentService.get()
                .then(function (response) {
                    $scope.submitting = false;

                    var temps = response.data;

                    angular.forEach(temps, function (temp) {
                        temp.EnrollmentDateString = moment(temp.EnrollmentDate).format('LL');
                    });

                    $scope.students = temps;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'students fetched';
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'There was a problem fecthing the student with a status code of ' + error.status;
                });
        };

        $scope.onDelete = function (student, index) {
            $scope.submitting = true;

            // make get reuqest
            StudentService.delete(student.Id)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'student "' + student.Name + '" deleted.';
                    $scope.students.splice(index, 1);
                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'student "' + student.Name + '" could not be deleted.';
                });
        };

        $scope.onModify = function (tempStudent, selectedStudent) {
            $scope.submitting = true;
            $('#studentModal').modal('hide');

            tempStudent.Id = selectedStudent.Id;
            tempStudent.ClassId = tempStudent.Class.Id;
            tempStudent.PersonId = selectedStudent.StudentPerson.Person.Id;

            StudentService.put(tempStudent.Id, tempStudent)
                .then(function (response) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'student with ID ' + selectedStudent.Id + ' modified!';

                    var studentResponse = response.data;

                    selectedStudent.EnrollmentDate = studentResponse.EnrollmentDate;
                    selectedStudent.EnrollmentDateString = studentResponse.EnrollmentDateString;
                    selectedStudent.StudentPerson = selectedStudent.StudentPerson;
                    selectedStudent.Class = studentResponse.Class;

                    $scope.resetTempStudent();

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;

                    if (error.status == 409) {
                        $scope.statusMessage.message = 'student with a name "' + tempStudent.Name + '" already exists.';
                    }
                    else {
                        $scope.statusMessage.message = 'student with ID ' + selectedStudent.Id + ' could not be modified with error code ' + error.status;
                    }
                });
        };

        $scope.removeStudents = function () {
            $scope.submitting = true;

            var idArrayViewModel = {};
            idArrayViewModel.Ids = [];

            angular.forEach($scope.students, function (student) {
                if (student.isSelected) {
                    idArrayViewModel.Ids.push(student.Id);
                }
            });

            StudentService.bulkDelete(idArrayViewModel)
                .then(function (response) {
                    $scope.submitting = false;

                    for (var i = 0; i < idArrayViewModel.Ids.length; i++) {
                        var id = idArrayViewModel.Ids[i];

                        for (var j = 0; j < $scope.students.length; j++) {
                            var Class = $scope.students[j];

                            if (id == Class.Id) {
                                $scope.students.splice(j, 1);
                                break;
                            }
                        }
                    }

                    $scope.statusMessage.type = actionSuccess;
                    $scope.statusMessage.message = 'student(s) deleted successfully!';

                    $scope.selectAll = false;
                    $scope.selectedForDeletion = false;

                }, function (error) {
                    $scope.submitting = false;
                    $scope.statusMessage.type = actionError;
                    $scope.statusMessage.message = 'student(s) could not be deleted due to status code - ' + error.status;

                });
        };

        $scope.selectAll = function () {
            if ($scope.students.length != 0) {
                if ($scope.selectedAll) {
                    $scope.selectedAll = true;
                    $scope.selectedForDeletion = true;
                } else {
                    $scope.selectedAll = false;
                    $scope.selectedForDeletion = false;
                }

                angular.forEach($scope.students, function (student) {
                    student.isSelected = $scope.selectedAll;
                });
            }
        };

        $scope.checkSelected = function () {
            for (var i = 0; i < $scope.students.length; i++) {
                if ($scope.students[i].isSelected) {
                    $scope.selectedForDeletion = true;
                    break;
                }
                else {
                    $scope.selectedForDeletion = false;
                }
            }
        };

        $scope.resetTempStudent = function () {
            $scope.tempStudent.EnrollmentDate = {};
            $scope.tempStudent.EnrollmentDateString = '';
            $scope.tempStudent.Person = {};
            $scope.tempStudent.Class = {};
        }

        $scope.onGet();
    }
})();