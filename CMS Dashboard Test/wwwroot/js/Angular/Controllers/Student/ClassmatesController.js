(function () {
    'use strict';

    angular
        .module(angularAppName)
        .constant('moment', moment)
        .controller('ClassmatesController', ClassmatesController);

    ClassmatesController.$inject = ['$scope', 'moment', 'ClassService']; 

    function ClassmatesController($scope, moment, ClassService) {
        $scope.title = 'ClassmatesController';

        ClassService.getClassForStudent()
            .then(function (response) {
                $scope.class = response.data;
                $scope.students = $scope.class.Students;
            });

        $scope.getBirthdateFormatted = function (birthdate) {
            var date = moment(birthdate);

            return date.format('LL');
        };
    }
})();