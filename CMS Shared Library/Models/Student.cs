namespace CMS.Shared.Library.Models
{
    using System;
    using System.Collections.Generic;

    public class Student : Person
    {
        public DateTime EnrollmentDate { get; set; }

        public Class Class { get; set; }

        public IEnumerable<Module> Modules { get; set; }

        public IEnumerable<GcmMobileClient> MobileClients { get; set; } // TODO: this will allow for me to find the affected students if an event is cancelled
    }
}