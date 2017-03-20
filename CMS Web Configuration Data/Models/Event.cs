namespace CMS.Web.Configuration.Data.Models
{
    using System;

    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Room { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool Repeatable { get; set; }

        public string BackgroundColorDescriptive { get; set; }

        public string BackgroundColor { get; set; }

        public string BorderColor { get; set; }

        public virtual Module Module { get; set; }

        public virtual Timetable Timetable { get; set; }
    }
}
