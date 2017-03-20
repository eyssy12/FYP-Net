namespace CMS.Messaging.Server.Library.Xmpp.Mechanisms
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using agsXMPP;
    using agsXMPP.sasl;
    using agsXMPP.Xml.Dom;
    using AgsAuth = agsXMPP.protocol.sasl.Auth;
    using AgsMechanismType = agsXMPP.protocol.sasl.MechanismType;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Looks better in this form.")]
    public class GoogleXmppPlainMechanism : Mechanism
    {
        // http://stackoverflow.com/questions/17697461/gcm-cloud-connection-server-doesnt-respond-to-sasl-on-xmpp

        private static string mechanismName;
        private static Type type;

        private XmppClientConnection connection;

        static GoogleXmppPlainMechanism()
        {
            type = typeof(GoogleXmppPlainMechanism);
            mechanismName = nameof(GoogleXmppPlainMechanism);
        }

        public static Type Type
        {
            get { return GoogleXmppPlainMechanism.type; }
        }

        public static string MechanismName
        {
            get { return GoogleXmppPlainMechanism.mechanismName; }
        }

        public override void Init(XmppClientConnection con)
        {
            this.connection = con;

            con.Send(new AgsAuth(AgsMechanismType.PLAIN, this.ConstructPlainAuthentication()));
        }

        public override void Parse(Node e)
        {
        }

        private string ConstructPlainAuthentication()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append((char)0);
            sb.Append(this.connection.ConstructPlainJidAsString());
            sb.Append((char)0);
            sb.Append(this.Password);

            byte[] msg = Encoding.UTF8.GetBytes(sb.ToString());
            string result = Convert.ToBase64String(msg, 0, msg.Length);

            return result;
        }
    }
}