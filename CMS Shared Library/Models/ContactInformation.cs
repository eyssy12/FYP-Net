namespace CMS.Shared.Library.Models
{
    public class ContactInformation
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string HousePhone { get; set; }

        public string MobilePhone { get; set; }

        public Person Person { get; set; }
    }
}