namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class StudentViewModel
    {
        public int Id { get; set; }

        public DateTime EnrollmentDate { get; set; }

        [Required]
        public int? PersonId { get; set; }

        [Required]
        public int? ClassId { get; set; }
    }
}