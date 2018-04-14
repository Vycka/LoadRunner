using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    public class ScenarioHandler : IScenarioHandler
    {
        private readonly IScenario _scenario;

        protected readonly IUniqueIdFactory<int> _globalIdFactory;
        protected readonly IIterationControl _context;

        protected int _threadIterationId;

        public ScenarioHandler(IUniqueIdFactory<int> globalIdFactory, IScenario scenario, IIterationControl context)
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
        /// Prepares context for next iteration
        /// </summary>
        public void PrepareNext()
        {
            _context.Reset(_threadIterationId++, _globalIdFactory.Next());
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
        /// Executes iteration
        /// </summary>
        public void Execute()
        {
            _context.Checkpoint(Checkpoint.Names.Setup);
            bool setupSuccess = ExecuteWithExceptionHandling(() => _scenario.IterationSetup(_context));

            if (setupSuccess)
            {
                _context.Checkpoint(Checkpoint.Names.Iteration);

                _context.Start();
                ExecuteWithExceptionHandling(() => _scenario.ExecuteScenario(_context));
                _context.Stop();

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