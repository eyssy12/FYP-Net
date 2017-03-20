namespace CMS.Dashboard.Test.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class IdArrayViewModel
    {
        [Required]
        public IEnumerable<int> Ids { get; set; }
    }
}