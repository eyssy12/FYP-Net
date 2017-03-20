namespace CMS.Web.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Student
    {
        public Student()
        {
            this.StudentModules = new List<StudentModule>();
        }

        public int Id { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public virtual StudentPerson StudentPerson { get; set; }

        public virtual Class Class { get; set; }

        public virtual ICollection<StudentModule> StudentModules { get; set; }
    }
}