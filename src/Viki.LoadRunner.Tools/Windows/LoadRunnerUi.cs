using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Parameters;

namespace Viki.LoadRunner.Tools.Windows
{
    public partial class LoadRunnerUi : Form, IResultsAggregator
    {
        private readonly MetricMultiplexer _metricMultiplexerTemplate;
        private IMetric _metricMultiplexer;
        private readonly LoadRunnerEngine _loadRunnerEngine;

        /// <summary>
        /// Initializes new executor instance
        /// </summary>
        /// <typeparam name="TTestScenario">ILoadTestScenario to be executed object type</typeparam>
        /// <param name="parameters">LoadTest parameters</param>
        /// <param name="resultsAggregators">Aggregators to use when aggregating results from all iterations</param>
        /// <returns></returns>
        public static LoadRunnerUi Create<TTestScenario>(LoadRunnerParameters parameters, params IResultsAggregator[] resultsAggregators)
            where TTestScenario : ILoadTestScenario
        {
            LoadRunnerUi ui = new LoadRunnerUi(parameters, typeof(TTestScenario), resultsAggregators);

            return ui;
        }

        private LoadRunnerUi(LoadRunnerParameters parameters, Type iTestScenarioType, IResultsAggregator[] resultsAggregators)
        {
            _metricMultiplexerTemplate = new MetricMultiplexer(new IMetric[]
            {
                new PercentileMetric(0.5, 0.95, 0.99),
                new CountMetric(),
                new ErrorCountMetric(),
                new TransactionsPerSecMetric()
            });

            _loadRunnerEngine = new LoadRunnerEngine(parameters, iTestScenarioType, resultsAggregators.Concat(new [] { this }).ToArray());

            InitializeComponent();
        }

        void IResultsAggregator.Begin()
        {
            _startButton.Invoke(new InvokeDelegate(() => _startButton.Enabled = false));
            _stopButton.Invoke(new InvokeDelegate(() => _stopButton.Enabled = true));

            _metricMultiplexer = ((IMetric)_metricMultiplexerTemplate).CreateNew();

            _backgroundWorker1.RunWorkerAsync();
        }

        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            _metricMultiplexer.Add(result);
        }

        void IResultsAggregator.End()
        {
            _backgroundWorker1.CancelAsync();

            _startButton.Invoke(new InvokeDelegate(() => _startButton.Enabled = true));
            _stopButton.Invoke(new InvokeDelegate(() => _stopButton.Enabled = false));
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            _loadRunnerEngine.RunAsync();
        }

        private void _backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (_backgroundWorker1.CancellationPending == false)
            {
                string jsonResult = JsonConvert.SerializeObject(GetData(), Formatting.Indented);

                resultsTextBox.Invoke(new InvokeDelegate(() => resultsTextBox.Text = jsonResult));

                Thread.Sleep(1000);
            }
        }

        private IDictionary<string, object> GetData()
        {
            string[] labels = _metricMultiplexer.ColumnNames;
            object[] values = _metricMultiplexer.Values;

            Dictionary<string, object> dictionary = new Dictionary<string, object>(labels.Length);

            for (int i = 0; i < labels.Length; i++)
            {
                dictionary.Add(labels[i], values[i]);
            }

            return dictionary;
        }

        private delegate void InvokeDelegate();

        private void stopButton_Click(object sender, EventArgs e)
        {
            Task.Run(() => _loadRunnerEngine.Wait(TimeSpan.Zero, true));
        }
    }
}
