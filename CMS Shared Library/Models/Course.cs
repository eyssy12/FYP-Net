namespace CMS.Shared.Library.Models
{
    using System.Collections.Generic;
    using DegreeAward = Shared.Library.Enumerations.DegreeAward;

    public class Course
    {
        public Course()
        {
            this.Semesters = new List<Semester>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DegreeAward AwardType { get; set; }

        public Department Department { get; set; }

        public IEnumerable<Semester> Semesters { get; set; }
    }
}