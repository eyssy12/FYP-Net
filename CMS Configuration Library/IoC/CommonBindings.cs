namespace CMS.Configuration.Library.IoC
{
    using System.Data.Entity;
    using CMS.Library.Factories;
    using Data;
    using Ninject.Extensions.Factory;
    using Ninject.Modules;
    using NodaTime;

    public abstract class CommonBindings : NinjectModule
    {
        public override void Load()
        {
            this.BindFactories();
            this.BindDatabase();
            this.BindMisc();
        }

        protected virtual void BindDatabase()
        {
            this.Bind<DbContext, IdentityEntities>().To<IdentityEntities>();
        }

        protected virtual void BindMisc()
        {
            this.Bind<IClock>().To<SystemClock>();
        }

        protected virtual void BindFactories()
        {
            this.Bind<IFactory>().ToFactory().InSingletonScope();
        }
    }
}