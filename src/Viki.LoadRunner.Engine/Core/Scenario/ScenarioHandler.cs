using System;
using Viki.LoadRunner.Engine.Core.Generator.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    public class ScenarioHandler : IScenarioHandler
    {
        private readonly IScenario _scenario;

        private readonly IGlobalCountersControl _globalCounters;
        private readonly IUniqueIdGenerator<int> _threadIterationIdGenerator;
        protected readonly IIterationControl _context;

        private bool _setupSuccess;


        public ScenarioHandler(IGlobalCountersControl globalCounters, IUniqueIdGenerator<int> threadIterationIdGenerator, IScenario scenario, IIterationControl context)
        {
            _scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalCounters = globalCounters ?? throw new ArgumentNullException(nameof(globalCounters));
            _threadIterationIdGenerator = threadIterationIdGenerator ?? throw new ArgumentNullException(nameof(threadIterationIdGenerator));
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
            _context.Reset(_threadIterationIdGenerator.Next(), _globalCounters.IterationId.Next());
        }

        public void Warmup()
        {
            _context.Checkpoint(Checkpoint.Names.Setup);
            _setupSuccess = ExecuteWithExceptionHandling(() => _scenario.IterationSetup(_context));
        }

        /// <summary>
        /// Executes iteration
        /// </summary>
        public void Execute()
        {
            if (_setupSuccess)
            {
                _context.Checkpoint(Checkpoint.Names.Iteration);

                _context.Start();
                ExecuteWithExceptionHandling(() => _scenario.ExecuteScenario(_context));
                _context.Stop();

            }
            else
            {
                _context.Skip();
            }

            _context.Checkpoint(Checkpoint.Names.TearDown);
            ExecuteWithExceptionHandling(() => _scenario.IterationTearDown(_context));
        }

        /// <summary>
        /// Final cleanup
        /// </summary>
        public void Cleanup()
        {
            _context.Reset(-1, -1);
            _scenario.ScenarioTearDown(_context);
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