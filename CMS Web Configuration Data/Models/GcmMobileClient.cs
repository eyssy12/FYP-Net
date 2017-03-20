namespace CMS.Web.Configuration.Data.Models
{
    public class GcmMobileClient
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public virtual Person Person { get; set; }
    }
}