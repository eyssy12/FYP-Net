namespace CMS.Processing.Messaging
{
    using System;
    using agsXMPP;
    using agsXMPP.sasl;
    using agsXMPP.Xml.Dom;
    using CMS.Library.Processing;
    using CMS.Messaging.Server.Library.Communications;
    using CMS.Messaging.Server.Library.Models;
    using CMS.Messaging.Server.Library.Xmpp.Mechanisms;
    using AgsMessage = agsXMPP.protocol.client.Message;

    public class MessageProcessor
    {
        public const string ProjectID = "fyp-cms-college",
            SenderID = "334770348820",
            Server = "gcm.googleapis.com",
            EndpointPreProd = "gcm-preprod.googleapis.com",
            EndpointProduction = "gcm-xmpp.googleapis.com",
            ApiKey = "AIzaSyDF58nZS8k8KzA5lo4l2S4lViYFBhRq7jA",
            Category = "com.eyssyapps.fyp-cms";

        public const string EndpointFormat = "{0}:{1}",
            ClientUsernameFormat = "{0}@{1}";

        public const int EndpointPreProdPort = 5236,
            EndpointProductionPort = 5235;

        protected readonly IItemProcessor<GcmMessageStateHolder> ParsingProcessor;
        protected readonly IItemProcessor<ParsedGcmMessageStateHolder> DecisionProcessor;
        protected readonly IXmppConnectionClient Client;

        public MessageProcessor(
            IXmppConnectionClient client,
            IItemProcessor<GcmMessageStateHolder> parsingProcessor,
            IItemProcessor<ParsedGcmMessageStateHolder> decisionProcessor)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "a connection client is missing");
            }

            if (parsingProcessor == null)
            {
                throw new ArgumentNullException(nameof(parsingProcessor), "an item parsing processor is missing");
            }

            if (decisionProcessor == null)
            {
                throw new ArgumentNullException(nameof(decisionProcessor), "an item decision processor is missing");
            }

            this.Client = client;
            this.ParsingProcessor = parsingProcessor;
            this.DecisionProcessor = decisionProcessor;
        }

        public void Start()
        {
            this.Client.OnSaslStart += this.Client_OnSaslStart;
            this.Client.OnXmppConnectionStateChanged += this.Client_OnXmppConnectionStateChanged;
            this.Client.OnError += this.Client_OnError;
            this.Client.OnLogin += this.Client_OnLogin;
            this.Client.OnAuthError += this.Client_OnAuthError;
            this.Client.OnMessage += this.Client_OnMessage;
            this.Client.OnSocketError += this.Client_OnSocketError;
            this.Client.OnClose += this.Client_OnClose;
            this.Client.OnWriteSocketData += this.Client_OnWriteSocketData;

            this.Client.Connect();

            // TODO:shouldnt rely on this, this should become a windows service in the long run so that the runtime is persistant
            string kb;
            do
            {
                Console.Write("cmd: ");
                kb = Console.ReadLine();

            } while (kb != "stop");
        }

        public void Close()
        {
            this.Client.Close();

            this.Client.OnSaslStart -= this.Client_OnSaslStart;
            this.Client.OnXmppConnectionStateChanged -= this.Client_OnXmppConnectionStateChanged;
            this.Client.OnError -= this.Client_OnError;
            this.Client.OnLogin -= this.Client_OnLogin;
            this.Client.OnAuthError -= this.Client_OnAuthError;
            this.Client.OnMessage -= this.Client_OnMessage;
            this.Client.OnSocketError -= this.Client_OnSocketError;
            this.Client.OnClose -= this.Client_OnClose;
            this.Client.OnWriteSocketData -= this.Client_OnWriteSocketData;
        }

        private void PopulateMessageStateHolder(AgsMessage message)
        {
            GcmMessageStateHolder stateHolder = new GcmMessageStateHolder(message);

            this.ParsingProcessor.Process(stateHolder);

            ParsedGcmMessageStateHolder parsedStateHolder = new ParsedGcmMessageStateHolder(stateHolder);

            this.DecisionProcessor.Process(parsedStateHolder);

            Console.WriteLine();
            Console.WriteLine("-> (" + DateTime.UtcNow.ToString() + ")");
            Console.WriteLine("Type: " + stateHolder.MessageType ?? "default");
            Console.WriteLine();
            Console.WriteLine("Finished processing the message from => " + stateHolder.From);
            Console.WriteLine("------------------");
            Console.WriteLine();
        }

        private void Client_OnSaslStart(object sender, SaslEventArgs args)
        {
            args.Auto = false;
            args.Mechanism = GoogleXmppPlainMechanism.MechanismName;
        }

        private void Client_OnSocketError(object sender, Exception ex)
        {
            Console.WriteLine("SOCKET ERR: " + ex.Message);
        }

        private void Client_OnMessage(object sender, AgsMessage msg)
        {
            this.PopulateMessageStateHolder(msg);
        }

        private void Client_OnWriteSocketData(object sender, byte[] data, int count)
        {
            Console.WriteLine("SENDING DATA TO SOCKET (byte count): " + count);
        }

        private void Client_OnAuthError(object sender, Element e)
        {
            Console.WriteLine("AUTH ERR: " + e.ToString());
        }

        private void Client_OnLogin(object sender)
        {
            Console.WriteLine("ON LOGIN: " + "logged in -" + sender.ToString());
        }

        private void Client_OnError(object sender, Exception ex)
        {
            Console.WriteLine("ON ERR: " + ex.Message);
        }

        private void Client_OnXmppConnectionStateChanged(object sender, XmppConnectionState state)
        {
            Console.WriteLine("ON CON STATE CHANGE: " + sender.ToString() + "\n" + "State: " + state.ToString());
        }

        private void Client_OnClose(object sender)
        {
            Console.WriteLine("CLOSED: " + sender.ToString());
        }
    }
}