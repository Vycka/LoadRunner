using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ScenarioFactory : ReflectionFactory<IScenario>, IScenarioFactory
    {
        public ScenarioFactory(Type createType) : base(createType)
        {
        }
    }
}