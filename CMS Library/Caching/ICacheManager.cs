namespace CMS.Library.Caching
{
    using System.Collections.Generic;

    public interface ICacheManager<TState>
    {
        void Initialize();

        bool Refresh();

        void Clear();

        IEnumerable<TState> Get();
    }
}