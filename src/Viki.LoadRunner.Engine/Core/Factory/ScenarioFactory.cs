using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ScenarioFactory<T> : IScenarioFactory
    {
        private readonly Type _scenarioType;

        public ScenarioFactory(Type scenarioType)
        {
            if (scenarioType == null)
                throw new ArgumentNullException(nameof(scenarioType));

            if (!scenarioType.GetInterfaces().Contains(typeof(T)))
                throw new ArgumentException($"Provided {nameof(scenarioType)} must implement {typeof(T)}", nameof(scenarioType));

            _scenarioType = scenarioType;
        }

        public object Create()
        {
            T instance = (T)Activator.CreateInstance(_scenarioType);

            return instance;
        }
    }
}