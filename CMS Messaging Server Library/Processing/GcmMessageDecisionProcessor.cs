namespace CMS.Messaging.Server.Library.Processing
{
    using System.Collections.Generic;
    using CMS.Library.Extensions;
    using CMS.Messaging.Server.Library.Actions;
    using CMS.Messaging.Server.Library.Models;

    public class GcmMessageDecisionProcessor : ActionProcessor<ParsedGcmMessageStateHolder>
    {
        public GcmMessageDecisionProcessor(IEnumerable<IAction<ParsedGcmMessageStateHolder>> actions)
            : base(actions)
        {
        }

        public override void Process(ParsedGcmMessageStateHolder item)
        {
            this.Actions.ForEach(action =>
            {
                action.Run(item);
            });
        }
    }
}