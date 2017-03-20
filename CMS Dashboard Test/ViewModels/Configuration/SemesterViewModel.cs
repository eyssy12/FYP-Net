namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SemesterViewModel
    {
        public int Id { get; set; }

        [Range(1, 16, ErrorMessage = "Number has to be bigger than 0")]
        public int Number { get; set; }

        public int CourseId { get; set; }

        public IEnumerable<ClassViewModel> Classes { get; set; } // done

        public IEnumerable<ModuleViewModel> Modules { get; set; } // done
    }
}