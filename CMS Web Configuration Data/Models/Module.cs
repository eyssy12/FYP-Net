namespace CMS.Web.Configuration.Data.Models
{
    using System.Collections.Generic;
    using ModuleType = Enumerations.ModuleType;

    public class Module
    {
        public Module()
        {
            this.LecturerModules = new List<LecturerModule>();
            this.StudentModules = new List<StudentModule>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Credits { get; set; }

        public ModuleType ModuleType { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual ICollection<LecturerModule> LecturerModules { get; set; }

        public virtual ICollection<StudentModule> StudentModules { get; set; }
    }
}