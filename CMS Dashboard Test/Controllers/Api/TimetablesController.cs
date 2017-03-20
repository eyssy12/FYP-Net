namespace CMS.Dashboard.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Extensions;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using PushSharp.Google;
    using Serialization;
    using Services;
    using ViewModels.Configuration;

    [Produces("application/json")]
    [Route("api/Timetables")]
    [Authorize(Roles = "Student,Lecturer,Admin")]
    public class TimetablesController : ApiControllerBase<Timetable>
    {
        private static int EqualDateTimes = 0;
        private readonly INotifyService<GcmNotification> MobileClientNotifier;

        public TimetablesController(ILoggerFactory loggerFactory, Entities context, MyJsonSerializerSettings settings, INotifyService<GcmNotification> mobileClientNotifier)
            : base(loggerFactory, context, settings)
        {
            this.MobileClientNotifier = mobileClientNotifier;
            this.MobileClientNotifier.Start();
        }

        // GET: api/Timetables
        [HttpGet]
        public IActionResult GetTimetables()
        {
            IEnumerable<Timetable> timetable = this.Context.Timetables.ToArray();

            return Json(timetable, this.Settings);
        }

        [Route("GetTimetableForStudent")]
        [Authorize(Roles = "Student,Admin")]
        public IActionResult GetTimetableForStudent()
        {
            ApplicationUser user = this.GetCurrentApplicationUserFromPrincipal();

            Student student = this.Context.Students
                .Include(s => s.Class)
                    .ThenInclude(s => s.Timetable)
                        .ThenInclude(s => s.Events)
                            .ThenInclude(s => s.Module)
                .SingleOrDefault(s => s.StudentPerson.PersonId == user.Person.Id);

            return Json(student.Class.Timetable, this.Settings);
        }

        [Route("GetTimetableForLecturer")]
        [Authorize(Roles = "Lecturer,Admin")]
        public IActionResult GetTimetableForLecturer()
        {
            ApplicationUser user = this.GetCurrentApplicationUserFromPrincipal();

            Lecturer lecturer = this.Context.Lecturers
                .Include(s => s.LecturerPerson)
                .ToArray()
                .SingleOrDefault(s => s.LecturerPerson.PersonId == user.Person.Id);

            IEnumerable<Event> events = this.Context.Events
                .Include(s => s.Module)
                    .ThenInclude(s => s.LecturerModules)
                .ToArray()
                .Where(s => s.Module.LecturerModules.Any(e => e.LecturerId == lecturer.Id))
                .ToArray();

            return Json(events, this.Settings);
        }

        [Route("CancelClasses")]
        [HttpPost]
        public IActionResult CancelClasses([FromBody] IEnumerable<CancelledEventViewModel> cancelledEvents)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            IEnumerable<Timetable> timetables = this.Context.Timetables
                .Include(s => s.Events)
                .Include(s => s.Class)
                    .ThenInclude(s => s.Students)
                        .ThenInclude(s => s.StudentPerson)
                            .ThenInclude(s => s.Person)
                                .ThenInclude(s => s.MobileClients)
                .ToArray();

            IDictionary<Timetable, IList<CancelledEventViewModel>> affectedTimetables = new Dictionary<Timetable, IList<CancelledEventViewModel>>();

            // separate this to a method
            cancelledEvents.ForEach(cancelledEvent =>
            {
                Timetable affected = timetables.SingleOrDefault(s => s.Events.Any(e => e.Id == cancelledEvent.CancellationEventId));

                if (affected != null)
                {
                    if (affectedTimetables.ContainsKey(affected))
                    {
                        IList<CancelledEventViewModel> cancelledEventsByTimetable = affectedTimetables[affected];
                        cancelledEventsByTimetable.Add(cancelledEvent);
                    }
                    else
                    {
                        affectedTimetables.Add(affected, new List<CancelledEventViewModel> { cancelledEvent });
                    }
                }
            });

            if (affectedTimetables.Any())
            {
                IEnumerable<Event> allEvents = this.Context.Events.ToArray();
                List<CancelledEvent> cancelledEventsToAdd = new List<CancelledEvent>();
                List<GcmNotification> notificationsToSend = new List<GcmNotification>();

                affectedTimetables.ForEach(affected =>
                {
                    cancelledEventsToAdd.AddRange(affected.Value.ToModels(allEvents));

                    JObject response = this.BuildCancelledClassResponse(affected.Value.Select(s => s.CancellationEventId));
                    IEnumerable<string> mobileIds = this.RetrieveMobileClientsIds(affected.Key);

                    GcmNotification notification = mobileIds.CreateNotification(response, GcmNotificationPriority.High);

                    notificationsToSend.Add(notification);
                });

                this.Context.CancelledEvents.AddRange(cancelledEventsToAdd);
                this.Context.SaveChanges();

                notificationsToSend.ForEach(s => this.MobileClientNotifier.SendNotification(s));

                this.MobileClientNotifier.Stop();

                return new HttpOkResult();
            }

            return HttpNotFound();
        }

        // GET: api/Timetables/5
        [HttpGet("{id}", Name = "GetTimetable")]
        public IActionResult GetTimetable([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Timetable timetable = this.Context.Timetables.Single(m => m.Id == id);

            if (timetable == null)
            {
                return HttpNotFound();
            }

            return Ok(timetable);
        }


        // PUT: api/Timetables/5
        [HttpPut("{id}")]
        public IActionResult PutTimetable(int id, [FromBody] ModifedTimetableViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != viewModel.Id)
            {
                return HttpBadRequest();
            }

            Timetable timetable = this.Context.Timetables
                .Include(s => s.Class)
                    .ThenInclude(s => s.Students)
                        .ThenInclude(s => s.StudentPerson)
                            .ThenInclude(s => s.Person)
                                .ThenInclude(s => s.MobileClients)
                .Include(s => s.Events)
                .SingleOrDefault(s => s.Id == viewModel.Id);

            if (timetable.Name != viewModel.Name)
            {
                timetable.Name = viewModel.Name;
            }

            bool shouldNotify;
            this.UpdateTimetableWithModifications(timetable, viewModel.ModifiedEvents, out shouldNotify);
            IEnumerable<Event> newEvents = this.UpdateTimetableWithNewEvents(timetable, viewModel.NewEvents); // need this to return the added events so that the id values get generated after changes are saved on the db
            this.UpdateTimetableWithRemovedEvents(timetable, viewModel.EventIdsToRemove);

            try
            {
                this.Context.Entry(timetable).State = EntityState.Modified;
                this.Context.SaveChanges();

                if (viewModel.NewEvents.Count() > 0 || viewModel.EventIdsToRemove.Count() > 0)
                {
                    shouldNotify = true;
                }

                // if only colours are changed then we have no need to inform the user
                if (!shouldNotify)
                {
                    return Ok();
                }

                JObject response = this.BuildResponse(
                    viewModel.ModifiedEvents.Select(s => s.Id).ToArray(), 
                    newEvents.Select(s => s.Id).ToArray(), 
                    viewModel.EventIdsToRemove.ToArray());

                IEnumerable<string> ids = this.RetrieveMobileClientsIds(timetable);
                GcmNotification notification = ids.CreateNotification(response, GcmNotificationPriority.High);

                this.SendNotificationAndStop(notification);

                return Ok();
            }
            catch
            {
                return HttpBadRequest();
            }
        }

        protected JObject BuildCancelledClassResponse(IEnumerable<int> cancelledEventIds)
        {
            JObject response =
               new JObject(
                   new JProperty("contents",
                       new JObject(
                           new JProperty("message_type", "notification"),
                           new JProperty("timetable_change_cancelled_events",
                               new JObject(
                                   new JProperty("cancelled_event_ids", cancelledEventIds))))));

            return response;
        }

        protected JObject BuildResponse(
            IEnumerable<int> modifiedEvents,
            IEnumerable<int> newEvents,
            IEnumerable<int> removedEvents)
        {
            JObject response =
               new JObject(
                   new JProperty("contents",
                       new JObject(
                           new JProperty("message_type", "notification"),
                           new JProperty("timetable_change",
                               new JObject(
                                   new JProperty("modified_events", modifiedEvents),
                                   new JProperty("new_events", newEvents),
                                   new JProperty("removed_events", removedEvents))))));

            return response;
        }

        protected void SendNotificationAndStop(GcmNotification notification)
        {
            this.MobileClientNotifier.SendNotification(notification);
            this.MobileClientNotifier.Stop();
        }

        private void UpdateTimetableWithRemovedEvents(Timetable timetable, IEnumerable<int> eventIdsToRemove)
        {
            // remove the events from the timetable after we've done modifications of the exsting ones
            // theoretically, it's faster as you've less to iterate thru now when searching for the right event by id
            foreach (int eventToRemove in eventIdsToRemove)
            {
                Event @event = this.Context.Events.SingleOrDefault(s => s.Id == eventToRemove);

                if (@event != null)
                {
                    timetable.Events.Remove(@event);
                }
            }
        }

        private IEnumerable<Event> UpdateTimetableWithNewEvents(Timetable timetable, IEnumerable<EventViewModel> viewModelEvents)
        {
            ICollection<Event> newEvents = this.BuildEvents(viewModelEvents).ToArray();

            // now add the new ones we added in the UI
            newEvents.ForEach(newEvent =>
            {
                timetable.Events.Add(newEvent);
            });

            return newEvents;
        }

        // POST: api/Timetables
        [HttpPost]
        public IActionResult PostTimetable([FromBody] TimetableViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Class @class = this.Context.Classes.SingleOrDefault(s => s.Id == viewModel.ClassId);

            Timetable timetable = new Timetable
            {
                Class = @class,
                Name = viewModel.Name
            };

            timetable.Events = this.BuildEvents(viewModel.Events).ToArray();

            try
            {
                this.Context.Timetables.Add(timetable);
                this.Context.SaveChanges();

                return Ok();
            }
            catch (DbUpdateException)
            {
                if (TimetableExists(timetable.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }

                return HttpBadRequest();
            }
        }

        // DELETE: api/Timetables/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTimetable(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Timetable timetable = this.Context.Timetables.Include(s => s.Events).Single(m => m.Id == id);
            if (timetable == null)
            {
                return HttpNotFound();
            }

            this.Context.Timetables.Remove(timetable);
            this.Context.SaveChanges();

            return Ok(timetable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Context.Dispose();
            }
            base.Dispose(disposing);
        }

        protected bool TimetableExists(int id)
        {
            return this.Context.Timetables.Count(e => e.Id == id) > 0;
        }

        protected IEnumerable<string> RetrieveMobileClientsIds(Timetable timetable)
        {
            return timetable
                .Class
                .Students
                .SelectMany(s => s.StudentPerson
                    .Person
                    .MobileClients
                    .Select(a => a.Token));
        }

        protected IEnumerable<Event> BuildEvents(IEnumerable<EventViewModel> viewModelEvents)
        {
            return viewModelEvents
                .Select(e =>
                {
                    Module module = this.Context.Modules.SingleOrDefault(s => s.Id == e.ModuleId);

                    Event @event = new Event
                    {
                        Start = e.Start,
                        End = e.End,
                        Repeatable = e.Repeatable,
                        BackgroundColor = e.BackgroundColor,
                        BackgroundColorDescriptive = e.BackgroundColorDescriptive,
                        BorderColor = e.BorderColor,
                        Room = e.Room,
                        Title = e.Title,
                        Module = module
                    };

                    return @event;
                })
                .ToArray();
        }

        protected void UpdateTimetableWithModifications(Timetable timetable, IEnumerable<EventViewModel> modifiedEvents, out bool shouldNotify)
        {
            shouldNotify = false;

            // modify existing ones with new data
            foreach (EventViewModel eventViewModel in modifiedEvents)
            {
                Event @event = this.Context.Events.SingleOrDefault(s => s.Id == eventViewModel.Id);

                // the only things you can change is the start, end dates plus the visuals which are the colours

                if (DateTime.Compare(@event.Start, eventViewModel.Start) != TimetablesController.EqualDateTimes)
                {
                    @event.Start = eventViewModel.Start;
                    shouldNotify = true;
                }

                if (DateTime.Compare(@event.End, eventViewModel.End) != TimetablesController.EqualDateTimes)
                {
                    @event.End = eventViewModel.End;
                    shouldNotify = true;
                }

                // these should be checked aswell but maybe EF is smart under the hood where identical values dont get pushed to databse as a change ?
                @event.BorderColor = eventViewModel.BorderColor;
                @event.BackgroundColor = eventViewModel.BackgroundColor;
                @event.BackgroundColorDescriptive = eventViewModel.BackgroundColorDescriptive;
            }
        }
    }
}