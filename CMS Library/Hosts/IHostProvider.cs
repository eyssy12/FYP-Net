namespace CMS.Library.Hosts
{
    using System;
    using Factories;

    public interface IHostProvider : IDisposable
    {
        IFactory Factory { get; }

        IHost Construct();
    }
}