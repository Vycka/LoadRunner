using System;
using System.Diagnostics;
using Viki.LoadRunner.Engine.Executor.Strategy.Timer.Interfaces;

#pragma warning disable 1591

namespace Viki.LoadRunner.Engine.Executor.Strategy.Timer
{
    public class ExecutionTimer : ITimerControl
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private DateTime _beginTime;

        public void Start()
        {
            _beginTime = DateTime.UtcNow;

            _stopwatch.Reset();
            _stopwatch.Start();
        }


        public void Stop()
        {
            _stopwatch.Stop();
        }

        public TimeSpan Value => _stopwatch.Elapsed;
        public bool IsRunning => _stopwatch.IsRunning;
        public DateTime BeginTime => _beginTime;
    }
}