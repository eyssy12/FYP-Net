(function () {
    'use strict';

    angular
        .module(loginAppName)
        .controller('LoginController', LoginController);

    LoginController.$inject = ['$scope']; 

    function LoginController($scope) {
        $scope.title = 'LoginController';
        $scope.isLoggingIn = false;

        $scope.onLogin = function () {
            $scope.isLoggingIn = true;
        }
    }
})();