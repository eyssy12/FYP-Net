namespace CMS.Library.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Ninject;
    using Ninject.Modules;
    using Ninject.Parameters;

    public class NinjectContainer : IIocContainer
    {
        protected readonly StandardKernel Kernel;

        public NinjectContainer(params INinjectModule[] modules)
        {
            if (modules.IsEmpty())
            {
                throw new ArgumentNullException(nameof(modules), "message"); // TODO: message
            }

            this.Kernel = new StandardKernel(new NinjectSettings { AllowNullInjection = true });
            this.Kernel.Load(modules);
        }

        public object ResolveAutomatically(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo constructor = constructors[0];

            IList<IParameter> constructorParameters = new List<IParameter>();
            constructor.GetParameters().ForEach(p =>
            {
                string name = p.Name;
                string named = this.GetNamedAttributeValue(p);
                object value;

                Type parameterType = p.ParameterType;

                if (string.IsNullOrWhiteSpace(named))
                {
                    value = this.Get(p.ParameterType);
                }
                else
                {
                    value = this.Get(p.ParameterType, named);
                }

                constructorParameters.Add(new ConstructorArgument(name, value));
            });

            return this.Get(type, parameters: constructorParameters.ToArray());
        }

        public T ResolveAutomatically<T>()
        {
            Type baseType = typeof(T);

            return (T)this.ResolveAutomatically(baseType);
        }

        public object Get(Type type, string named = null, params IParameter[] parameters)
        {
            try
            {
                return this.Kernel.Get(type, named, parameters);
            }
            catch
            {
                throw;
            }
        }

        public T Get<T>(string named = null, params IParameter[] parameters)
        {
            try
            {
                return this.GetDependency(
                    named,
                    (p) => this.Kernel.Get<T>(p),
                    (s, p) => this.Kernel.Get<T>(s, p),
                    parameters);
            }
            catch
            {
                throw;
            }
        }

        public void Inject(object item)
        {
            this.Kernel.Inject(item);
        }

        public T TryGet<T>(string named = null, params IParameter[] parameters)
        {
            return this.GetDependency(
                named,
                (p) => this.Kernel.TryGet<T>(p), 
                (s, p) => this.Kernel.TryGet<T>(s, p),
                parameters);
        }

        protected T GetDependency<T>(
            string named,
            Func<IParameter[], T> defaultRetriever,
            Func<string, IParameter[], T> namedRetriever, 
            params IParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(named))
            {
                return defaultRetriever(parameters);
            }

            return namedRetriever(named, parameters);
        }

        protected string GetNamedAttributeValue(ParameterInfo parameter)
        {
            CustomAttributeData namedAttribute = parameter
                .CustomAttributes
                .SingleOrDefault(c => c.AttributeType == typeof(NamedAttribute));

            if (namedAttribute == null)
            {
                return null;
            }

            CustomAttributeTypedArgument data = namedAttribute.ConstructorArguments.SingleOrDefault();

            if (data == null)
            {
                return null;
            }

            return data.Value as string;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}