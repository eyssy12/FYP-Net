(function () {
    'use strict';

    var apiBase = '/api/courses';

    angular
        .module(angularAppName)
        .factory('CourseService', CourseService);

    CourseService.$inject = ['HttpService']; 

    function CourseService(HttpService)
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
