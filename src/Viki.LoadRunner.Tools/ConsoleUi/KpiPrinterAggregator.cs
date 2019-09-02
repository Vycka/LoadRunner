using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Tools.ConsoleUi
{
    public class KpiPrinterAggregator : IAggregator
    {
        private readonly TimeSpan _interval;
       
        private MetricsHandler<IResult> _metrics;

        public Action<IEnumerable<Val>> OutputAction = (r) => Console.Out.WriteLine(
                String.Join(
                    Environment.NewLine,
                    r.Select(kv => String.Concat(" * ", kv.Key, ": ", kv.Value))
                ) + Environment.NewLine
            );

        private Timer _timer;

        public KpiPrinterAggregator(TimeSpan interval, params IMetric<IResult>[] metrics)
        {
            _metrics = new MetricsHandler<IResult>(metrics);
            _interval = interval;
        }


        public void Begin()
        {
            _metrics = _metrics.Create();

            _timer = new Timer(_interval.TotalMilliseconds);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) => Output();

            _timer.Start();
        }

        public void Aggregate(IResult result)
        {
            _metrics.Add(result);
        }

        public void End()
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        private void Output()
        {
            MetricsHandler<IResult> current = _metrics;
            _metrics = _metrics.Create();


            OutputAction(current.Export());
        }
    }
}