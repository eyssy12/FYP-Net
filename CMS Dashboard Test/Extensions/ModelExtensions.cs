namespace CMS.Dashboard.Test.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Dashboard.Test.ViewModels.Configuration;
    using CMS.Web.Configuration.Data.Models;

    public static class ModelExtensions
    {
        public static CancelledEvent ToModel(this CancelledEventViewModel cancelledEvent, IEnumerable<Event> allEvents)
        {
            Event @event = allEvents.Single(s => s.Id == cancelledEvent.CancellationEventId);

            return new CancelledEvent
            {
                CancellationEvent = @event,
                CancelledBy = cancelledEvent.CancelledBy,
                Timestamp = cancelledEvent.Timestamp
            };
        }

        public static IEnumerable<CancelledEvent> ToModels(this IEnumerable<CancelledEventViewModel> cancelledEvents, IEnumerable<Event> allEvents)
        {
            return cancelledEvents.Select(s => s.ToModel(allEvents));
        }
    }
}