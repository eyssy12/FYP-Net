namespace CMS.Messaging.Server.Library.Models
{
    public abstract class MessageStateHolderBase
    {
        public string MessageType { get; set; }

        public string From { get; set; }

        public string EntityId { get; set; }

        public bool IsNotificationMessage { get; set; }

        public bool IsDataMessage { get; set; }

        public string Action { get; set; }

        public string Value { get; set; }

        public string Category { get; set; }

        public int TimeToLive { get; set; }

        public string Timestamp { get; set; }
    }
}