namespace CMS.Web.Configuration.Data.Models
{
    using System.Collections.Generic;

    public class Timetable
    {
        public Timetable()
        {
            this.Events = new List<Event>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual Class Class { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}