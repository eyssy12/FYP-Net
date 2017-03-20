(function () {
    'use strict';

    angular
        .module(angularAppName)
        .controller('ModulesController', ModulesController);

    ModulesController.$inject = ['$scope', 'EnumerationsService', 'ModuleService']; 
    function ModulesController($scope, EnumerationsService, ModuleService) {
        $scope.title = 'ModulesController';

        ModuleService.getModulesForStudent()
            .then(function (response) {
                $scope.modules = response.data.Modules;
                $scope.students = response.data.Students;
            }, function (error) {
            });

        EnumerationsService.getModuleTypes()
            .then(function (response) {
                $scope.moduleTypes = response.data;
            });

        $scope.prepareLinkToModuleView = function (module) {
            return '/Student/ModuleView/' + module.Id;
        };

        $scope.determineStudentCountForModule = function (module) {
            var moduleTypeAsString = $scope.getModuleTypeAsString(module.ModuleType);

            if (moduleTypeAsString == "Mandatory") {
                return $scope.students.length;
            }

            return '0';
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