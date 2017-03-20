using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    public class TimetableViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Timetable name cannot be shorter than 6 chracters")]
        public string Name { get; set; }

        public int ClassId { get; set; }

        public IEnumerable<EventViewModel> Events { get; set; }
    }
}