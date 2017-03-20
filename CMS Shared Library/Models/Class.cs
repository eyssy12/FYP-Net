namespace CMS.Shared.Library.Models
{
    using System;
    using System.Collections.Generic;
    using EnrollmentStage = Shared.Library.Enumerations.EnrollmentStage;

    public class Class
    {
        public Class()
        {
            this.Students = new List<Student>();
        }

        public int Id { get; set; }

        public DateTime YearCommenced { get; set; }

        public EnrollmentStage EnrollmentStage { get; set; }

        public IEnumerable<Student> Students { get; set; }

        public Semester Semester { get; set; }
    }
}