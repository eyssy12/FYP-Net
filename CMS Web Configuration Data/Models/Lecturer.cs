namespace CMS.Web.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Lecturer
    {
        public Lecturer()
        {
            this.LecturerModules = new List<LecturerModule>();
        }

        public int Id { get; set; }

        public DateTime HireDate { get; set; }

        public virtual LecturerPerson LecturerPerson { get; set; }

        public virtual ICollection<LecturerModule> LecturerModules { get; set; } // done
    }
}