namespace CMS.Library.Caching
{
    using System;
    using System.Collections.Generic;
    using NodaTime;

    public class EntityCacheManager<TState> : CacheManagerBase<TState>
    {
        // TODO: how am i going to store the cache ? need a key that will not add duplicates to the memory

        protected readonly IReadOnlyDictionary<int, TState> states;

        public EntityCacheManager(IClock clock)
            : base(clock)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TState> Get()
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override bool Refresh()
        {
            throw new NotImplementedException();
        }
    }
}