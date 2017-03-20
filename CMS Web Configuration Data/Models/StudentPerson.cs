namespace CMS.Web.Configuration.Data.Models
{
    public class StudentPerson
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public virtual Person Person { get; set; }

        public int StudentId { get; set; }

        public virtual Student Student  {get; set; }
    }
}