namespace CMS.Web.Configuration.Data.Models
{
    using System;

    public class NewsPost
    {
        public int Id { get; set; }

        public string PostedBy { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}