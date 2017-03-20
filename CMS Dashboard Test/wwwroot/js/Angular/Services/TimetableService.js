(function () {
    'use strict';

    var apiBase = '/api/timetables';
    var getTimetableForStudent = '/GetTimetableForStudent';
    var getTimetableForLecturer = '/GetTimetableForLecturer';
    var cancelClass = '/CancelClasses';

    angular
        .module(angularAppName)
        .factory('TimetableService', TimetableService);

    TimetableService.$inject = ['HttpService'];
    function TimetableService(HttpService) {
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
        service.getTimetableForStudent = function () {
            return HttpService.get(apiBase + getTimetableForStudent);
        }
        service.getTimetableForLecturer = function () {
            return HttpService.get(apiBase + getTimetableForLecturer);
        }
        service.cancelClasses = function (data) {
            return HttpService.deleteWithData(apiBase + cancelClass, data);
        }
        return service;
    }
})();