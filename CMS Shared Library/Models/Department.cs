namespace CMS.Shared.Library.Models
{
    using System.Collections.Generic;

    public class Department
    {
        public Department()
        {
            this.Courses = new List<Course>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}