namespace CMS.Dashboard.Test.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NewsPostViewModel
    {
        public int Id { get; set; }
        
        public string PostedBy { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime Timestamp { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
    }
}