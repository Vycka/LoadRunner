namespace Viki.LoadRunner.Engine.Executor.Scenario.Interfaces
{
    public interface IScenario
    {
        void ScenarioSetup(IIteration context);

        void IterationSetup(IIteration context);

        /// <summary>
        /// Test scenario for single thread
        /// </summary>
        /// <param name="context"></param>
        void ExecuteScenario(IIteration context);


        void IterationTearDown(IIteration context);

        void ScenarioTearDown(IIteration context);
    }
}