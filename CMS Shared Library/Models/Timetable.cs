namespace CMS.Shared.Library.Models
{
    using System.Collections.Generic;

    public class Timetable
    {
        public Timetable()
        {
            this.Events = new List<TimetableEvent>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Class Class { get; set; }

        public IEnumerable<TimetableEvent> Events { get; set; }
    }
}