(function () {
    'use strict';

    var apiBase = '/api/lecturers';

    angular
        .module(angularAppName)
        .factory('LecturerService', LecturerService);

    LecturerService.$inject = ['HttpService'];

    function LecturerService(HttpService) 
    {
        var service = {};

        service.get = function () {
            return HttpService.get(apiBase);
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