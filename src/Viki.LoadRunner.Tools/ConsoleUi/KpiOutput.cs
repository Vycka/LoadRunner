using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Tools.ConsoleUi
{
    public class KpiOutput : IAggregator
    {
        private readonly TimeSpan _interval;
       
        private IMetric _metrics;

        public Action<IEnumerable<KeyValuePair<string, object>>> OutputAction = (r) => Console.Out.WriteLine(
                String.Join(
                    Environment.NewLine,
                    r.Select(kv => String.Concat(" * ", kv.Key, ": ", kv.Value))
                ) + Environment.NewLine
            );

        private Timer _timer;

        public KpiOutput(TimeSpan interval, params IMetric[] metrics)
        {
            _metrics = new MetricMultiplexer(metrics);
            _interval = interval;
        }


        public void Begin()
        {
            _metrics = _metrics.CreateNew();

            _timer = new Timer(_interval.TotalMilliseconds);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) => Output();

            _timer.Start();
        }

        public void TestContextResultReceived(IResult result)
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
            IMetric current = _metrics;
            _metrics = _metrics.CreateNew();

            IEnumerable<KeyValuePair<string, object>> kpiResults = BuildKpi(current);
            OutputAction(kpiResults);
        }

        private static IEnumerable<KeyValuePair<string, object>> BuildKpi(IMetric metric)
        {
            string[] names = metric.ColumnNames;
            object[] values = metric.Values;

            for (int i = 0; i < names.Length; i++)
            {
                yield return new KeyValuePair<string, object>(names[i], values[i]);
            }
        }
    }
}