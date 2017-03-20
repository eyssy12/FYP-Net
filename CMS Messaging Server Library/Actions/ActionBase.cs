namespace CMS.Messaging.Server.Library.Actions
{
    using System;
    using CMS.Library.Factories;
    public abstract class ActionBase
    {
        protected readonly IFactory Factory;
        protected readonly string IdentifierKey;

        protected ActionBase(IFactory factory, string identifierKey)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory), "no repository has been provided");
            }

            if (string.IsNullOrWhiteSpace(identifierKey))
            {
                throw new ArgumentNullException(nameof(identifierKey), "No identifier for an action ahs been provided"); // TODO: add to resources
            }

            this.Factory = factory;
            this.IdentifierKey = identifierKey;
        }
    }
}