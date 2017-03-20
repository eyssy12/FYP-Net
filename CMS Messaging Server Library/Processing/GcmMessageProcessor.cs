namespace CMS.Messaging.Server.Library.Processing
{
    using System.Collections.Generic;
    using Actions;
    using CMS.Library.Extensions;
    using Models;

    public class GcmMessageProcessor : ActionProcessor<GcmMessageStateHolder>
    {
        public GcmMessageProcessor(IEnumerable<IAction<GcmMessageStateHolder>> actions)
            : base(actions)
        {
        }
        
        public override void Process(GcmMessageStateHolder item)
        {
            this.Actions.ForEach(action =>
            {
                action.Run(item);
            });
        }
    }
}