using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Playground
{
    public class ReplayResult<TUserData> : IterationResult
    {
        public Checkpoint[] Checkpoints
        {
            get { return (Checkpoint[]) base.Checkpoints; }
            set { base.Checkpoints = value;  }
        }

        public TUserData UserData
        {
            get { return (TUserData)((IIterationMetadata<object>)this).UserData; }
            set { ((IIterationMetadata<object>)this).UserData = value; }
        }
    }
}