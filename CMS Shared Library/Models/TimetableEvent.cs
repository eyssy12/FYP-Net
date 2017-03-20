namespace CMS.Shared.Library.Models
{
    using System;

    public class TimetableEvent
    {
        public int Id { get; set; }

        public string BackgroundColor { get; set; }

        public string BackgroundColorDescriptive { get; set; }

        public string BorderColor { get; set; }

        public string Room { get; set; }

        public DateTime End { get; set; }

        public DateTime Start { get; set; }

        public string Title { get; set; }

        public bool Repeatable { get; set; }

        public Module Module { get; set; }

        public Timetable Timetable { get; set; }
    }
}