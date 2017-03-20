namespace CMS.Web.Configuration.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Person
    {
        public Person()
        {
            this.MobileClients = new List<GcmMobileClient>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public string MobilePhone { get; set; }

        [JsonIgnore] // we dont want to return any data back regarding the app user because it contains sensitive account info that should not be exposed
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<GcmMobileClient> MobileClients { get; set; }
    }
}