using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ReflectionFactory<T> : IFactory<T>
    {
        private readonly Type _type;

        public ReflectionFactory(Type createType)
        {
            if (createType == null)
                throw new ArgumentNullException(nameof(createType));

            if (!createType.GetInterfaces().Contains(typeof(T)))
                throw new ArgumentException($"Provided {nameof(createType)} must implement {typeof(T)}", nameof(createType));

            _type = createType;
        }

        public T Create()
        {
            T instance = (T)Activator.CreateInstance(_type);

            return instance;
        }
    }
}