(function () {
    'use strict';

    angular
        .module(angularAppName)
        .controller('SidebarController', SidebarController);

    SidebarController.$inject = ['$scope', 'SidebarService', 'AccountService']; 

    function SidebarController($scope, SidebarService, AccountService)
    {
        $scope.title = 'SidebarController';
        $scope.sidebar = SidebarService;
    }
})();
