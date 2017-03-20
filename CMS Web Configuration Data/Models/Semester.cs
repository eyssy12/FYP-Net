namespace CMS.Web.Configuration.Data.Models
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

        public virtual Course Course { get; set; }

        public virtual ICollection<Class> Classes { get; set; }

        public virtual ICollection<Module> Modules { get; set; }
    }
}