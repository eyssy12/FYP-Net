namespace CMS.Shared.Library.Models
{
    using System.Collections.Generic;

    public class Semester
    {
        public Semester()
        {
            this.Classes = new List<Class>();
            this.Modules = new List<Module>();
        }

        public int Id { get; set; }

        public int Number { get; set; }

        public int Credits { get; set; }

        public Course Course { get; set; }

        public IEnumerable<Class> Classes { get; set; }

        public IEnumerable<Module> Modules { get; set; }
    }
}