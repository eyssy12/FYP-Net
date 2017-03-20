namespace TestApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Library.Bases;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using TestApi.Serialization;

    [Route(TimetableController.DefaultApiControllerRoute)]
    public class TimetableController : SecureControllerBase
    {
        protected const string TimetableForStudentTemplate = "GetTimetableForStudent/" + TimetableController.ApiRouteIdSegment,
            TimetableForLecturerTemplate = "GetTimetableForLecturer/" + TimetableController.ApiRouteIdSegment,
            CancelledEventsForStudentTemplate = "GetCancelledEventsForStudent/" + TimetableController.ApiRouteIdSegment,
            CancelledEventByIdTemplate = "GetCancelledEventById/" + TimetableController.ApiRouteIdSegment;

        public TimetableController(Entities context, MyJsonSerializerSettings settings)
            : base (context, settings)
        {
        }

        [Route(TimetableController.TimetableForStudentTemplate)]
        public IActionResult GetTimetableForStudent(int id)
        {
            IEnumerable<Timetable> timetables = this.Context.Timetables
                .Include(s => s.Class)
                    .ThenInclude(s => s.Students)
                .Include(s => s.Events)
                .ToArray();

            Timetable timetable = timetables.SingleOrDefault(s => s.Class.Students.Any(e => e.Id == id));

            if (timetable == null)
            {
                // will return an empty array
                return Json(new { });
            }

            return Json(timetable, this.Settings);
        }

        [Route(TimetableController.TimetableForLecturerTemplate)]
        public IActionResult GetTimetableForLecturer(int id)
        {
            // Since there is no direct timetable created for a lecturer, their timetable will consist of events from students timetables
            IEnumerable<Event> events = this.Context.Events
                .Include(s => s.Module)
                    .ThenInclude(s => s.LecturerModules)
                .ToArray()
                .Where(s => s.Module.LecturerModules.Any(e => e.LecturerId == id))
                .ToArray();

            return Json(events, this.Settings);
        }

        [Route(TimetableController.CancelledEventsForStudentTemplate)]
        public IActionResult GetCancelledEventsForStudent(int id)
        {
            IEnumerable<CancelledEvent> cancellations = this.Context.CancelledEvents
                .Include(s => s.CancellationEvent)
                    .ThenInclude(s => s.Timetable)
                        .ThenInclude(s => s.Class)
                            .ThenInclude(s => s.Students)
                .Take(20) // we only want to take the top 20 because there can be a lot of these cancellations, the mobile app only needs the top 20 for history at most
                .ToArray();

            // Do a cleanup operation for cancellations that may be obsolete
            IEnumerable<CancelledEvent> toRemove = cancellations.Where(s => s.CancellationEvent == null).ToArray();
            if (toRemove.Any())
            {
                this.Context.CancelledEvents.RemoveRange(toRemove);
                this.Context.SaveChanges();
            }

            cancellations = cancellations
                .Where(s => s.CancellationEvent != null && s.CancellationEvent // if a timetable is removed or an event itself, then the cancellation event would be null
                    .Timetable
                    .Class
                    .Students
                    .Any(e => e.Id == id))
                .ToArray()
                .Select(cancellation =>
                {
                    // dereference redundant data
                    cancellation.CancellationEvent.Timetable = null;
                    return cancellation;
                });

            return Json(cancellations.Select(s => this.TrimCancelledEvent(s)), this.Settings);
        }

        [Route(TimetableController.CancelledEventByIdTemplate)]
        public IActionResult CancelledEventById(int id)
        {
            CancelledEvent cancelledEvent = this.Context.CancelledEvents
                .Include(s => s.CancellationEvent)
                .SingleOrDefault(s => s.Id == id);

            if (cancelledEvent == null)
            {
                return HttpNotFound();
            }

            return Json(this.TrimCancelledEvent(cancelledEvent), this.Settings);
        }

        protected dynamic TrimCancelledEvent(CancelledEvent cancelledEvent)
        {
            return new
            {
                CancelledBy = cancelledEvent.CancelledBy,
                Timestamp = cancelledEvent.Timestamp,
                CancelledEventId = cancelledEvent.CancellationEvent.Id,
                CancelledEventTitle = cancelledEvent.CancellationEvent.Title,
                CancelledEventStart = cancelledEvent.CancellationEvent.Start,
                CancelledEventEnd = cancelledEvent.CancellationEvent.End
            };
        }
    }
}