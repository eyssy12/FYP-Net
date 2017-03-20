namespace CMS.Messaging.Server.Library.Models
{
    public class NotificationMessage : MessageBase
    {
        public NotificationContents Notification { get; set; }
    }
}