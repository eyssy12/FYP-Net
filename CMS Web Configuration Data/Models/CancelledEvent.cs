namespace CMS.Web.Configuration.Data.Models
{
    using System;

    public class CancelledEvent
    {
        public int Id { get; set; }

        public string CancelledBy { get; set; }
        
        public DateTime Timestamp { get; set; }

        public virtual Event CancellationEvent { get; set; }
    }
}