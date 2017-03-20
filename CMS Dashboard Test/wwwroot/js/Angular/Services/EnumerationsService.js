(function () {
    'use strict';

    var apiBase = '/api/enumerations',
        degreeAwards = '/degreeawards',
        enrollmentstages = '/enrollmentstages',
        userRoles = '/userroles',
        moduleTypes = '/moduletypes';

    angular
        .module(angularAppName)
        .factory('EnumerationsService', EnumerationsService);

    EnumerationsService.$inject = ['$http', 'HttpService'];

    function EnumerationsService($http, HttpService) {

        var service = {};

        service.getAwardTypes = function ()
        {
            return HttpService.get(apiBase + degreeAwards, true);
        }

        service.getEnrollmentStages = function () {
            return HttpService.get(apiBase + enrollmentstages, true);
        }

        service.getUserRoles = function () {
            return HttpService.get(apiBase + userRoles, true);
        }

        service.getModuleTypes = function () {
            return HttpService.get(apiBase + moduleTypes, true);
        }

        return service;
    }
})();