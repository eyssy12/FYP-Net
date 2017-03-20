namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class LecturerViewModel
    {
        public int Id { get; set; }

        public DateTime HireDate { get; set; }

        [Required]
        public int? PersonId { get; set; }
    }
}