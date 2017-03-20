namespace CMS.Service
{
    using agsXMPP.Factory;
    using CMS.Library.IoC;
    using CMS.Processing.Messaging;
    using Messaging.Server.Configuration.Library.IoC;
    using Messaging.Server.Library.Xmpp.Mechanisms;

    public class Program
    {
        public static void Main(string[] args)
        {
            SaslFactory.AddMechanism(GoogleXmppPlainMechanism.MechanismName, GoogleXmppPlainMechanism.Type);

            NinjectContainer container = new NinjectContainer(new MessagingServerBindings());

            MessageProcessor processor = container.ResolveAutomatically<MessageProcessor>();
            processor.Start();
        }
    }
}