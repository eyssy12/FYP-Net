namespace CMS.Messaging.Server.Library.Communications
{
    using System;
    using agsXMPP;
    using agsXMPP.net;
    using agsXMPP.protocol.client;
    using agsXMPP.sasl;
    using CMS.Library.Extensions;
    using Models;
    using AgsXmppConnectionClient = agsXMPP.XmppClientConnection;

    public class XmppConnectionClient : IXmppConnectionClient
    {
        protected readonly XmppConnectionSettings Settings;

        private readonly AgsXmppConnectionClient client;

        private Jid serverJabberID;

        public XmppConnectionClient(XmppConnectionSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "Xmpp client settings are missing - without those, the client would not be able to inititate communications with GCM servers"); // TODO: message
            }

            this.Settings = settings;
            this.client = new AgsXmppConnectionClient
            {
                UseSSL = settings.UseSSL,
                UseStartTLS = settings.UseStartTLS,
                Server = settings.Server,
                ConnectServer = settings.ConnectServer,
                Port = settings.Port,
                Username = settings.Username,
                Password = settings.Password,
                AutoResolveConnectServer = settings.AutoResolveConnectServer,
                SocketConnectionType = settings.ConnectionType,
                KeepAlive = settings.KeepAlive,
            };

            this.OnMessage += this.RetrieveServerJabberID;
        }

        public event MessageHandler OnMessage;

        public event XmppElementHandler OnAuthError;

        public event SaslEventHandler OnSaslStart;

        public event ObjectHandler OnLogin;

        public event ObjectHandler OnClose;

        public event ErrorHandler OnSocketError;

        public event XmppConnectionStateHandler OnXmppConnectionStateChanged;

        public event BaseSocket.OnSocketDataHandler OnWriteSocketData;

        public event ErrorHandler OnError;

        public Jid ServerJabberID
        {
            get
            {
                return this.serverJabberID;
            }

            private set
            {
                this.serverJabberID = value;
                this.OnMessage -= this.RetrieveServerJabberID;
            }
        }

        public Jid MyJabberID
        {
            get { return this.client.MyJID;  }
        }

        public void Connect()
        {
            this.client.OnSaslStart += this.OnSaslStart;
            this.client.OnAuthError += this.OnAuthError;
            this.client.OnMessage += this.OnMessage;
            this.client.OnClose += this.OnClose;
            this.client.OnLogin += this.OnLogin;
            this.client.OnSocketError += this.OnSocketError;
            this.client.OnXmppConnectionStateChanged += this.OnXmppConnectionStateChanged;
            this.client.OnWriteSocketData += this.OnWriteSocketData;
            this.client.OnError += this.OnError;

            this.client.Open();
        }

        public void Close()
        {
            this.client.Close();

            this.client.OnSaslStart -= this.OnSaslStart;
            this.client.OnAuthError -= this.OnAuthError;
            this.client.OnMessage -= this.OnMessage;
            this.client.OnClose -= this.OnClose;
            this.client.OnLogin -= this.OnLogin;
            this.client.OnSocketError -= this.OnSocketError;
            this.client.OnXmppConnectionStateChanged -= this.OnXmppConnectionStateChanged;
            this.client.OnWriteSocketData -= this.OnWriteSocketData;
            this.client.OnError -= this.OnError;
        }

        public void Send(object item)
        {
            Message message = item.Retrieve<Message>(this);

            this.client.Send(message);
        }

        private void RetrieveServerJabberID(object sender, Message msg)
        {
            if (this.ServerJabberID == null)
            {
                this.ServerJabberID = msg.From;
            }
        }
    }
}