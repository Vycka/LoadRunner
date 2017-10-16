using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Collector;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Presets;
using Viki.LoadRunner.Engine.Presets.Interfaces;
using Viki.LoadRunner.Engine.Utils;


namespace Viki.LoadRunner.Tools.Windows
{
    public partial class LoadRunnerUi : Form, IResultsAggregator
    {
        private readonly ICustomStrategySettings _settingsTemplate;
        public string TextTemplate = "LR-UI {0}";

        private readonly MetricMultiplexer _metricMultiplexerTemplate;
        private IMetric _metricMultiplexer;

        /// <summary>
        /// Exposed LoadRunnerEngine instance to give access to its status properties.
        /// 
        /// But It shouldn't be controlled from here, use UI buttons instead.
        /// </summary>
        private readonly LoadRunnerEngine _instance;
        private readonly CustomStrategy _strategy;

        private readonly ConcurrentQueue<IResult> _resultsQueue = new ConcurrentQueue<IResult>();

        

        public LoadRunnerUi(ICustomStrategySettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settingsTemplate = settings;

            _metricMultiplexerTemplate = new MetricMultiplexer(new IMetric[]
            {
                new FuncMultiMetric<int>((row, result) => 
                    result.Checkpoints.ForEach(c => row[c.Name] = (int)c.TimePoint.TotalMilliseconds),
                    () => default(int)
                ), 
                new CountMetric(),
                new ErrorCountMetric(),
                new TransactionsPerSecMetric()
            });

            _strategy = new CustomStrategy(settings.Clone().Add(this));
            _instance = new LoadRunnerEngine(_strategy);

            InitializeComponent();
        }

        void IResultsAggregator.Begin()
        {
            ResetStats();
            TestStartedDisableButtons();

            // Invoke forces this command to be executed on UI thread
            // This will allow BW ProcessChange to work properly.
            Invoke(new InvokeDelegate(() => _backgroundWorker1.RunWorkerAsync()));
        }

        private void ResetStats()
        {
            _metricMultiplexer = ((IMetric)_metricMultiplexerTemplate).CreateNew();
        }


        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            _resultsQueue.Enqueue(result);
        }

        void IResultsAggregator.End()
        {
            _backgroundWorker1.CancelAsync();

            TestEndedEnableButtons();
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Start?", "Start?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                _instance.RunAsync();

                TestStartedDisableButtons();
            }
        }

        private void TestStartedDisableButtons()
        {
            _startButton.Invoke(new InvokeDelegate(() => _startButton.Enabled = false));
            _validateButton.Invoke(new InvokeDelegate(() => _validateButton.Enabled = false));
            _stopButton.Invoke(new InvokeDelegate(() => _stopButton.Enabled = true));
        }

        private void TestEndedEnableButtons()
        {
            _startButton.Invoke(new InvokeDelegate(() => _startButton.Enabled = true));
            _validateButton.Invoke(new InvokeDelegate(() => _validateButton.Enabled = true));
            _stopButton.Invoke(new InvokeDelegate(() => _stopButton.Enabled = false));
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Stop?", "Stop?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
                Task.Run(() => _instance.Wait(TimeSpan.Zero, true));
        }

        private void _backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (_backgroundWorker1.CancellationPending == false)
            {
                _backgroundWorker1.ReportProgress(0);

                Thread.Sleep(1000);
            }

            _backgroundWorker1.ReportProgress(0);
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

        private void _validateButton_Click(object sender, EventArgs e)
        {
            IterationResult result = ScenarioValidator.Validate((IScenario) Activator.CreateInstance(_settingsTemplate.TestScenarioType));
            ICheckpoint checkpoint = result.Checkpoints.First(c => c.Name == Checkpoint.Names.IterationEnd);

            AppendMessage($"Validation OK: {checkpoint.TimePoint.TotalMilliseconds}ms.");
        }

        private void _clearButton_Click(object sender, EventArgs e)
        {
            ResetStats();
        }

        private void _backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            IResult result;

            while (_resultsQueue.TryDequeue(out result))
            {
                _metricMultiplexer.Add(result);

                foreach (ICheckpoint checkpoint in result.Checkpoints)
                {
                    if (checkpoint.Error != null)
                    {
                        AppendMessage($"{checkpoint.Name}\r\n{checkpoint.Error}");
                    }
                }
            }

            string jsonResult = JsonConvert.SerializeObject(GetData(), Formatting.Indented);
            resultsTextBox.Text = jsonResult;
            RefreshWindowTitle();
        }

        private int _errorIndex = 1;
        private void AppendMessage(string message)
        {
            string existingTextBoxValue = tbErrors.Text;
            if (existingTextBoxValue.Length > 10000)
            {
                existingTextBoxValue = existingTextBoxValue.Substring(0, 10000);
            }

            string newText = $"--- {DateTime.Now:O} #{_errorIndex++} ---\r\n{message}\r\n";
            newText += existingTextBoxValue;

            tbErrors.Text = newText;
        }

        private void LoadRunnerUi_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_instance.Running)
            {
                e.Cancel = true;

                MessageBox.Show("Can't close window when test is running. Stop it first");
            }
        }

        private void LoadRunnerUi_Shown(object sender, EventArgs e)
        {
            RefreshWindowTitle();
        }

        private void RefreshWindowTitle()
        {
            Text = string.Format(TextTemplate, _strategy.Timer.Value.ToString("g"));
        }
    }
}
