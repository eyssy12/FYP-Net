namespace CMS.Shared.Library.Providers
{
    using System.Collections.Generic;
    
    public interface IEntityProvider<TEntity>
        where TEntity : class
    {
        TEntity Get(int id);

        IEnumerable<TEntity> GetAll();

        void Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}