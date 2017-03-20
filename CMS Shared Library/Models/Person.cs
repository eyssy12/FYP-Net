namespace CMS.Shared.Library.Models
{
    using System;

    public class Person
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public string MobilePhone { get; set; }
    }
}