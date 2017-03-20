namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using UserRole = CMS.Web.Configuration.Data.Enumerations.UserRole;

    public class ApplicationUserViewModel
    {
        public string Id { get; set; }

        public string Password { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Users username cannot be shorter than 6 characters")]
        public string UserName { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Users email be shorter than 6 characters")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Range(1, int.MaxValue)]
        public UserRole UserRole { get; set; }

        public int PersonId { get; set; }
    }
}