using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Results
{
    public class ResultItemTotals : ResultItem
    {
        private readonly List<Exception> _iterationSetupErrors;
        private readonly List<Exception> _iterationTearDownErrors;


        public ResultItemTotals(Dictionary<string, ResultItemRow> resultRows)
        {
            ResultItemRow headerRow = resultRows[Checkpoint.IterationStartCheckpointName];
            ResultItemRow footerRow = resultRows[Checkpoint.IterationEndCheckpointName];
            ResultItemRow tearDownRow = resultRows[Checkpoint.IterationTearDownEndCheckpointName];

            
            TotalIterationCount = headerRow.Count;
            TotalErrorCount = resultRows.Sum(resultItemRow => resultItemRow.Value.ErrorCount);
            TotalDuration = footerRow.GetLastIterationEndTime() - headerRow.GetFirstIterationBeginTime();
            AbortedThreads = headerRow.Count - footerRow.Count - footerRow.ErrorCount;

            _iterationSetupErrors = headerRow.GetErrors().ToList();
            _iterationTearDownErrors = tearDownRow.GetErrors().ToList();
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