namespace CMS.Messaging.Server.Library.Models
{
    using System;
    using agsXMPP.protocol.client;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class GcmMessageStateHolder : MessageStateHolderBase
    {
        private Message message;
        private Lazy<JObject> payload;

        public GcmMessageStateHolder(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "blah"); // TODO:better message and add to resources
            }

            this.message = message;
            this.payload = new Lazy<JObject>(() => this.ParseMessage());
        }

        public Message Message
        {
            get { return this.message; }
        }

        public JObject Payload
        {
            get { return this.payload.Value; }
        }

        private JObject ParseMessage()
        {
            return JsonConvert.DeserializeObject<JObject>(this.Message.FirstChild.Value);
        }
    }
}