namespace CMS.Shared.Library.Models
{
    using System;
    using System.Collections.Generic;

    public class Lecturer : Person
    {
        public DateTime HireDate { get; set; }

        public IEnumerable<Module> Modules { get; set; }

        public IEnumerable<GcmMobileClient> MobileClients { get; set; }
    }
}