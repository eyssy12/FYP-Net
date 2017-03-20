namespace CMS.Web.Configuration.Data.Models
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

        public ICollection<Course> Courses { get; set; }
    }
}