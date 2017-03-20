namespace CMS.Messaging.Server.Library.Xmpp
{
    public static class Protocol
    {
        public const string Action = "action",
            Value = "value",
            DataContents = "data",
            EntityId = "entity_id",
            Timestamp = "timestamp",
            Registration = "registration",
            ReRegistration = "reregistration",
            CancelEvent = "event_cancelled",
            ModifyEvent = "modify_event",
            Notification = "notification",
            Message = "message",
            MessageType = "message_type",
            From = "from",
            To = "to";
    }
}