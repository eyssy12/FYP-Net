(function () {
    'use strict';

    var apiBase = '/api/persons';
    var getAvailablePersons = '/availablepersons';
    var getAccountlessPersons = '/accountlesspersons';
    
    angular
        .module(angularAppName)
        .factory('PersonService', PersonService);

    PersonService.$inject = ['HttpService'];

    function PersonService(HttpService)
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

        service.getAvailablePersons = function () {
            return HttpService.get(apiBase + getAvailablePersons);
        }

        service.getAccountlessPersons = function ()
        {
            return HttpService.get(apiBase + getAccountlessPersons);
        }

        return service;
    }
})();