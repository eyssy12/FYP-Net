(function () {
    'use strict';

    var apiBase = '/api/modules';
    var modulesAllInclusive = '/modulesallinclusive';
    var getModulesForStudent = '/getmodulesforstudent';
    var getModulesForLecturer = '/getmodulesforlecturer';

    angular
        .module(angularAppName)
        .factory('ModuleService', ModuleService);

    ModuleService.$inject = ['HttpService'];

    function ModuleService(HttpService)
    {
        var service = {};

        service.get = function () {
            return HttpService.get(apiBase);
        }

        service.getById = function (id) {
            return HttpService.get(apiBase + '/' + id);
        }

        service.post = function (data) {
            return HttpService.post(apiBase, data);
        }

        service.put = function (id, data) {
            return HttpService.put(apiBase + '/', id, data);
        }

        service.delete = function (id) {
            return HttpService.delete(apiBase + '/', id);
        }

        service.bulkDelete = function (idArrayViewModel) {
            return HttpService.bulkDelete(apiBase, idArrayViewModel);
        }

        service.getModulesAllInclusive = function () {
            return HttpService.get(apiBase + modulesAllInclusive);
        }

        service.getModulesForStudent = function ()
        {
            return HttpService.get(apiBase + getModulesForStudent);
        }

        service.getModulesForLecturer = function ()
        {
            return HttpService.get(apiBase + getModulesForLecturer);
        }

        return service;
    }
})();