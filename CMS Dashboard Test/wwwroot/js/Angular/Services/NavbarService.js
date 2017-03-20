(function () {
    'use strict';

    angular
        .module(angularAppName)
        .factory('NavbarService', NavbarService);

    NavbarService.$inject = [];

    function NavbarService($http) 
    {
        return
        {
             notifications: []
        };
    }
})();