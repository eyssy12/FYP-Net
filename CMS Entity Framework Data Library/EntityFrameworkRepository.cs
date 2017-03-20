namespace CMS.Entity.Framework.Data.Library
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using CMS.Library.Data;

    public class EntityFrameworkRepository<TDbContext, TEntity> : ISimpleRepository<TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> Table;

        public EntityFrameworkRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "context is missing");
            }

            this.Context = context;
            this.Table = this.Context.Set<TEntity>();
        }

        public IQueryable<TEntity> Get()
        {
            return this.Table.AsQueryable();
        }

        public void Add(TEntity entity)
        {
            this.Table.Add(entity);
            this.SaveChanges();
        }

        public void SaveChanges()
        {
            this.Context.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Context.Dispose();
            }
        }
    }
}