namespace CMS.Messaging.Server.Library.Xmpp
{
    using System;
    using agsXMPP;
    using agsXMPP.protocol.client;
    using agsXMPP.Xml.Dom;
    using CMS.Library.Extensions;
    using Messaging.Server.Library.Models;
    using Newtonsoft.Json;

    public static class Extensions
    {
        public const int DefaultTimeToLive = 86400;

        private const string MessageNamespace = "jabber:client",
            GcmNamespace = "google:mobile:data",
            TagNameMessage = "message",
            TagNameGcm = "gcm",
            AtXmppEscape = "\\40",
            AtCharacter = "@";

        public static Jid ConstructPlainJid(this XmppClientConnection connection)
        {
            return new Jid(Extensions.ParseJid(connection.Username));
        }

        public static Jid ConstructPlainJid(this string username)
        {
            return new Jid(Extensions.ParseJid(username));
        }

        public static string ConstructPlainJidAsString(this XmppClientConnection connection)
        {
            return Extensions.ParseJid(connection.Username);
        }

        public static string ConstructPlainJidAsString(this string username)
        {
            return Extensions.ParseJid(username);
        }

        public static NotificationMessage CreateNotificationMessage(
            this NotificationContents contents,
            string messageId, 
            string to, 
            string category, 
            int timeToLive = Extensions.DefaultTimeToLive)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents), "message"); // TODO: message in resources
            }

            return new NotificationMessage
            {
                MessageId = messageId,
                To = to,
                Category = category,
                TimeToLive = timeToLive,
                Notification = contents
            };
        }

        public static Message CreateNotificationMessage(
            this Jid to,
            Jid from,
            object contents,
            MessageType messageType = MessageType.normal,
            JsonSerializerSettings settings = null)
        {
            Message msg = new Message(
                to: to,
                from: from,
                type: messageType,
                body: null);

            Element element = msg.FirstChild;
            element.Namespace = Extensions.GcmNamespace;
            element.TagName = Extensions.TagNameGcm;
            element.NodeType = NodeType.Element;

            Node node = msg.FirstChild.ChildNodes.Item(0);
            node.NodeType = NodeType.Text;

            node.Value = contents.SerializeAsJson(settings: settings);
            msg.AddChild(element);

            return msg;
        }

        public static Message CreateNotificationMessage(
            this Jid to, 
            Jid from, 
            NotificationMessage notification, 
            MessageType messageType = MessageType.normal, 
            JsonSerializerSettings settings = null)
        {
            Message msg = new Message(
                to: to, 
                from: from, 
                type: messageType, 
                body: null);

            Element element = msg.FirstChild;
            element.Namespace = Extensions.GcmNamespace;
            element.TagName = Extensions.TagNameGcm;
            element.NodeType = NodeType.Element;

            Node node = msg.FirstChild.ChildNodes.Item(0);
            node.NodeType = NodeType.Text;

            node.Value = notification.SerializeAsJson(settings: settings);
            msg.AddChild(element);

            return msg;
        }

        private static string ParseJid(string username)
        {
            string[] split = username.Split(new[] { Extensions.AtXmppEscape }, StringSplitOptions.None);

            return string.Concat(split[0], Extensions.AtCharacter, split[1]);
        }
    }
}