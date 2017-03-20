namespace CMS.Library.Events
{
    using System;

    public class EventArgs<T1> : EventArgs
    {
        public readonly T1 Item1;

        public EventArgs(T1 item1)
        {
            this.Item1 = item1;
        }
    }
}