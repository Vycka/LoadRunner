using System;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    public class ScenarioHandler : IScenarioHandler
    {
        private readonly IScenario _scenario;

        protected readonly IGlobalCountersControl _globalCounters;
        protected readonly IIterationControl _context;

        protected int _threadIterationId;

        public ScenarioHandler(IGlobalCountersControl globalCounters, IScenario scenario, IIterationControl context)
        {
            if (globalCounters == null)
                throw new ArgumentNullException(nameof(globalCounters));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _scenario = scenario;
            _context = context;

            _globalCounters = globalCounters;
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
            _context.Reset(_threadIterationId++, _globalCounters.IterationId.Next());
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
                _globalCounters.Errors.Add(1);
                _context.SetError(ex);
            }

            return result;
        }
    }
}