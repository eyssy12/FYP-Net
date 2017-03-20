(function () {
    'use strict';

    angular
        .module(angularAppName)
        .constant('moment', moment)
        .controller('TimetableController', TimetableController);

    TimetableController.$inject = ['$scope', 'moment', 'uiCalendarConfig', 'AccountService', 'TimetableService']; 
    function TimetableController($scope, moment, uiCalendarConfig, AccountService, TimetableService) {
        $scope.loading = true;
        $scope.events = [];
        $scope.eventSources = [$scope.events];

        $scope.uiConfig = {
            calendar: {
                header: {
                    left: 'today',
                    center: 'title',
                    right: 'agendaWeek,agendaDay'
                },
                buttonText: {
                    today: 'today',
                    week: 'week',
                    day: 'day'
                },
                minTime: "8:00:00",
                maxTime: "19:00:00",
                scrollTime: "8:00:00",
                defaultView: 'agendaWeek',
                height: 800,
                selectHelper: true,
                editable: false,
                droppable: false,
                allDaySlot: false,
                viewRender: $scope.renderView,
                changeView: $scope.changeView
            }
        };

        TimetableService.getTimetableForStudent()
            .then(function (response) {
                $scope.timetable = response.data;
                $scope.populateCalendarFromExistingTimetable();
                $scope.loading = false;
            }, function (error) {
                $scope.loading = false;
            });

        $scope.populateCalendarFromExistingTimetable = function () {
            var fullCalendarObject = uiCalendarConfig.calendars['calendar'];

            angular.forEach($scope.timetable.Events, function (event) {
                var now = moment(new Date());
                var start = moment(event.Start);
                var end = moment(event.End);

                var weekDifference = now.week() - start.week();
                if (weekDifference > 0) {
                    start = jQuery.extend(true, {}, start);
                    start.add(weekDifference, 'weeks');

                    end = jQuery.extend(true, {}, end);
                    end.add(weekDifference, 'weeks');
                }

                var eventRebuilt =
                {
                    eventId: event.Id,
                    moduleId: event.Module.Id,
                    title: event.Title,
                    room: event.Room,
                    originalStart: event.Start,
                    originalEnd: event.End,
                    start: start,
                    end: end,
                    allDay: false,
                    backgroundColor: event.BackgroundColor,
                    borderColor: event.BorderColor,
                    backgroundColorDescriptive: event.BackgroundColorDescriptive // fix this part
                };

                fullCalendarObject.fullCalendar('renderEvent', eventRebuilt, true);
            });
        };
    }
})();