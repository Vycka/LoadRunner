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
            _stopwatch.Reset();
            //UpdateCurrent();

            _beginTime = DateTime.UtcNow;
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public TimeSpan CurrentValue => _stopwatch.Elapsed;
        public bool IsRunning => _stopwatch.IsRunning;
        public DateTime BeginTime => _beginTime;
    }
}