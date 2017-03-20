namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PersonViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Persons First Name cannot be shorter than 2 chracters")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Persons Second Name cannot be shorter than 6 chracters")]
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string MobilePhone { get; set; }

        public string Address { get; set; }
    }
}