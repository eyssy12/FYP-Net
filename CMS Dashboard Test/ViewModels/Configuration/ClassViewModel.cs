namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EnrollmentStage = CMS.Web.Configuration.Data.Enumerations.EnrollmentStage;

    public class ClassViewModel
    {
        public ClassViewModel()
        {
            this.Students = new List<StudentViewModel>();
        }

        public int Id { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Class Name cannot be shorter than 6 characters")]
        public string Name { get; set; }

        public DateTime YearCommenced { get; set; }

        public EnrollmentStage EnrollmentStage { get; set; }

        public int? CourseId { get; set; }

        public int? TimetableId { get; set; }

        [Required]
        public int? SemesterId { get; set; } // done

        public IEnumerable<StudentViewModel> Students { get; set; } // done
    }
}
