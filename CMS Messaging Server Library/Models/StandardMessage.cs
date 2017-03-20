namespace CMS.Messaging.Server.Library.Models
{
    public abstract class MessageBase
    {
        public string MessageId { get; set; }

        public int TimeToLive { get; set; }

        public string To { get; set; }

        public string Category { get; set; }
    }
}