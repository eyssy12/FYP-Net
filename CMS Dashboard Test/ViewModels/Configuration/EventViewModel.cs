namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EventViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Room { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool Repeatable { get; set; }

        public string BackgroundColorDescriptive { get; set; }

        public string BackgroundColor { get; set; }

        public string BorderColor { get; set; }

        [Required]
        public int? ModuleId { get; set; }

        [Required]
        public int? TimetableId { get; set; }
    }
}
