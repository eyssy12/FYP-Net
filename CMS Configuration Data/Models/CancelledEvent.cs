namespace CMS.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CancelledEvent")]
    public partial class CancelledEvent
    {
        public int Id { get; set; }

        public int? CancellationEventId { get; set; }

        public string CancelledBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Timestamp { get; set; }

        public virtual Event Event { get; set; }
    }
}
