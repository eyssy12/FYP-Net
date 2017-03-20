namespace CMS.Service.Library
{
    using System;
    using System.Collections.Generic;
    using CMS.Library.Extensions;

    public class MessagingService : ServiceBase
    {
        protected readonly IEnumerable<Type> Components;
        
        public MessagingService(string name, IEnumerable<Type> components)
            : base(name)
        {
            if (components.IsEmpty())
            {
                throw new ArgumentNullException(nameof(components), "message"); // TODO: message
            }

            this.Components = components;
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
