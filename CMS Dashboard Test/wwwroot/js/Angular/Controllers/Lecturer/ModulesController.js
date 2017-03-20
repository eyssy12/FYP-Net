(function () {
    'use strict';

    angular
        .module(angularAppName)
        .controller('ModulesController', ModulesController);

    ModulesController.$inject = ['$scope', 'EnumerationsService', 'ModuleService']; 

    function ModulesController($scope, EnumerationsService, ModuleService)
    {
        $scope.title = 'ModulesController';
        $scope.boxColours =
        [
            'box-primary',
            'box-success',
            'box-warning',
            'box-danger',
            'box-info',
            'box-purple'
        ];

        EnumerationsService.getModuleTypes()
            .then(function (response) {
                $scope.moduleTypes = response.data;
            });


        ModuleService.getModulesForLecturer()
            .then(function (response) {
                $scope.modules = response.data;
            });

        $scope.getModuleTypeAsString = function (id) {

            for (var i = 0; i < $scope.moduleTypes.length; i++) {
                var moduleType = $scope.moduleTypes[i];

                if (id == moduleType.Id) {
                    return moduleType.Value;
                }
            }
        };

        $scope.prepareLinkToModuleView = function (module) {
            return '/Lecturer/ModuleView/' + module.Id;
        };

        $scope.getRandomBoxColour = function () {
            var randIndex = Math.floor(Math.random() * ($scope.boxColours.length - 0 + 1)) + 0;

            var boxColour = $scope.boxColours[randIndex]

            return boxColour;
        };
    }
})();