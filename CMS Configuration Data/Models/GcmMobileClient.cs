namespace CMS.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GcmMobileClient")]
    public partial class GcmMobileClient
    {
        public int Id { get; set; }

        public int? PersonId { get; set; }

        [Required]
        [StringLength(200)]
        public string Token { get; set; }

        public virtual Person Person { get; set; }
    }
}
