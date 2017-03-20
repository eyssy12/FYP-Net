namespace CMS.Web.Configuration.Data.Models
{
    using System.Collections.Generic;
    using DegreeAward = Enumerations.DegreeAward;

    public class Course
    {
        public Course()
        {
            this.Semesters = new List<Semester>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DegreeAward AwardType { get; set; }

        public virtual Department Department { get; set; }

        public virtual ICollection<Semester> Semesters { get; set; }
    }
}