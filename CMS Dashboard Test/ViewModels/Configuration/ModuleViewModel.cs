namespace CMS.Dashboard.Test.ViewModels.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using ModuleType = Web.Configuration.Data.Enumerations.ModuleType;

    public class ModuleViewModel
    {
        public int Id { get; set; }

        [Range(20, Int32.MaxValue, ErrorMessage = "Number has to be bigger than 20")]
        public int Credits { get; set; }

        [Required]
        [MinLength(6)]
        public string Name { get; set; }

        public ModuleType ModuleType { get; set; }

        public int CourseId { get; set; }

        public int SemesterId { get; set; } 

        public IEnumerable<int> LecturerIds { get; set; }
    }
}
