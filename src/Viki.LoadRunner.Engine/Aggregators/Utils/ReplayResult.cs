using System;
using System.Diagnostics;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    [DebuggerDisplay("T:{ThreadIterationId} G:{GlobalIterationId} L:{ThreadIterationId} TS:{(int)(IterationStarted.TotalMilliseconds)}")]
    public class ReplayResult<TUserData> : IterationResult
    {
        private Checkpoint[] _realCheckpoints;

        public new Checkpoint[] Checkpoints
        {
            get { return _realCheckpoints; }
            set
            {
                base.Checkpoints = value.Cast<ICheckpoint>().ToArray();
                _realCheckpoints = value;
            }
        }

        public new TUserData UserData
        {
            get { return (TUserData)base.UserData; }
            set { base.UserData = value; }
        }

        /// <summary>
        /// offsets IterationStarted and IterationFinished values by provided offset
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(TimeSpan offset)
        {
            IterationStarted += offset;
            IterationFinished += offset;
        }
    }
}