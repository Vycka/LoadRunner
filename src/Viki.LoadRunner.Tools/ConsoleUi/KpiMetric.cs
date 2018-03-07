using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;

namespace Viki.LoadRunner.Tools.ConsoleUi
{
    public class KpiMetric : IMetric
    {
        private readonly IMetric[] _metricTemplates;
        private readonly TimeSpan _printPeriod;

        private readonly ExecutionTimer _timer;
        private TimeSpan _lastPrint;

        private IMetric _metrics;

        public Action<object> Output = (r) => Console.Out.WriteLine(JsonConvert.SerializeObject(r, Formatting.Indented));

        public KpiMetric(TimeSpan printPeriod, params IMetric[] metrics)
        {
            // We duplicate provided metrics at the start, so _metricTemplates won't be touched
            _metrics = ((IMetric)new MetricMultiplexer(metrics)).CreateNew();

            _metricTemplates = metrics;

            _printPeriod = printPeriod;

            _timer = new ExecutionTimer();
            _timer.Start();
            _lastPrint = TimeSpan.Zero;
        }

        public IMetric CreateNew()
        {
            return new KpiMetric(_printPeriod, _metricTemplates);
        }

        public void Add(IResult result)
        {
            _metrics.Add(result);

            TimeSpan currentTime = _timer.Value;
            if (currentTime > _lastPrint + _printPeriod)
            {
                _lastPrint = currentTime;

                Dictionary<string, object> kpiResults = new Dictionary<string, object>(BuildKpi().ToDictionary(kv => kv.Key, kv => kv.Value));

                Output(kpiResults);

                _metrics = _metrics.CreateNew();
            }
        }

        private IEnumerable<KeyValuePair<string, object>> BuildKpi()
        {
            string[] names = ColumnNames;
            object[] values = Values;

            for (int i = 0; i < ColumnNames.Length; i++)
            {
                yield return new KeyValuePair<string, object>(names[i], values[i]);
            }
        }

        public string[] ColumnNames => _metrics.ColumnNames;
        public object[] Values => _metrics.Values;
    }
}