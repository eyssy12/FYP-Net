namespace CMS.Web.Configuration.Data.Models
{
    public class LecturerModule
    {
        public int Id { get; set; }

        public int LecturerId { get; set; }

        public virtual Lecturer Lecturer { get; set; }

        public int ModuleId { get; set; }
        
        public virtual Module Module { get; set; }
    }
}