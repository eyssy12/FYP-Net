namespace CMS.Messaging.Server.Library.Actions.Parsing
{
    using CMS.Library.Factories;
    using Models;
    using Newtonsoft.Json.Linq;
    using Xmpp;

    public class ParseSenderTokenAction : ActionBase, IAction<GcmMessageStateHolder>
    {
        public ParseSenderTokenAction(IFactory factory, string identifierKey = Protocol.From)
            : base(factory, identifierKey)
        {
        }

        public void Run(GcmMessageStateHolder item)
        {
            JToken fromToken;
            if (item.Payload.TryGetValue(this.IdentifierKey, out fromToken))
            {
                item.From = fromToken.ToObject<string>();
            }
        }
    }
}