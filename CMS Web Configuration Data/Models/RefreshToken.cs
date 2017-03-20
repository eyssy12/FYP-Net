namespace CMS.Web.Configuration.Data.Models
{
    using System;

    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public DateTime ExpiryDate { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}