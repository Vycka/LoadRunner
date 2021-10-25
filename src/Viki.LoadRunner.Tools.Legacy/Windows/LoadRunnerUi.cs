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
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Utils;
using Viki.LoadRunner.Engine.Validators;


namespace Viki.LoadRunner.Tools.Windows
{
    public partial class LoadRunnerUi : Form, IAggregator, IStrategyExecutor
    {
        public string TextTemplate = "LR-UI {0}";

        private MetricsTemplate<IResult> _metricsTemplate;
        private IMetric<IResult> _metrics;

        private readonly ExecutionTimer _timer;
        private readonly ConcurrentQueue<IResult> _resultsQueue = new ConcurrentQueue<IResult>();

        private IValidator _validator;
        private LoadRunnerEngine _engine;

        public LoadRunnerUi()
        {
            _timer = new ExecutionTimer();

            _metricsTemplate = new MetricsTemplate<IResult>(new IMetric[]
            {
                new FuncMultiMetric<int>((row, result) => 
                    result.Checkpoints.ForEach(c => row[c.Name] = (int)c.TimePoint.TotalMilliseconds),
                    () => default(int)
                ), 
                new CountMetric(),
                new ErrorCountMetric(),
                new TransactionsPerSecMetric()
            });

            InitializeComponent();
        }

        public void Setup(IStrategy strategy, IValidator validator)
        {
            _engine = new LoadRunnerEngine(strategy);
            _engine.Started += EngineOnStarted;
            _engine.Stopped += EngineOnStopped;

            _validateButton.Enabled = validator != null;
            _validator = validator;
        }

        public void Run()
        {
            Application.Run(this);
        }

        public event ExecutorStartedEventDelegate Started;

        public event ExecutorStoppedEventDelegate Stopped;

        private void EngineOnStarted(IStrategyExecutor sender, ITestState state)
        {
            Started?.Invoke(sender, _engine.State);
        }

        private void EngineOnStopped(IStrategyExecutor sender, Exception exception)
        {
            Stopped?.Invoke(sender, exception);
        }

        void IAggregator.Begin()
        {
            _timer.Start();

            ResetStats();
            TestStartedDisableButtons();

            // Invoke forces this command to be executed on UI thread
            // This will allow BW ProcessChange to work properly.
            Invoke(new InvokeDelegate(() => _backgroundWorker1.RunWorkerAsync()));
        }

        private void ResetStats()
        {
            _metrics = _metricsTemplate.Create();
        }


        void IAggregator.Aggregate(IResult result)
        {
            _resultsQueue.Enqueue(result);
        }

        void IAggregator.End()
        {
            _timer.Stop();

            _backgroundWorker1.CancelAsync();

            TestEndedEnableButtons();
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Start?", "Start?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                _engine.RunAsync();

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
                Task.Run(() => _engine.CancelAsync());
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

            string[] labels = _metrics.ColumnNames;
            object[] values = _metrics.Values;

            Dictionary<string, object> dictionary = new Dictionary<string, object>(labels.Length);

            for (int i = 0; i < labels.Length; i++)
            {
                dictionary.Add(labels[i], values[i]);
            }

            return dictionary;
        }

        private delegate void InvokeDelegate();

        private async void _validateButton_Click(object sender, EventArgs e)
        {
            IterationResult result = await Task.Run(() => _validator.Validate(0)).ConfigureAwait(false);
           

            Invoke(new InvokeDelegate(
                () => AppendMessage($"Validation OK:\r\n{String.Join("\r\n", Process(result).Select(c => $"{c.Item1}->{c.Item2}: {c.Item3}{(c.Item4 != null ? $"\r\n{c.Item4.ToString()}\r\n" : "")}"))}"))
            );
        }

        private static IEnumerable<Tuple<string, string, TimeSpan, object>> Process(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length - 1; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                ICheckpoint nextCheckpoint = checkpoints[i + 1];
                TimeSpan momentDiff = checkpoint.Diff(nextCheckpoint);

                yield return new Tuple<string, string, TimeSpan, object>(checkpoint.Name, nextCheckpoint.Name, momentDiff, checkpoint.Error);
            }
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
                _metrics.Add(result);

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
            if (_engine.Running)
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
            Text = string.Format(TextTemplate, _timer.Value.ToString("g"));
        }
    }
}
