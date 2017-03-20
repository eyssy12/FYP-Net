namespace CMS.Configuration.Data.Providers
{
    using System;
    using CMS.Library.Data;
    using CMS.Library.Factories;

    public abstract class ProviderBase<TEntity>
        where TEntity : class
    {
        protected readonly IFactory Factory;

        protected ProviderBase(IFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory), "factory is missing");
            }

            this.Factory = factory;
        }

        protected ISimpleRepository<TEntity> CreateRepository()
        {
            return this.Factory.Create<ISimpleRepository<TEntity>>();
        }

        protected IdentityEntities CreateEntities()
        {
            return this.Factory.Create<IdentityEntities>();
        }

        protected virtual void Invoke(Action<IdentityEntities> invoke)
        {
            using (IdentityEntities data = this.CreateEntities())
            {
                invoke(data);
                data.SaveChanges();
            }
        }

        protected virtual TResult Invoke<TResult>(Func<ISimpleRepository<TEntity>, TResult> invoke)
        {
            using (ISimpleRepository<TEntity> repository = this.CreateRepository())
            {
                return invoke(repository);
            }
        }
    }
}