namespace CMS.Shared.Library.Models
{
    using System.Collections.Generic;

    public class Address
    {
        public int Id { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Address4 { get; set; }

        public IEnumerable<Person> Persons { get; set; }
    }
}