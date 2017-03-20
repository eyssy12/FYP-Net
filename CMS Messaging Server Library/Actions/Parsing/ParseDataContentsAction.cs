namespace CMS.Messaging.Server.Library.Actions.Parsing
{
    using System;
    using CMS.Library.Factories;
    using Models;
    using Newtonsoft.Json.Linq;
    using Xmpp;

    public class ParseDataContentsAction : ActionBase, IAction<GcmMessageStateHolder>
    {
        public ParseDataContentsAction(IFactory factory, string identifierKey = Protocol.DataContents)
            : base(factory, identifierKey)
        {
        }

        public void Run(GcmMessageStateHolder item)
        {
            JToken dataContentsToken;
            if (item.Payload.TryGetValue(this.IdentifierKey, out dataContentsToken))
            {
                item.Action = dataContentsToken.Value<string>(Protocol.Action);
                item.Value = dataContentsToken.Value<string>(Protocol.Value);
                item.EntityId = dataContentsToken.Value<string>(Protocol.EntityId);
                item.Timestamp = dataContentsToken.Value<string>(Protocol.Timestamp);
            }
        }
    }
}