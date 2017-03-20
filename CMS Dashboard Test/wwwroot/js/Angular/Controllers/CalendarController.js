(function () {
    'use strict';

    angular
        .module(angularAppName)
        .constant('moment', moment)
        .controller('CalendarController', CalendarController);

    CalendarController.$inject = ['$scope', '$http', '$filter', '$compile', 'moment', 'uiCalendarConfig',
        'ClassService', 'CourseService', 'SemesterService', 'ModuleService', 'EnumerationsService', 'TimetableService'];

    function CalendarController($scope, $http, $filter, $compile, moment, uiCalendarConfig, ClassService, CourseService, SemesterService, ModuleService, EnumerationsService, TimetableService) {
        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();
        var currentView = "week";

        $scope.loadingCourses = true;
        $scope.loadingEnumerations = true;
        $scope.loadingClasses = true;
        $scope.loadingSemesters = true;
        $scope.loading = true;
        $scope.cachedClassWithExternalEvents = [];
        $scope.initExternalEvents = function ()
        {
            var elements = $('#external-events div.external-event');

            angular.forEach(elements, function (element)
            {
                var htmlObject = $(element);
                var associatedEventObject = {};

                for (var i = 0; i < $scope.externalEvents.length; i++)
                {
                    var thisEvent = $scope.externalEvents[i];
                    var divTitle = $.trim(htmlObject.text());
                    if (thisEvent.title == divTitle) {
                        associatedEventObject = thisEvent;
                        break;
                    }
                }

                var eventObject = {
                    title: associatedEventObject.title, // use the element's text as the event title
                    moduleId: associatedEventObject.moduleId,
                    classId: associatedEventObject.classId,
                    semesterId: associatedEventObject.semesterId,
                    backgroundColorDescriptive: associatedEventObject.backgroundColorDescriptive,
                    backgroundColor: associatedEventObject.backgroundColor,
                    borderColor: associatedEventObject.borderColor
                };

                var prepared = htmlObject.data('prepared');
                if (!prepared)
                {
                    htmlObject.data('prepared', true);
                    // store the Event Object in the DOM element so we can get to it later
                    htmlObject.data('eventObject', eventObject);
                    // make the event draggable using jQuery UI
                    htmlObject.draggable({
                        zIndex: 1070,
                        revert: true, // will cause the event to go back to its
                        revertDuration: 0  //  original position after the drag
                    });
                }
            });
        }

        $scope.clearAllCalendarEvents = function () {
            uiCalendarConfig.calendars['calendar'].fullCalendar('removeEvents'); // will remove all if no id filter is passed as second parameter
        };

        $scope.removeCalendarEventsByIds = function (ids) {
            uiCalendarConfig.calendars['calendar'].fullCalendar('removeEvents', ids);
        };

        $scope.prepareSemesters = function () {
            $scope.classSemesterModules = [];

            for (var i = 0; i < $scope.semesters.length; i++)
            {
                var semester = $scope.semesters[i];
                for (var j = 0; j < semester.Classes.length; j++)
                {
                    var semesterClass = semester.Classes[j];
                    if (semesterClass.Name == $scope.selectedClass.Name) {
                        $scope.classSemesterModules.push(semester);
                        $scope.modules = semester.modules;
                        break;
                    }
                }
            }
        };

        $scope.formatSemesterOptionsName = function (semester) {
            return "Semester " + semester.Number;
        };

        $scope.prepareSemestersForCourse = function () {
            $("#moduleColourPicker").addClass("disabled");

            $scope.changeAmount = 0;
            $scope.externalEvents = [];
            $scope.selectedSemester = undefined;
            $scope.selectedClass = undefined;
            $scope.clearAllCalendarEvents();

            var temp = [];
            angular.forEach($scope.courses, function (course) {
                if (course.Name == $scope.selectedCourseForSemesters.Name) {
                    $scope.courseSemesters = course.Semesters;
                }
            });
        };

        $scope.prepareClassesForSemester = function () {
            $scope.changeAmount = 0;
            $scope.externalEvents = [];
            $scope.clearAllCalendarEvents();
            $scope.selectedClass = undefined;
            $scope.semesterClasses = $scope.selectedSemester.Classes;
        };

        $scope.prepareModules = function ()
        {
            if (!(jQuery.isEmptyObject($scope.selectedClass) && jQuery.isEmptyObject($scope.selectedSemester)))
            {
                $scope.changeAmount = 0;
                $scope.externalEvents = [];

                var cachedClass = {};
                for (var i = 0; i < $scope.cachedClassWithExternalEvents.length; i++)
                {
                    var tempCached = $scope.cachedClassWithExternalEvents[i];
                    if (tempCached.classId == $scope.selectedClass.Id &&
                        tempCached.semesterId == $scope.selectedSemester.Id)
                    {
                        cachedClass = tempCached;
                        break;
                    }
                }

                var externalEvents = [];
                if (cachedClass == undefined || cachedClass == null || jQuery.isEmptyObject(cachedClass)) {
                    angular.forEach($scope.selectedSemester.Modules, function (module)
                    {
                        externalEvents.push({
                            title: module.Name,
                            allDay: false,
                            backgroundColor: "#0073b7", //Blue
                            borderColor: "#0073b7", //Blue
                            backgroundColorDescriptive: 'blue',
                            classId: $scope.selectedClass.Id,
                            semesterId: $scope.selectedSemester.Id,
                            moduleId: module.Id
                        });
                    });

                    var cachedClass =
                    {
                        classId: $scope.selectedClass.Id,
                        semesterId: $scope.selectedSemester.Id,
                        externalEvents: externalEvents
                    };

                    $scope.cachedClassWithExternalEvents.push(cachedClass);
                }
                else
                {
                    angular.forEach(cachedClass.externalEvents, function (externalEvent)
                    {
                        externalEvents.push(externalEvent);
                    });
                }

                $scope.externalEvents = externalEvents;
                if ($scope.externalEvents.lenght > 0)
                {
                    $("#moduleColourPicker").removeClass("disabled");
                }
                else
                {
                    $("#moduleColourPicker").addClass("disabled");
                }

                var timetable = $scope.selectedClass.Timetable;
                if (timetable == undefined || timetable == null)
                {
                    $scope.eventSources = [];
                    $scope.clearAllCalendarEvents();
                    $scope.newTimetable = true;
                }
                else
                {
                    $scope.newTimetable = false;
                    $scope.existingTimetable = timetable;
                    $scope.timetableName = timetable.Name;
                    $scope.existingTimetable.NewEvents = [];
                    $scope.existingTimetable.ModifiedEvents = [];
                    $scope.existingTimetable.EventIdsToRemove = []

                    $scope.existingTimetableCopy = jQuery.extend(true, {}, timetable); // true indicates a deep copy
                    $scope.populateCalendarFromExistingTimetable();
                }

                $scope.selectedEvent = undefined;
            }
        };

        $scope.populateCalendarFromExistingTimetable = function () {
            var fullCalendarObject = uiCalendarConfig.calendars['calendar'];

            angular.forEach($scope.existingTimetable.Events, function (event)
            {
                var now = moment(new Date());
                var start = moment(event.Start);
                var end = moment(event.End);
                var weekDifference = now.week() - start.week();
                if (weekDifference > 0)
                {
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

        $scope.revertChanges = function () {
            if (!jQuery.isEmptyObject($scope.existingTimetable))
            {
                $scope.existingTimetable = jQuery.extend(true, {}, $scope.existingTimetableCopy);
                $scope.existingTimetable.NewEvents = [];
                $scope.existingTimetable.ModifiedEvents = [];
                $scope.existingTimetable.EventIdsToRemove = [];

                $scope.clearAllCalendarEvents();
                $scope.populateCalendarFromExistingTimetable();
                $scope.changeAmount = 0;
                $scope.determineSaveButtonTooltipText();
            }
        };

        $scope.saveChanges = function () {

            if ($scope.changeAmount == 0)
            {
                return;
            }

            if ($scope.timetableName == undefined || $scope.timetableName.lenght < 1)
            {
                $scope.invalidTimetable = true;
            }
            else
            {
                $scope.invalidTimetable = false;
                $scope.loading = true;

                var timetable = {};
                timetable.Name = $scope.timetableName;
                timetable.ClassId = $scope.selectedClass.Id;
                timetable.Events = [];

                var fullCalendarObject = uiCalendarConfig.calendars['calendar'];
                var items = fullCalendarObject.fullCalendar('clientEvents');

                if ($scope.existingTimetable == undefined || $scope.existingTimetable == null)
                {
                    angular.forEach(items, function (event)
                    {
                        timetable.Events.push($scope.rebuildTimetableEventForBackend(event));
                    });

                    TimetableService.post(timetable)
                        .then(function (response)
                        {
                            $scope.changeAmount = 0;
                            $scope.determineSaveButtonTooltipText();
                            $scope.existingTimetable = timetable;
                            $scope.selectedClass.Timetable = $scope.existingTimetable;
                            $scope.loading = false;
                        }, function (error)
                        {
                            window.alert("Something happened: " + error.status)
                            $scope.loading = false;
                        });
                }
                else
                {
                    var modifiedEvents = [];
                    var newEvents = [];

                    angular.forEach($scope.existingTimetable.ModifiedEvents, function (event)
                    {
                        // duplicates from NewEvents get added in here when applying  the colour change to all events of the same type
                        if (event.eventId != undefined) {
                            modifiedEvents.push($scope.rebuildTimetableEventForBackend(event));
                        }

                        // modifiedEvents.push($scope.rebuildTimetableEventForBackend(event));
                    });

                    angular.forEach($scope.existingTimetable.NewEvents, function (event)
                    {
                        newEvents.push($scope.rebuildTimetableEventForBackend(event));
                    });

                    var modifiedTimetable =
                    {
                        Id: $scope.existingTimetable.Id,
                        Name: $scope.timetableName,
                        NewEvents: newEvents,
                        ModifiedEvents: modifiedEvents,
                        EventIdsToRemove: $scope.existingTimetable.EventIdsToRemove
                    };

                    TimetableService.put(modifiedTimetable.Id, modifiedTimetable)
                        .then(function (response)
                        {
                            // success, we can let go of this data now
                            $scope.existingTimetable.ModifiedEvents = [];
                            $scope.existingTimetable.NewEvents = [];
                            $scope.existingTimetable.EventIdsToRemove = [];
                            $scope.existingTimetableCopy = jQuery.extend(true, {}, $scope.existingTimetable);

                            $scope.changeAmount = 0;
                            $scope.loading = false;
                        }, function (error)
                        {
                            window.alert("something happened when trying to modify the timetable");

                            $scope.loading = false;
                        });
                }
            }
        };

        $scope.rebuildTimetableEventForBackend = function (event)
        {
            var timetableEvent =
            {
                Id: event.eventId,
                Title: event.title,
                Room: "no room",
                Start: event.start,
                End: event.end,
                BackgroundColorDescriptive: event.backgroundColorDescriptive,
                BackgroundColor: event.backgroundColor,
                BorderColor: event.borderColor,
                ModuleId: event.moduleId,
                Repeatable: event.isRepeatable,
                TimetableId: $scope.existingTimetable == undefined ? -1 : $scope.existingTimetable.Id
            };

            return timetableEvent;
        };
     
        //with this you can handle the events that generated by clicking the day(empty spot) in the calendar
        $scope.dayClick = function( date, allDay, jsEvent, view ){

        };
     
        $scope.externalEventDrop = function (date, allDay) {
            // retrieve the dropped element's stored Event Object
            var originalEventObject = $(this).data('eventObject');
            // we need to copy it, so that multiple events don't have a reference to the same object
            var copiedEventObject = $.extend({}, originalEventObject);
            var startDate = new Date(date._d.getTime());
            var endDate = new Date(date._d.getTime());
            // college starts at 9am and the minimum hours for a class is 1 hour
            // the check for 0 is in the case the admin drops the event on a month view
            // in the week view, the event is dropped on a specific time grid as expected
            if (startDate.getHours() == 0) {
                startDate.setHours(startDate.getHours() + 9);
            }
            if (endDate.getHours() == 0) {
                endDate.setHours(endDate.getHours() + 10);
            }
            // when an event is dropped in a week view, the start and end date have the same time, hence hours + 1
            if (startDate.getHours() == endDate.getHours()) {
                endDate.setHours(endDate.getHours() + 1);
            }
            // assign it the date that was reported
            copiedEventObject.start = startDate;
            copiedEventObject.end = endDate;
            copiedEventObject.original = true;
            copiedEventObject.stick = true; // fixes the dissapearance of events when switching between calendar views
            copiedEventObject.allDay = false;
            copiedEventObject.backgroundColorDescriptive = $scope.selectedColour.descriptive;
            copiedEventObject.backgroundColor = $(this).css("background-color");
            copiedEventObject.borderColor = $(this).css("border-color");

            $scope.addEvent(copiedEventObject);
        };

        $scope.createExternalEvent = function () {
            $scope.externalEvents.push({
                title: $scope.newExternalEventTemp.title,
                backgroundColor: $scope.selectedColour,
                borderColor: $scope.selectedColour,
                backgroundColorDescriptive: $scope.selectedColour
            });

            $scope.newExternalEventTemp.title = '';
            $scope.selectedColour = '';
        };
     
        $scope.determineEventChangeState = function (event) {
            var eventExistsAsANewEvent = false;

            if ($scope.newTimetable)
            {
                for (var i = 0; i < $scope.events.length; i++)
                {
                    var newEvent = $scope.events[i];
                }
            }
            else
            {
                for (var i = 0; i < $scope.existingTimetable.NewEvents.length; i++) {
                    var existingNewEvent = $scope.existingTimetable.NewEvents[i];
                    if (existingNewEvent.eventId == event.eventId) {
                        eventExistsAsANewEvent = true;
                        $scope.existingTimetable.NewEvents.splice(i, 1);
                        break;
                    }
                }

                if (!eventExistsAsANewEvent) {
                    var isNewModification = false;
                    for (var i = 0; i < $scope.existingTimetable.ModifiedEvents.length; i++) {
                        var modifiedEvent = $scope.existingTimetable.ModifiedEvents[i];

                        if (modifiedEvent.eventId == event.eventId) {
                            modifiedEvent = undefined;
                            isNewModification = true;
                            $scope.existingTimetable.ModifiedEvents.splice(i, 1);

                            break;
                        }
                    }

                    if (!isNewModification) {
                        $scope.changeAmount++;
                    }
                }

                if (eventExistsAsANewEvent) {
                    $scope.existingTimetable.NewEvents.push(event);
                }
                else {
                    $scope.existingTimetable.ModifiedEvents.push(event);
                }
            }
        };

        //with this you can handle the events that generated by droping any event to different position in the calendar
        $scope.alertOnDrop = function (event, dayDelta, minuteDelta, allDay, revertFunc, jsEvent, ui, view) {
            $scope.determineEventChangeState(event);
        };
     
        //with this you can handle the events that generated by resizing any event to different position in the calendar
        $scope.alertOnResize = function (event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view) {
            $scope.determineEventChangeState(event);
        };
     
        $scope.addEvent = function (event)
        {
            if ($scope.events.length > 0)
            {
                $("#moduleColourPicker").removeClass("disabled");
            }

            $scope.events.push(event);

            if (!jQuery.isEmptyObject($scope.existingTimetable))
            {
                $scope.existingTimetable.NewEvents.push(jQuery.extend(true, {}, event));
            }

            $scope.changeAmount++;
            $scope.determineSaveButtonTooltipText();
        };
     
        //with this you can handle the click on the events
        $scope.eventClick = function (event)
        {
            $scope.selectedEvent = event;
            $scope.events;
            // TODO: decide if on click the event should change its border colour ( to appear highlighted) and to allow bulk delete of events from the calendar
        };

        $scope.showDeleteTimetableConfirmDialog = function () {
            $('#deleteTimetableModal').modal('show');
        };

        $scope.deleteTimetable = function () {
            $scope.timetableName = '';

            TimetableService.delete($scope.existingTimetable.Id)
                .then(function (response) {
                    $scope.clearAllCalendarEvents();
                    $scope.selectedClass.Timetable = undefined;
                    $scope.existingTimetable = undefined;
                }, function (error) {
                });
        };

        $scope.getFormattedSelectedEventTime = function () {
            if ($scope.selectedEvent == undefined) {
                return '';
            }
            // moment.js
            var startTime = moment($scope.selectedEvent.start._d);
            var endTime = moment($scope.selectedEvent.end._d);

            var startDateFormatted= startTime.format('LL'); // eg. March 17, 2016 
            var startTimeFormatted = startTime.format('LT'); // eg. 1:37 PM

            var endTimeFormatted = endTime.format('LT');

            return startDateFormatted + ' @ ' + startTimeFormatted + ' - ' + endTimeFormatted;
        };

        $scope.getFormattedSelectedEventName = function () {
            if ($scope.selectedEvent == undefined)
            {
                return 'None';
            }

            return $scope.selectedEvent.title;
        };

        $scope.removeEventFromCalendar = function () {
            $scope.selectedEvent.isRepeatable = false;

            var id = $scope.selectedEvent._id;
            var eventId = $scope.selectedEvent.eventId;

            $scope.removeCalendarEventsByIds([id]);
            $scope.selectedEvent = undefined;

            var externalEventChange = false;
            for (var i = 0; i < $scope.existingTimetable.NewEvents.length; i++)
            {
                var event = $scope.existingTimetable.NewEvents[i];

                if (event.eventId == eventId)
                {
                    externalEventChange = true;
                    break;
                }
            }

            // if a new external event has been added to the existing timetable, 
            // it's not a new change since it didnt exist there from the start
            if (externalEventChange)
            {
                $scope.changeAmount--;
            }
            else
            {
                if ($scope.existingTimetable != undefined || $scope.existingTimetable != null)
                {
                    $scope.changeAmount++;
                    $scope.existingTimetable.EventIdsToRemove.push(eventId);
                }
                else
                {
                    $scope.changeAmount--;
                }
            }
        };
     
        $scope.eventRender = function (event, element, view) {
            element.attr({
                'tooltip': event.title,
                'tooltip-append-to-body': true
            });

            $compile(element)($scope);
        };
     
        //with this you can handle the events that generated by each page render process
        $scope.renderView = function (view) {

            var date = new Date(view.calendar.getDate());
            $scope.currentDate = date.toDateString();

        };
 
        //with this you can handle the events that generated when we change the view i.e. Month, Week and Day
        $scope.changeView = function (view, calendar) {
            uiCalendarConfig.calendars[calendar].fullCalendar('changeView', view);
            alert('You are looking at ' + currentView)
        };

        $scope.setSelectedColour = function (colour) {
            // if the same colour is picked, we dont want to reiterate through the calendar as its pointless
            if (colour.descriptive != $scope.newExternalEventTemp.backgroundColorDescriptive) {
                $scope.selectedColour = colour;
                $scope.newExternalEventTemp.backgroundColorDescriptive = colour.descriptive;

                var fullCalendarObject = uiCalendarConfig.calendars['calendar'];
                // TODO: figure out why setting the colours doesnt change inside the calendar itself
                // RESOLVED: wasnt retrieving the right objects, needed to call the 'clientEvents' of the fullCalendar to get the internal objects
                var items = fullCalendarObject.fullCalendar('clientEvents');
                var modified = [];
                for (var i = 0; i < items.length; i++)
                {
                    var calendarEvent = items[i];
                    if (calendarEvent.title == $scope.newExternalEventTemp.title) {
                        calendarEvent.backgroundColor = colour.hex;
                        calendarEvent.borderColor = colour.hex;
                        calendarEvent.backgroundColorDescriptive = colour.descriptive;


                        modified.push(calendarEvent);
                        // not breaking here because the intention is to have repeating events for a week which will have the same title
                    }
                }

                for (var i = 0; i < modified.length; i++)
                {
                    var modifiedTemp = modified[i];
                    if ($scope.existingTimetable.ModifiedEvents.length > 0) {
                        var shouldAddToModifedEvents = true;

                        for (var j = 0; j < $scope.existingTimetable.ModifiedEvents.length; j++)
                        {
                            var exisitingModified = $scope.existingTimetable.ModifiedEvents[j];
                            if (modifiedTemp.eventId == exisitingModified.eventId) {
                                exisitingModified.backgroundColorDescriptive = modifiedTemp.backgroundColorDescriptive;
                                shouldAddToModifedEvents = false;
                                break;
                            }
                            else
                            {
                                shouldAddToModifedEvents = true;
                            }
                        }

                        if (shouldAddToModifedEvents) {
                            $scope.changeAmount++;
                            $scope.existingTimetable.ModifiedEvents.push(modifiedTemp);
                        }
                    }
                    else
                    {
                        $scope.changeAmount++;
                        $scope.existingTimetable.ModifiedEvents.push(modifiedTemp);
                    }
                }

                fullCalendarObject.fullCalendar('rerenderEvents');
            }
        };

        $scope.setSelectedEventRepeatable = function ()
        {
            if ($scope.selectedEvent.isRepeatable == true)
            {
                var startDate = new Date($scope.selectedEvent.start._d.getTime());
                var endDate = new Date($scope.selectedEvent.end._d.getTime());

                for (var i = 0; i < 2; i++)
                {
                    var repeatedStartDate = new Date(startDate.getTime());
                    var repeatedEndDate = new Date(endDate.getTime());

                    startDate.setDate(startDate.getDate() + 7);
                    endDate.setDate(endDate.getDate() + 7);

                    repeatedStartDate.setTime(startDate.getTime());
                    repeatedEndDate.setTime(endDate.getTime());

                    var repeatedEvent = {
                        title: $scope.selectedEvent.title,
                        allDay: $scope.selectedEvent.allDay,
                        start: repeatedStartDate,
                        end: repeatedEndDate,
                        stick: true,
                        backgroundColor: $scope.selectedEvent.backgroundColor,
                        borderColor: $scope.selectedEvent.borderColor,
                        backgroundColorDescriptive: $scope.selectedEvent.backgroundColorDescriptive,
                        originalReference: $scope.selectedEvent,
                        original: false
                    };

                    $scope.events.push(repeatedEvent);
                }
            }
            else
            {
                $scope.selectedEvent.isRepeatable = false;
                var fullCalendarObject = uiCalendarConfig.calendars['calendar'];
                angular.forEach($scope.events, function (event) {
                    // removes all events that are associated with a particular module and are not originals themselves
                    // the original event will remain on the calendar as expected.
                    if ($scope.selectedEvent.title == event.title && !event.original) {
                        fullCalendarObject.fullCalendar('removeEvents', event._id);
                    }
                });
            }
        };

        $scope.formatClassOptionsName = function (classObj) {
            if ($scope.enrollmentStages == undefined || $scope.enrollmentStages == null) {
                return '';
            }

            var enrollmentStage = '';
            for (var i = 0; i < $scope.enrollmentStages.length; i++)
            {
                if ($scope.enrollmentStages[i].Id == classObj.EnrollmentStage) {
                    enrollmentStage = $scope.enrollmentStages[i].Value;
                    break;
                }
            }

            return classObj.Name + ' - ' + enrollmentStage + ' year - ' + classObj.Students.length + ' students';
        }

        $scope.determineSaveButtonTooltipText = function () {
            if ($scope.changeAmount < 1)
            {
                $scope.saveButtonTooltipText = 'No changes to save';
            }
            else
            {
                $scope.saveButtonTooltipText = 'Save ' + $scope.changeAmount + ' changes';
            }
        }
     
        /* config object */
        $scope.uiConfig = {
            calendar:{
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
                scrollTime :  "8:00:00",
                defaultView: 'agendaWeek',
                height: 800,
                selectHelper: true,
                editable: true,
                droppable: true,
                allDaySlot: false,
                dayClick: $scope.dayClick,
                drop: $scope.externalEventDrop,
                eventDrop: $scope.alertOnDrop, // this is will get called only when an event from the calendar is dropped to another part of the calendar
                eventResize: $scope.alertOnResize,
                eventClick: $scope.eventClick,
                eventRender: $scope.eventRender,
                viewRender: $scope.renderView,
                changeView: $scope.changeView
            }    
        };

        $scope.newTimetable = false;
        $scope.invalidTimetable = false;
        $scope.selectedColour = { descriptive: 'blue', hex: '#0073b7' };
        $scope.existingTimetable = undefined;
        $scope.existingTimetableCopy = undefined;
        $scope.changeAmount = 0;
        $scope.newExternalEventTemp = {};
        $scope.externalEvents = [];
        $scope.selectedClass = undefined;
        $scope.selectedCourseForSemesters = undefined;
        $scope.saveButtonTooltipText = 'No changes to save';
        $scope.events = [];
        $scope.colours =
        [
            { descriptive: 'aqua', hex: '#00c0ef' },
            { descriptive: 'blue', hex: '#0073b7' },
            { descriptive: 'light-blue', hex: '#3c8dbc' },
            { descriptive: 'teal', hex: '#39cccc' },
            { descriptive: 'yellow', hex: '#f39c12' },
            { descriptive: 'orange', hex: '#ff851b' },
            { descriptive: 'green', hex: '#00a65a' },
            { descriptive: 'lime', hex: '#01ff70' },
            { descriptive: 'red', hex: '#dd4b39' },
            { descriptive: 'purple', hex: '#605ca8' },
            { descriptive: 'navy', hex: '#001f3f' },
        ];

        EnumerationsService.getEnrollmentStages()
            .then(function (response) {
                $scope.enrollmentStages = response.data;
                $scope.loadingEnumerations = false;
                $scope.checkLoadingState();
            }, function (error) {
                $scope.loadingEnumerations = false;
                $scope.checkLoadingState();
            });

        ClassService.get()
            .then(function (response) {
                $scope.classes = response.data;
                $scope.loadingClasses = false;
                $scope.checkLoadingState();
            }, function (error) {
                $scope.loadingClasses = false;
                $scope.checkLoadingState();
            });

        CourseService.get()
            .then(function (response) {
                $scope.courses = response.data;
                $scope.loadingCourses = false;
                $scope.checkLoadingState();
            });

        SemesterService.get()
            .then(function (response) {
                $scope.semesters = response.data;
                $scope.loadingSemesters = false;
                $scope.checkLoadingState();
            });

        $scope.checkLoadingState = function ()
        {
            if (!($scope.loadingClasses || $scope.loadingSemesters || $scope.loadingEnumerations || $scope.loadingCourses))
            {
                $scope.loading = false;
            }
            else
            {
                $scope.loading = true;
            }
        }

        /* event sources array*/
        $scope.eventSources = [$scope.events];
    }
})();