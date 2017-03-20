namespace CMS.Messaging.Server.Library.Actions.Parsing
{
    using CMS.Library.Factories;
    using Models;
    using Newtonsoft.Json.Linq;
    using Xmpp;

    public class ParseMessageTypeAction : ActionBase, IAction<GcmMessageStateHolder>
    {
        public ParseMessageTypeAction(IFactory factory, string identifierKey = Protocol.MessageType)
            : base(factory, identifierKey)
        {
        }

        public void Run(GcmMessageStateHolder item)
        {
            JToken messageTypeToken;
            if (item.Payload.TryGetValue(this.IdentifierKey, out messageTypeToken))
            {
                item.MessageType = messageTypeToken.ToObject<string>();
            }
        }
    }
}