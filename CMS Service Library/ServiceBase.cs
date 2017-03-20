namespace CMS.Service.Library
{
    using System;
    using CMS.Library.Services;

    public abstract class ServiceBase : IService
    {
        protected readonly string Name;

        protected ServiceBase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "message");
            }

            this.Name = name;
        }

        public abstract void Start();

        public abstract void Stop();
    }
}