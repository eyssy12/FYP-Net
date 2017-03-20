namespace CMS.Library.Hosts
{
    using System;
    using Events;
    using Processing;
    using ProcessingStatusEnum = CMS.Library.Enumerations.ProcessingStatus;

    public interface IHost : IProcess
    {
        event EventHandler<EventArgs<ProcessingStatusEnum>> StatusChanged;

        ProcessingStatusEnum Status { get; }

        string Name { get; }
    }
}