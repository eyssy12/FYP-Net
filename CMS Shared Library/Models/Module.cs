namespace CMS.Shared.Library.Models
{
    using System.Collections.Generic;

    public class Module
    {
        public Module()
        {
            this.Lecturers = new List<Lecturer>();
            this.Students = new List<Student>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Lecturer> Lecturers { get; set; }

        public IEnumerable<Student> Students { get; set; }
    }
}