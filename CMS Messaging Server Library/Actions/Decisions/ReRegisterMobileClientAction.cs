namespace CMS.Messaging.Server.Library.Actions.Decisions
{
    using System;
    using CMS.Library.Factories;
    using CMS.Messaging.Server.Library.Xmpp;
    using CMS.Shared.Library.Providers;
    using Models;
    using Shared.Library.Models;
    public class ReRegisterMobileClientAction : ActionBase, IAction<ParsedGcmMessageStateHolder>
    {
        protected readonly IPersonProvider PersonProvider;
        protected readonly IGcmMobileClientProvider MobileClientProvider;

        public ReRegisterMobileClientAction(IFactory factory, IPersonProvider personProvider, IGcmMobileClientProvider mobileClientProvider, string identifierKey = Protocol.ReRegistration)
            : base(factory, identifierKey)
        {
            if (personProvider == null)
            {
                throw new ArgumentNullException(nameof(personProvider), "");
            }

            if (mobileClientProvider == null)
            {
                throw new ArgumentNullException(nameof(mobileClientProvider), "");
            }

            this.PersonProvider = personProvider;
            this.MobileClientProvider = mobileClientProvider;
        }

        public void Run(ParsedGcmMessageStateHolder item)
        {
            if (item.Action == this.IdentifierKey)
            {
                Console.WriteLine("-> (" + DateTime.UtcNow.ToString() + ") - ReRegistration initiated by entity: " + item.EntityId);

                this.MobileClientProvider.Update(new GcmMobileClient
                {
                    Token = item.Value,
                    EntityId = item.EntityId
                });
            }
        }
    }
}