namespace CMS.Web.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using EnrollmentStage = Enumerations.EnrollmentStage;

    public class Class
    {
        public Class()
        {
            this.Students = new List<Student>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime YearCommenced { get; set; }

        public EnrollmentStage EnrollmentStage { get; set; }

        public virtual Timetable Timetable { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}