using Viki.LoadRunner.Engine.Executor.Context.Interfaces;

namespace Viki.LoadRunner.Engine
{
    public interface ILoadTestScenario
    {
        void ScenarioSetup(IIterationContext context);

        void IterationSetup(IIterationContext context);

        /// <summary>
        /// Test scenario for single thread
        /// </summary>
        /// <param name="context"></param>
        void ExecuteScenario(IIterationContext context);


        void IterationTearDown(IIterationContext context);

        void ScenarioTearDown(IIterationContext context);
    }
}