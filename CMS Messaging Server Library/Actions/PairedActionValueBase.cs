namespace CMS.Messaging.Server.Library.Actions
{
    using System;
    using CMS.Library.Factories;

    public abstract class PairedActionValueBase : ActionBase 
    {
        protected readonly string ActionKey,
            ValueKey;

        protected PairedActionValueBase(IFactory factory, string rootKey, string actionKey, string valueKey)
            : base(factory, rootKey)
        {
            if (string.IsNullOrWhiteSpace(actionKey))
            {
                throw new ArgumentNullException(nameof(actionKey), "An identifier has not been provided"); //TODO: add to resources
            }

            if (string.IsNullOrWhiteSpace(valueKey))
            {
                throw new ArgumentNullException(nameof(valueKey), "An identifier has not been provided"); //TODO: add to resources
            }

            this.ActionKey = actionKey;
            this.ValueKey = valueKey;
        }
    }
}