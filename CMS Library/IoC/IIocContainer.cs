namespace CMS.Library.IoC
{
    using System;
    using Ninject.Parameters;

    public interface IIocContainer : IDisposable
    {
        T ResolveAutomatically<T>();

        object Get(Type type, string named = null, params IParameter[] parameters);

        T Get<T>(string named = null, params IParameter[] parameters);

        T TryGet<T>(string named = null, params IParameter[] parameters);

        void Inject(object item);
    }
}