using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Utils
{
    public class CheckpointOrderLearner
    {

        private List<string> _checkpointOrder;
        private readonly static Checkpoint CheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public IReadOnlyList<string> LearnedOrder => _checkpointOrder; 

        public CheckpointOrderLearner()
        {
            Reset();
        }

        public void Reset()
        {

            _checkpointOrder = new List<string>();
        }

        public void Learn(TestContextResult resultContext)
        {

            Checkpoint previousCheckpoint = CheckpointBase;
            foreach (Checkpoint checkpoint in resultContext.Checkpoints)
            {
                if (!_checkpointOrder.Contains(checkpoint.CheckpointName))
                { 
                    if (_checkpointOrder.Count == 0)
                    {
                        _checkpointOrder.Add(checkpoint.CheckpointName);
                    }
                    else
                    {
                        int insertPosition = _checkpointOrder.FindIndex(s => s == previousCheckpoint.CheckpointName) + 1;

                        if (insertPosition == 0 || insertPosition == _checkpointOrder.Count)
                            _checkpointOrder.Add(checkpoint.CheckpointName);
                        else
                            _checkpointOrder.Insert(insertPosition, checkpoint.CheckpointName);
                    }
                }

                previousCheckpoint = checkpoint;
            }
            
        }
    }
}