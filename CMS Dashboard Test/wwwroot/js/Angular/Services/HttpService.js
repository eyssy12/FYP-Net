(function () {
    'use strict';

    var bulkDelete = '/bulkDelete/';

    angular
        .module(angularAppName)
        .factory('HttpService', HttpService);

    HttpService.$inject = ['$http'];
    function HttpService($http)
    {
        var service = {};
        service.get = function (url, cache) {
            return $http.get(url, { cache: cache });
        }
        service.post = function (url, data) {
            return $http.post(url, data);
        }
        service.put = function (url, id, data) {
            return $http.put(url + id, data);
        }
        service.delete = function (url, id) {
            return $http.delete(url + id);
        }
        service.deleteWithData = function (apiUrl, data) {
            return $http.post(apiUrl, data);
        }
        service.bulkDelete = function (apiBase, idArrayModel) {
            return $http.post(apiBase + bulkDelete, idArrayModel);
        }
        return service;
    }
})();