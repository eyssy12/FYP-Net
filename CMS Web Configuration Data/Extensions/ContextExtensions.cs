namespace CMS.Web.Configuration.Data.Extensions
{
    using Microsoft.Data.Entity;

    public static class ContextExtensions
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}