namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ModifedTimetableViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Timetable name cannot be shorter than 6 characters")]
        public string Name { get; set; }

        public IEnumerable<EventViewModel> NewEvents { get; set; }

        public IEnumerable<EventViewModel> ModifiedEvents { get; set; }

        public IEnumerable<int> EventIdsToRemove { get; set; }
    }
}