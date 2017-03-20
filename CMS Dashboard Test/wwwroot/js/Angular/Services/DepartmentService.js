(function () {
    'use strict';
    
    var apiBase = '/api/departments';

    angular
        .module(angularAppName)
        .factory('DepartmentService', DepartmentService);

    DepartmentService.$inject = ['HttpService'];

    function DepartmentService(HttpService) {

        var service = {};

        service.get = function () {
            return HttpService.get(apiBase);
        }

        service.getWithoutComplexTypes = function () {
            return HttpService.get(apiBase + '/withoutComplexTypes');
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

        return service;
    }
})();