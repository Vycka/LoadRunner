using System;
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
            Reset();

            _beginTime = DateTime.UtcNow;
            _stopwatch.Start();
        }

        public void Reset()
        {
            _stopwatch.Reset();
            _beginTime = DateTime.MinValue;
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