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
        $scope.selectedEvent = {};
        $scope.selectedForCancelation = [];

        // any methods in relation to uiConfig must be before uiConfig is configured
        $scope.timetableEventClick = function (event)
        {
            $scope.selectedEvent = event;

            var newSelection = true;

            for (var i = 0; i < $scope.selectedForCancelation.length; i++)
            {
                var selected = $scope.selectedForCancelation[i];
                if (selected.eventId == $scope.selectedEvent.eventId) {
                    newSelection = false;
                    $scope.selectedForCancelation.splice(i, 1);

                    break;
                }
            }

            if (newSelection){
                // need to do this only once because the timetable events cant change dynamically 
                if (jQuery.isEmptyObject($scope.selectedEvent.startEndTimeFormatted)) {
                    $scope.selectedEvent.startEndTimeFormatted = $scope.formatStartEndTimeForEvent($scope.selectedEvent);
                }

                $scope.selectedForCancelation.push($scope.selectedEvent);
            }
        };

        $scope.eventMouseoverHandler = function (event) {
            jQuery(this).css('cursor', 'pointer');
        };

        $scope.eventMouseoutHandler = function (event) {
            jQuery(this).css('cursor', 'default');
        };

        $scope.formatStartEndTimeForEvent = function (event) {
            var startTime = moment(event.start._d);
            var endTime = moment(event.end._d);

            var startDateFormatted = startTime.format('dddd'); // eg. March 17, 2016 
            var startTimeFormatted = startTime.format('LT'); // eg. 1:37 PM

            var endTimeFormatted = endTime.format('LT');

            return startDateFormatted + ' @ ' + startTimeFormatted + ' - ' + endTimeFormatted;
        };

        $scope.cancelClasses = function () {
            $scope.loading = true;

            var cancelledEvents = [];
            angular.forEach($scope.selectedForCancelation, function (event)
            {
                var cancelledEvent =
                {
                    CancelledBy: $scope.user.UserName,
                    Timestamp: new Date(),
                    CancellationEventId: event.eventId
                };

                cancelledEvents.push(cancelledEvent);
            });

            TimetableService.cancelClasses(cancelledEvents)
                .then(function (response) {
                    $scope.loading = false;
                    $scope.selectedForCancelation = [];
                }, function (error) {
                    $scope.loading = false;
                    window.alert(error.status);
                });
        };

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
                editable: true,
                droppable: true,
                allDaySlot: false,
                eventClick: $scope.timetableEventClick,
                viewRender: $scope.renderView,
                changeView: $scope.changeView,
                eventMouseover: $scope.eventMouseoverHandler,
                eventMouseout: $scope.eventMouseoutHandler
            }
        };

        AccountService.getLoggedInUser()
            .then(function (response)
            {
                $scope.user = response.data;
            });

        TimetableService.getTimetableForLecturer()
            .then(function (response) {
                $scope.events = response.data;

                $scope.populateCalendarFromExistingTimetable();
                $scope.loading = false;

            }, function (error) {
                $scope.loading = false;
            });

        $scope.populateCalendarFromExistingTimetable = function () {
            var fullCalendarObject = uiCalendarConfig.calendars['calendar'];

            angular.forEach($scope.events, function (event) {

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
                    durationEditable: false,
                    startEditable: false,
                    backgroundColor: event.BackgroundColor,
                    borderColor: event.BorderColor,
                    backgroundColorDescriptive: event.BackgroundColorDescriptive,
                };

                fullCalendarObject.fullCalendar('renderEvent', eventRebuilt, true);
            });
        };

        $scope.eventSources = [$scope.events];
    }
})();