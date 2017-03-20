namespace CMS.Library.Caching
{
    using System;
    using NodaTime;

    public class Cache<TState> : ICache<TState>
    {
        protected readonly IClock Clock;

        private TState state;

        public Cache(TState state, IClock clock)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state), "state missing");
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock), "blah");
            }

            this.state = state;
            this.Clock = clock;
        }

        public Instant LastAccessed { get; protected set; }

        public Instant LastUpdated { get; protected set; }

        public virtual TState State
        {
            get
            {
                this.LastAccessed = this.Clock.Now;

                return this.state;
            }

            set
            {
                this.LastUpdated = this.Clock.Now;

                this.state = value;
            }
        }
    }
}