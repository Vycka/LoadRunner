﻿using System;
using System.Diagnostics;
#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Timer
{
    public class ExecutionTimer : ITimer
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private DateTime _beginTime;

        public void Start()
        {
            _stopwatch.Reset();
            UpdateCurrent();

            _beginTime = DateTime.UtcNow;
            _stopwatch.Start();
        }

        public void UpdateCurrent()
        {
            CurrentValue = _stopwatch.Elapsed;
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public TimeSpan CurrentValue { get; private set; }
        public DateTime BeginTime => _beginTime;
    }
}