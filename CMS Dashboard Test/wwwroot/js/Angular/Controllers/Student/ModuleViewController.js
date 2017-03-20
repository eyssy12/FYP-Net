(function () {
    'use strict';

    angular
        .module(angularAppName)
        .controller('ModuleViewController', ModuleViewController);

    ModuleViewController.$inject = ['$scope', 'EnumerationsService', 'ModuleService']; 
    function ModuleViewController($scope, EnumerationsService, ModuleService) {
        $scope.loading = true;
        $scope.enumerationsLoaded = false;
        $scope.modulesLoaded = false;

        $scope.init = function (moduleId) {
            $scope.moduleId = moduleId;

            EnumerationsService.getModuleTypes()
                .then(function (response) {
                    $scope.moduleTypes = response.data;
                    $scope.enumerationsLoaded = true;
                    $scope.checkIsDataLoaded();
                });

            ModuleService.getById(moduleId)
                .then(function (response) {
                    $scope.module = response.data;
                    $scope.modulesLoaded = true;
                    $scope.checkIsDataLoaded();
                });
        };

        $scope.checkIsDataLoaded = function () {
            if ($scope.enumerationsLoaded && $scope.modulesLoaded) {
                $scope.loading = false;
            }
        };

        $scope.getModuleTypeAsString = function (id) {
            for (var i = 0; i < $scope.moduleTypes.length; i++) {
                var moduleType = $scope.moduleTypes[i];

                if (id == moduleType.Id) {
                    return moduleType.Value;
                }
            }
        };
    }
})();