using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    public class IterationContext : IIterationControl
    {
        #region Fields

        // TODO: Create List specific for this job, the one which could perform shallow copy of array to gain performance
        private readonly List<ICheckpoint> _checkpoints;

        private Checkpoint _activeCheckpoint = null;

        public IterationContext(int threadId, ITimer timer, object initialUserData = null)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            
            ThreadId = threadId;
            Timer = timer;
            UserData = initialUserData;
            _checkpoints = new List<ICheckpoint>();

            Reset(-1,-1);
        }

        #endregion


        public TimeSpan IterationStarted { get; private set; }
        public TimeSpan IterationFinished { get; private set; }

        #region IIterationContextControl methods

        public void Start()
        {
            IterationStarted = Timer.Value;
        }

        public void Stop()
        {
            IterationFinished = Timer.Value;
        }

        public void Reset(int threadIterationId, int globalIterationId)
        {
            GlobalIterationId = globalIterationId;
            ThreadIterationId = threadIterationId;

            _checkpoints.Clear();

            IterationStarted = TimeSpan.MaxValue;
            IterationFinished = TimeSpan.MinValue;
        }

        public void SetError(Exception error)
        {
            _activeCheckpoint.Error = error;
        }

        #endregion

        #region IIteration

        public void Checkpoint(string checkpointName = null)
        {
            if (checkpointName == null)
                checkpointName = $"Checkpoint #{_checkpoints.Count + 1}";

            _activeCheckpoint = new Checkpoint(checkpointName, IterationElapsedTime);
            _checkpoints.Add(_activeCheckpoint);
        }

        IReadOnlyList<ICheckpoint> IIteration.Checkpoints => _checkpoints.AsReadOnly();

        public ICheckpoint[] CopyCheckpoints()
        {
            ICheckpoint[] copy = new ICheckpoint[_checkpoints.Count];
            _checkpoints.CopyTo(copy);

            return copy;
        }

        public TimeSpan IterationElapsedTime
        {
            get
            {
                if (IterationFinished != TimeSpan.MinValue)
                    return IterationFinished - IterationStarted;
                if (IterationStarted != TimeSpan.MaxValue)
                    return Timer.Value - IterationStarted;

                return TimeSpan.Zero;
            }
        }

        public ITimer Timer { get; }

        public int GlobalIterationId { get; private set; }
        public int ThreadId { get; }
        public int ThreadIterationId { get; private set; }
        public object UserData { get; set; }

        #endregion
    }
}