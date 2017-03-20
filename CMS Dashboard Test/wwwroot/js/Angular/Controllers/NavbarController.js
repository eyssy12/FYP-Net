(function () {
    'use strict';

    angular
        .module(angularAppName)
        .controller('NavbarController', NavbarController);

    NavbarController.$inject = ['$scope', 'AccountService']; 

    function NavbarController($scope, AccountService) {
        $scope.title = 'NavbarController';

        $scope.notifications = {};
        $scope.notifications.count = 0;
        $scope.notifications.data = {};
        

        $scope.checkCount = function () {
            if ($scope.notifications.count <= 0)
            {
                $scope.notifications.status = "No new notifications";
            }
            else
            {
                $scope.notifications.status = "You have " + $scope.notifications.count + " new notifications";
            }
        }

        $scope.checkCount();
    }
})();
