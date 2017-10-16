using System;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Scenario
{
    public class ScenarioHandler : IScenarioHandler
    {
        private readonly IUniqueIdFactory<int> _globalIdFactory;
        private readonly ILoadTestScenario _scenario;
        private readonly IIterationContextControl _context;

        private int _threadIterationId;

        public ScenarioHandler(IUniqueIdFactory<int> globalIdFactory, ILoadTestScenario scenario, IIterationContextControl context)
        {

            if (globalIdFactory == null)
                throw new ArgumentNullException(nameof(globalIdFactory));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _scenario = scenario;
            _context = context;

            _globalIdFactory = globalIdFactory;
            _threadIterationId = 0;
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