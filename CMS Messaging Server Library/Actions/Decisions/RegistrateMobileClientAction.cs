namespace CMS.Messaging.Server.Library.Actions.Decisions
{
    using System;
    using System.Linq;
    using CMS.Library.Extensions;
    using CMS.Library.Factories;
    using Models;
    using Shared.Library.Models;
    using Shared.Library.Providers;
    using Xmpp;

    public class RegistrateMobileClientAction : ActionBase, IAction<ParsedGcmMessageStateHolder>
    {
        protected readonly IGcmMobileClientProvider Provider;

        public RegistrateMobileClientAction(IFactory factory, IGcmMobileClientProvider provider, string identifierKey = Protocol.Registration)
            : base(factory, identifierKey)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider), "");
            }

            this.Provider = provider;
        }

        public void Run(ParsedGcmMessageStateHolder item)
        {
            if (item.Action == this.IdentifierKey)
            {
                Console.WriteLine("-> (" + DateTime.UtcNow.ToString() + ") - Registration initiated by entity: " + item.EntityId);

                if (this.Provider
                    .GetAll()
                    .Where(p => p.Token == item.Value)
                    .IsEmpty())
                {
                    this.Provider.Create(new GcmMobileClient
                    {
                        Token = item.Value,
                        EntityId = item.EntityId
                    });
                }
            }
        }
    }
}