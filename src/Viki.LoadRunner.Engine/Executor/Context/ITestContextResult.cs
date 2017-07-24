﻿using System;

namespace Viki.LoadRunner.Engine.Executor.Context
{
    public interface ITestContextResult : IIterationMetadata<object>
    {
        /// <summary>
        /// All checkpoints containing meassurements from whole iteration
        /// </summary>
        ICheckpoint[] Checkpoints { get; }

        /// <summary>
        /// It contains value when this iteration  started (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationStarted { get; }

        /// <summary>
        /// It contains value when this iteration ended (relative to LoadTest start)
        /// </summary>
        TimeSpan IterationFinished { get; }
    }
}