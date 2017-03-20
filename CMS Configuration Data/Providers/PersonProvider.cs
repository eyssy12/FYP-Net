namespace CMS.Configuration.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Library.Extensions;
    using Library.Factories;
    using Shared.Library.Providers;
    using PersonData = CMS.Configuration.Data.Models.Person;
    using PersonModel = CMS.Shared.Library.Models.Person;

    public class PersonProvider : ProviderBase<PersonData>, IPersonProvider
    {
        public PersonProvider(IFactory factory)
            : base(factory)
        {
        }

        public void Create(PersonModel entity)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        public void Delete(PersonModel entity)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        public PersonModel Get(int id)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        public IEnumerable<PersonModel> GetAll()
        {
            return this.Invoke(repository =>
            {
                IEnumerable<PersonData> data = repository.Get().ToArray(); // TODO: find out if ToArray removes the DbContext initialization issue 

                if (data.IsEmpty())
                {
                    return data.AsEmpty<PersonData, PersonModel>();
                }

                return data
                    .Select(e => this.ToPersonModel(e))
                    .ToArray();
            });
        }

        public void Update(PersonModel entity)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        private PersonModel ToPersonModel(PersonData person)
        {
            PersonModel personModel = new PersonModel
            {
                Id = person.Id,
                ApplicationUserId = person.ApplicationUserId,
                Address = person.Address,
                BirthDate = person.BirthDate,
                FirstName = person.FirstName,
                LastName = person.LastName,
                MobilePhone = person.MobilePhone
            };

            return personModel;
        }
    }
}