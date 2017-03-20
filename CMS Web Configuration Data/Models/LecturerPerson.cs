namespace CMS.Web.Configuration.Data.Models
{
    public class LecturerPerson
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public virtual Person Person { get; set; }

        public int LecturerId { get; set; }

        public virtual Lecturer Lecturer { get; set; }
    }
}