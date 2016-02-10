using System.Diagnostics;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Playground
{
    [DebuggerDisplay("T:{ThreadIterationId} G:{GlobalIterationId} L:{ThreadIterationId}")]
    public class ReplayResult<TUserData> : IterationResult
    {
        public new Checkpoint[] Checkpoints
        {
            get { return (Checkpoint[]) base.Checkpoints; }
            set { base.Checkpoints = value;  }
        }

        public new TUserData UserData
        {
            get { return (TUserData)((IIterationMetadata<object>)this).UserData; }
            set { ((IIterationMetadata<object>)this).UserData = value; }
        }
    }
}