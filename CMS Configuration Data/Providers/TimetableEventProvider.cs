namespace CMS.Configuration.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Library.Extensions;
    using Library.Factories;
    using Shared.Library.Models;
    using Shared.Library.Providers;
    using TimetableEventData = CMS.Configuration.Data.Models.Event;
    using TimetableEventModel = CMS.Shared.Library.Models.TimetableEvent;

    public class TimetableEventProvider : ProviderBase<TimetableEventData>, ITimetableEventProvider
    {
        public TimetableEventProvider(IFactory factory)
            : base(factory)
        {
        }

        public void Create(TimetableEvent entity)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        public void Delete(TimetableEvent entity)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        public TimetableEvent Get(int id)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        public IEnumerable<TimetableEvent> GetAll()
        {
            return this.Invoke(repository =>
            {
                IEnumerable<TimetableEventData> events = repository.Get().ToArray(); // TODO: find out if ToArray removes the DbContext initialization issue 

                if (events.IsEmpty())
                {
                    return events.AsEmpty<TimetableEventData, TimetableEventModel>();
                }

                return events
                    .Select(e => this.ToEventModel(e))
                    .ToArray();
            });
        }

        public void Update(TimetableEvent entity)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        private TimetableEventModel ToEventModel(TimetableEventData @event)
        {
             // TODO: haven't included the module and timetable classes - might need to just do ToModels on them too ?

            return new TimetableEventModel
            {
                Id = @event.Id,
                BackgroundColor = @event.BackgroundColor,
                BackgroundColorDescriptive = @event.BackgroundColorDescriptive,
                BorderColor = @event.BorderColor,
                End = @event.End,
                Room = @event.Room,
                Start = @event.Start,
                Title = @event.Title,
                Repeatable = @event.Repeatable
            };
        }
    }
}