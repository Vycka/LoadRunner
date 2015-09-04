using System.Threading;
using System.Windows.Forms;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Tools.Aggregators
{
    public class HistogramResultsAggregatorUi : IResultsAggregator
    {
        private readonly TimelineWindow _totalsWindow = new TimelineWindow();
        private Thread _windowThread;

        public void TestContextResultReceived(TestContextResult result)
        {
        }

        public void Begin()
        {
            _windowThread = new Thread((() => Application.Run(_totalsWindow)));
            _windowThread.Start();
        }

        public void End()
        {
            _windowThread.Join();
        }
    }
}