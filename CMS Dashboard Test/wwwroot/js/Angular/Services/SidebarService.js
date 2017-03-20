(function () {
    'use strict';

    angular
        .module(angularAppName)
        .factory('SidebarService', SidebarService);

    function SidebarService()
    {
        return {
            searchText: ''
        };
    }
})();