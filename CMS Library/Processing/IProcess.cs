namespace CMS.Library.Processing
{
    using System;

    public interface IProcess : IDisposable
    {
        bool IsStarted { get; }

        void Start();

        void Stop();
    }
}