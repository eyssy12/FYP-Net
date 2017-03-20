(function () {
    'use strict';

    angular
        .module(loginAppName)
        .controller('ForgotPasswordController', ForgotPasswordController);

    ForgotPasswordController.$inject = ['$scope']; 

    function ForgotPasswordController($scope) 
    {
        $scope.title = 'ForgotPasswordController';

        $scope.processing = false;
        $scope.email = '';

        $scope.onRequestPasswordResetClick = function () {

            if ($scope.email.length > 0)
            {
                $scope.processing = true;
            }
        };
    }
})();
