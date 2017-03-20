namespace CMS.Messaging.Server.Library.Communications
{
    using agsXMPP;
    using agsXMPP.net;
    using agsXMPP.protocol.client;
    using agsXMPP.sasl;

    public interface IXmppConnectionClient : IConnectionClient
    {
        event MessageHandler OnMessage;

        event XmppElementHandler OnAuthError;

        event SaslEventHandler OnSaslStart;

        event ObjectHandler OnLogin;

        event ObjectHandler OnClose;

        event ErrorHandler OnError;

        event ErrorHandler OnSocketError;

        event XmppConnectionStateHandler OnXmppConnectionStateChanged;

        event BaseSocket.OnSocketDataHandler OnWriteSocketData;

        Jid ServerJabberID { get; }

        Jid MyJabberID { get; }
    }
}