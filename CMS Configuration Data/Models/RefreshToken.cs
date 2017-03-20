namespace CMS.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RefreshToken")]
    public partial class RefreshToken
    {
        public int Id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ExpiryDate { get; set; }

        public string Token { get; set; }

        [StringLength(450)]
        public string UserId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
