using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregates.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Results
{
    public class ResultItemTotals
    {
        private readonly List<Exception> _iterationSetupErrors;
        private readonly List<Exception> _iterationTearDownErrors;


        public ResultItemTotals(Dictionary<string, AggregatedCheckpoint> resultRows)
        {
            AggregatedCheckpoint setupRow = resultRows[Checkpoint.IterationSetupCheckpointName];
            AggregatedCheckpoint tearDownRow = resultRows[Checkpoint.IterationTearDownCheckpointName];

            TotalDuration = tearDownRow.LastIterationEndTime - setupRow.FirsIterationBeginTime;

            TotalIterationCount = setupRow.Count;
            TotalErrorCount = resultRows.Sum(resultItemRow => resultItemRow.Value.Errors.Count);

            // setupRow not, use iteration begin
            
            AbortedThreads = TotalIterationCount - tearDownRow.Count;

            _iterationSetupErrors = setupRow.Errors;
            _iterationTearDownErrors = tearDownRow.Errors;
        }

        public readonly int AbortedThreads;
        public readonly int TotalIterationCount;
        public readonly int TotalErrorCount;
        public int IterationsErrorRate => (1 / TotalIterationCount) * TotalErrorCount;

        public readonly TimeSpan TotalDuration;

        public IReadOnlyList<Exception> GetIterationSetupErrors() => _iterationSetupErrors;
        public IReadOnlyList<Exception> GetIterationTearDownErrors() => _iterationTearDownErrors;

        public int IterationSetupErrorCount => _iterationSetupErrors.Count;
        public int IterationTearDownErrorCount => _iterationTearDownErrors.Count;
    }
}