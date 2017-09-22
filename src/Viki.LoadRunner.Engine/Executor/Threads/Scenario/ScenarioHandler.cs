using System;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;

namespace Viki.LoadRunner.Engine.Executor.Threads.Scenario
{
    // TODO: Interface it
    public class ScenarioHandler
    {
        private readonly IUniqueIdFactory<int> _globalIdFactory;
        private readonly ILoadTestScenario _scenario;

        private int _threadIterationId = 0;

        private Context.IterationContext _context;
        public IIterationResult Context => _context;

        public ScenarioHandler(IUniqueIdFactory<int> globalIdFactory, ILoadTestScenario scenario, int threadId, object initialUserData, ITimer timer)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (globalIdFactory == null)
                throw new ArgumentNullException(nameof(globalIdFactory));

            _scenario = scenario;
            _context = new Context.IterationContext(threadId, timer, initialUserData);
            _globalIdFactory = globalIdFactory;
        }

        /// <summary>
        /// Initial setup
        /// </summary>
        public void Init()
        {
            _context.Reset(-1, -1);
            _scenario.ScenarioSetup(_context);
        }

        /// <summary>
        /// Final cleanup
        /// </summary>
        public void Cleanup()
        {
            _context.Reset(-1, -1);
            _scenario.ScenarioTearDown(_context);
        }

        /// <summary>
        /// Prepares context for next iteration
        /// </summary>
        public void PrepareNext()
        {
            _context.Reset(_threadIterationId++, _globalIdFactory.Next());
        }

        /// <summary>
        /// Executes iteration
        /// </summary>
        public void Execute()
        {
            _context.Checkpoint(Checkpoint.Names.Setup);
            bool setupSuccess = ExecuteWithExceptionHandling(() => _scenario.IterationSetup(_context));

            if (setupSuccess)
            {
                _context.Checkpoint(Checkpoint.Names.IterationStart);

                _context.Start();
                bool iterationSuccess = ExecuteWithExceptionHandling(() => _scenario.ExecuteScenario(_context));
                _context.Stop();

                if (iterationSuccess)
                {
                    _context.Checkpoint(Checkpoint.Names.IterationEnd);
                }
            }
            else
            {
                _context.Start();
                _context.Stop();
            }

            _context.Checkpoint(Checkpoint.Names.TearDown);
            ExecuteWithExceptionHandling(() => _scenario.IterationTearDown(_context));
        }

        private bool ExecuteWithExceptionHandling(Action action)
        {
            bool result = false;

            try
            {
                action.Invoke();
                result = true;
            }
            catch (Exception ex)
            {
                _context.SetError(ex);
            }

            return result;
        }
    }
}