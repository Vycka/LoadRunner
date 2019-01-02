using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces
{
    public interface ISpeedStrategy
    {
        /// <summary>
        /// Will be called within load-test initialization, before initial threads created.
        /// </summary>
        /// <param name="state">Global test state instance which is used throughout the whole test</param>
        void Setup(ITestState state);

        /// <summary>
        /// This must be thread safe! (as all threads will ask this single instance for work)
        /// Each thread before iteration will call Next() to check when it should execute the iteration
        /// Thread execution is controller by telling in ISchedule what to do.
        /// Setting to Idle - thread will wait until specified TimePoint and call Next() again.
        /// Setting to Execute - thread will wait until specified TimePoint, then executes IterationSetup/Execute/IterationTeardown sequence, and call's Next() again.
        /// </summary>
        /// <param name="id">Metadata object associated by caller thread</param>
        /// <param name="scheduler">scheduler to control thread execution</param>
        void Next(IIterationId id, ISchedule scheduler); // Must be thread safe

        /// <summary>
        /// IStrategyExecutor fires HeartBeat from its own root thread.
        /// This can be used to adjust some parameters (like changing throughput from execution time)
        /// </summary>
        /// <param name="state">same test state submitted on Setup but with.</param>
        void HeartBeat(ITestState state);

        /// <summary>
        /// Each thread after initialization will call ThreadStarted()
        /// </summary>
        /// <param name="id">Metadata object associated by the caller thread</param>
        /// <param name="scheduler">scheduler associated with thread and with its initial state</param>
        void ThreadStarted(IIterationId id, ISchedule scheduler);


        /// <summary>
        /// Each thread after its graceful stop will call this by passing id and scheduler in its latest states.
        /// </summary>
        /// <param name="id">Metadata object associated by the caller thread and contains its last state</param>
        /// <param name="scheduler">scheduler associated with thread and contains its last state</param>
        void ThreadFinished(IIterationId id, ISchedule scheduler);
    }
}