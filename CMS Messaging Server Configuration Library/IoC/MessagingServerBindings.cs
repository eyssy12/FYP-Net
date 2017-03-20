namespace CMS.Messaging.Server.Configuration.Library.IoC
{
    using agsXMPP.net;
    using agsXMPP.sasl;
    using CMS.Configuration.Data;
    using CMS.Configuration.Data.Models;
    using CMS.Configuration.Data.Providers;
    using CMS.Configuration.Library.IoC;
    using CMS.Library.Data;
    using CMS.Library.Processing;
    using Entity.Framework.Data.Library;
    using Newtonsoft.Json.Serialization;
    using Server.Library.Actions;
    using Server.Library.Actions.Decisions;
    using Server.Library.Actions.Parsing;
    using Server.Library.Communications;
    using Server.Library.Models;
    using Server.Library.Processing;
    using Server.Library.Xmpp.Contract.Resolvers;
    using Server.Library.Xmpp.Mechanisms;
    using Shared.Library.Providers;

    public class MessagingServerBindings : CommonBindings
    {
        public const string
            UnderscoreBetweenMultiWordIdentifierPropertyContractResolver = "Underscore Between Multiple Word Identifier Property",
            TestCR = "testcr",
            ParseDataContentsAction = "Parse Data Contents",
            ParseMessageTypeAction = "Parse Message Type",
            ParseSenderTokenAction = "Parse Sender Token",
            RegisterClientAction = "Register Client",
            ReRegisterClientAction = "Re Register Client",
            CancelEventAction = "Cancel Event";

        public override void Load()
        {
            base.Load();

            // the order of the loading actions matter!
            this.BindActions();
            this.BindMechanisms();
            this.BindContractResolvers();
            this.BindProcessors();
            this.BindConnectionClients();
            this.BindProviders();
            this.BindRepositories();
        }

        protected virtual void BindProviders()
        {
            this.Bind<IGcmMobileClientProvider>().To<GcmMobileClientProvider>().InSingletonScope();
            this.Bind<IPersonProvider>().To<PersonProvider>().InSingletonScope();
            this.Bind<ITimetableEventProvider>().To<TimetableEventProvider>().InSingletonScope();
        }

        protected virtual void BindRepositories()
        {
            this.Bind<ISimpleRepository<Student>>().To<EntityFrameworkRepository<IdentityEntities, Student>>();
            this.Bind<ISimpleRepository<GcmMobileClient>>().To<EntityFrameworkRepository<IdentityEntities, GcmMobileClient>>();
            this.Bind<ISimpleRepository<Event>>().To<EntityFrameworkRepository<IdentityEntities, Event>>();
            this.Bind<ISimpleRepository<AspNetUser>>().To<EntityFrameworkRepository<IdentityEntities, AspNetUser>>();
            this.Bind<ISimpleRepository<Timetable>>().To<EntityFrameworkRepository<IdentityEntities, Timetable>>();
            this.Bind<ISimpleRepository<StudentPerson>>().To<EntityFrameworkRepository<IdentityEntities, StudentPerson>>();
            this.Bind<ISimpleRepository<CancelledEvent>>().To<EntityFrameworkRepository<IdentityEntities, CancelledEvent>>();
        }

        protected virtual void BindConnectionClients()
        {
            XmppConnectionSettings settings = new XmppConnectionSettings
            {
                UseSSL = true,
                UseStartTLS = false, // gcm ccs doesnt support this extension
                Server = "gcm.googleapis.com",
                ConnectServer = "gcm.googleapis.com",
                Port = 5235,
                Username = string.Format("{0}@{1}", 334770348820, "gcm.googleapis.com"),
                Password = "AIzaSyDF58nZS8k8KzA5lo4l2S4lViYFBhRq7jA",
                AutoResolveConnectServer = false,
                ConnectionType = SocketConnectionType.Direct,
                KeepAlive = true,
            };

            this.Bind<IXmppConnectionClient>()
                .To<XmppConnectionClient>()
                .InSingletonScope()
                .WithConstructorArgument("settings", settings);
        }

        protected virtual void BindProcessors()
        {
            this.Bind<IItemProcessor<GcmMessageStateHolder>>().To<GcmMessageProcessor>();
            this.Bind<IItemProcessor<ParsedGcmMessageStateHolder>>().To<GcmMessageDecisionProcessor>();
        }

        protected virtual void BindContractResolvers()
        {
            this.Bind<IContractResolver>()
                .To<UnderscoreBetweenMultiWordIdentifierPropertyContractResolver>()
                .Named(MessagingServerBindings.UnderscoreBetweenMultiWordIdentifierPropertyContractResolver);
        }

        protected virtual void BindMechanisms()
        {
            this.Bind<Mechanism>().To<GoogleXmppPlainMechanism>();
        }

        protected virtual void BindActions()
        {
            this.Bind<IAction<GcmMessageStateHolder>>().To<ParseDataContentsAction>().Named(MessagingServerBindings.ParseDataContentsAction);
            this.Bind<IAction<GcmMessageStateHolder>>().To<ParseMessageTypeAction>().Named(MessagingServerBindings.ParseMessageTypeAction);
            this.Bind<IAction<GcmMessageStateHolder>>().To<ParseSenderTokenAction>().Named(MessagingServerBindings.ParseSenderTokenAction);

            this.Bind<IAction<ParsedGcmMessageStateHolder>>().To<CancelEventAction>().Named(MessagingServerBindings.CancelEventAction);
            this.Bind<IAction<ParsedGcmMessageStateHolder>>().To<RegistrateMobileClientAction>().Named(MessagingServerBindings.RegisterClientAction);
            this.Bind<IAction<ParsedGcmMessageStateHolder>>().To<ReRegisterMobileClientAction>().Named(MessagingServerBindings.ReRegisterClientAction);
        }
    }
}