namespace CMS.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NewsPost")]
    public partial class NewsPost
    {
        public int Id { get; set; }

        [StringLength(450)]
        public string ApplicationUserId { get; set; }

        public string Body { get; set; }

        public string PostedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Timestamp { get; set; }

        public string Title { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
