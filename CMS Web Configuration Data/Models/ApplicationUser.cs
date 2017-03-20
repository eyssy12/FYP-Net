namespace CMS.Web.Configuration.Data.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.NewsPosts = new List<NewsPost>();
        }

        public virtual Person Person { get; set; }

        public virtual ICollection<NewsPost> NewsPosts { get; set; }
    }
}