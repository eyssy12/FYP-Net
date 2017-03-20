namespace CMS.Library.Data
{
    using System;
    using System.Linq;

    public interface ISimpleRepository<TEntity> : IDisposable
        where TEntity : class
    {
        IQueryable<TEntity> Get();

        void Add(TEntity entity);

        void SaveChanges();
    }
}