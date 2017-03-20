namespace CMS.Configuration.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using CMS.Shared.Library.Providers;
    using Extensions;
    using Library.Extensions;
    using Library.Factories;
    using GcmMobileClientData = CMS.Configuration.Data.Models.GcmMobileClient;
    using GcmMobileClientModel = CMS.Shared.Library.Models.GcmMobileClient;
    using PersonData = CMS.Configuration.Data.Models.Person;

    public class GcmMobileClientProvider : ProviderBase<GcmMobileClientData>, IGcmMobileClientProvider
    {
        protected readonly IdentityEntities Entities;

        public GcmMobileClientProvider(IFactory factory, IdentityEntities entities)
            : base(factory)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "");
            }

            this.Entities = entities;
        }

        public void Create(GcmMobileClientModel entity)
        {
            IEnumerable<PersonData> persons = this.Entities.People.ToArray();
            PersonData person = persons.SingleOrDefault(s => s.ApplicationUserId == entity.EntityId);

            // apparently hits the database once according to georges presentation
            GcmMobileClientData mobileClient = new GcmMobileClientData
            {
                Token = entity.Token,
                Person = person
            };

            try
            {
                this.Entities.GcmMobileClients.Add(mobileClient);
                this.Entities.SaveChanges();
            }
            catch (DbException ex)
            {

            }
        }

        public void Delete(GcmMobileClientModel entity)
        {
            throw new InvalidOperationException("Not supported in FYPCMS v1.0");
        }

        public GcmMobileClientModel Get(int id)
        {
            return this.Invoke(repository =>
            {
                return repository
                    .Get()
                    .ToArray()
                    .Single(e => e.Id == id)
                    .ToApplicationModel();
            });
        }

        public IEnumerable<GcmMobileClientModel> GetAll()
        {
            return this.Invoke(repository =>
            {
                IEnumerable<GcmMobileClientData> data = repository.Get().ToArray(); // TODO: find out if ToArray removes the DbContext initialization issue 

                if (data.IsEmpty())
                {
                    return data.AsEmpty<GcmMobileClientData, GcmMobileClientModel>();
                }

                return data
                    .Select(e => e.ToApplicationModel())
                    .ToArray();
            });
        }

        public void Update(GcmMobileClientModel entity)
        {
            this.Invoke(entities =>
            {
                // TODO: check if using .Find() -> {on a List} will increase performance - it checks for in memory cache first before sending a query if contents dont exist
                GcmMobileClientData current = entities.GcmMobileClients.SingleOrDefault(s => s.Token == entity.Token);

                PersonData newPerson = entities.People.SingleOrDefault(s => s.ApplicationUserId == entity.EntityId);

                current.Person = newPerson;
                current.Token = entity.Token;
            });
        }
    }
}