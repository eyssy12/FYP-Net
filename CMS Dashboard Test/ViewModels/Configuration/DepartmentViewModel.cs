namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Department name cannot be shorter than 6 chracters")]
        public string Name { get; set; }

        public IEnumerable<CourseViewModel> Courses { get; set; }
    }
}
