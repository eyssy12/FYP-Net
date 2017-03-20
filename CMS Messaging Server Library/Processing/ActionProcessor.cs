namespace CMS.Messaging.Server.Library.Processing
{
    using System;
    using System.Collections.Generic;
    using Actions;
    using CMS.Library.Extensions;
    using CMS.Library.Processing;

    public abstract class ActionProcessor<T> : IItemProcessor<T>
    {
        protected readonly IEnumerable<IAction<T>> Actions;

        protected ActionProcessor(IEnumerable<IAction<T>> actions)
        {
            if (actions.IsEmpty())
            {
                throw new ArgumentException("Actions have not been provided - would not be able to process an event.", nameof(actions)); // TODO: add to resources
            }

            this.Actions = actions;
        }

        public abstract void Process(T item);
    }
}