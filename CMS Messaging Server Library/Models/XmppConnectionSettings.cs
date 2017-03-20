namespace CMS.Messaging.Server.Library.Models
{
    using agsXMPP.net;

    public class XmppConnectionSettings
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public string ConnectServer { get; set; }

        public int Port { get; set; }

        public bool UseSSL { get; set; }

        public bool UseStartTLS { get; set; }

        public bool AutoResolveConnectServer { get; set; }

        public bool KeepAlive { get; set; }

        public SocketConnectionType ConnectionType { get; set; }
    }
}