namespace CMS.Library.Caching
{
    using System;
    using NodaTime;

    public interface ICache<TState>
    {
        Instant LastAccessed { get; }

        Instant LastUpdated { get; }

        TState State { get; set; }
    }
}