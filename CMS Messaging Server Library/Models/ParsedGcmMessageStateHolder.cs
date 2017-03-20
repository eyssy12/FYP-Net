namespace CMS.Messaging.Server.Library.Models
{
    using System;

    public class ParsedGcmMessageStateHolder : MessageStateHolderBase
    {
        public ParsedGcmMessageStateHolder()
        {
        }

        public ParsedGcmMessageStateHolder(GcmMessageStateHolder stateHolder)
        {
            if (stateHolder == null)
            {
                throw new ArgumentNullException(nameof(stateHolder), "A stateholder with requried metadata is requried");
            }

            this.PopulateStateHolder(stateHolder);
        }

        protected void PopulateStateHolder(GcmMessageStateHolder stateHolder)
        {
            this.Action = stateHolder.Action;
            this.Value = stateHolder.Value;
            this.EntityId = stateHolder.EntityId;
            this.MessageType = stateHolder.MessageType;
            this.From = stateHolder.From;
            this.IsDataMessage = stateHolder.IsDataMessage;
            this.IsNotificationMessage = stateHolder.IsNotificationMessage;
            this.Timestamp = stateHolder.Timestamp;
        }
    }
}