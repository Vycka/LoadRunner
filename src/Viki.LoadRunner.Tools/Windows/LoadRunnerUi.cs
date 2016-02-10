using System;
using System.Linq;
using System.Windows.Forms;
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
        private MetricMultiplexer _metricMultiplexer;

        private readonly IMetric[] _metricTemplates =
        {
            new PercentileMetric(0.5, 0.95, 0.99),
            new CountMetric(),
            new ErrorCountMetric(),
            new TransactionsPerSecMetric()
        };

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
            _loadRunnerEngine = new LoadRunnerEngine(parameters, iTestScenarioType, resultsAggregators.Concat(new [] { this }).ToArray());

            InitializeComponent();
        }

        void IResultsAggregator.Begin()
        {
            _startButton.Enabled = false;

            _metricMultiplexer = new MetricMultiplexer(_metricTemplates);
        }

        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            _metricMultiplexer.Add(result);
        }

        void IResultsAggregator.End()
        {
            _startButton.Enabled = true;
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            _loadRunnerEngine.RunAsync();
            
        }
    }
}
