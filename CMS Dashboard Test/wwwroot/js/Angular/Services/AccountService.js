(function () {
    'use strict';

    var apiBase = '/api/applicationusers';
    var loggedInUserRoute = '/loggedinuser';

    angular
        .module(angularAppName)
        .factory('AccountService', AccountService);

    AccountService.$inject = ['HttpService'];

    function AccountService(HttpService) {

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

        service.getLoggedInUser = function () {
            return HttpService.get(apiBase + loggedInUserRoute)
        };

        return service;

    }
})();