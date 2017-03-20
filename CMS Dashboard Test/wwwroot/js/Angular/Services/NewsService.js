(function () {
    'use strict';

    var apiBase = '/api/news';

    angular
        .module(angularAppName)
        .factory('NewsService', NewsService);

    NewsService.$inject = ['HttpService'];

    function NewsService(HttpService)
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

        return service;
    }
})();