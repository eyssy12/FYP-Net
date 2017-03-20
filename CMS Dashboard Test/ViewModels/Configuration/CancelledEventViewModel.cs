namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CancelledEventViewModel
    {
        [Required]
        public string CancelledBy { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public int CancellationEventId { get; set; }
    }
}