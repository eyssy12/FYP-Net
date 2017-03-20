namespace CMS.Library.Caching
{
    using System;
    using System.Collections.Generic;
    using NodaTime;

    public abstract class CacheManagerBase<TState> : ICacheManager<TState>
    {
        protected readonly IClock Clock;

        protected CacheManagerBase(IClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock), "blah");
            }

            this.Clock = clock;
        }

        public abstract void Initialize();

        public abstract bool Refresh();

        public abstract void Clear();

        public abstract IEnumerable<TState> Get();

        protected virtual ICache<TState> CreateCache(TState value)
        {
            return new Cache<TState>(value, this.Clock);
        }
    }
}