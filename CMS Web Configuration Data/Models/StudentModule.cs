namespace CMS.Web.Configuration.Data.Models
{
    public class StudentModule
    {
        public int StudentId { get; set; }

        public virtual Student Student { get; set; }

        public int ModuleId { get; set; }

        public virtual Module Module { get; set; }
    }
}