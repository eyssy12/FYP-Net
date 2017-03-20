namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AwardType = CMS.Web.Configuration.Data.Enumerations.DegreeAward;

    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Course Name cannot be shorter than 6 chracters")]
        public string Name { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Course Code cannot be shorter than 6 chracters")]
        public string Code { get; set; }

        [Required]
        public AwardType AwardType { get; set; }

        public int DepartmentId { get; set; }

        public IEnumerable<SemesterViewModel> Semesters { get; set; }
    }
}
