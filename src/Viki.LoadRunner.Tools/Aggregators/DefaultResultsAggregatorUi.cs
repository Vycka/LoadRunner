//using System.Threading;
//using System.Windows.Forms;
//using Viki.LoadRunner.Engine.Aggregators;
//using Viki.LoadRunner.Engine.Executor.Result;
//using Viki.LoadRunner.Tools.Windows;

//namespace Viki.LoadRunner.Tools.Aggregators
//{
//    public class DefaultResultsAggregatorUi : IResultsAggregator
//    {
//        private readonly TotalsWindow _totalsWindow = new TotalsWindow();
//        private Thread _windowThread;

//        public void TestContextResultReceived(IResult result)
//        {
//        }



//        public void Begin()
//        {
//            _windowThread = new Thread((() => Application.Run(_totalsWindow)));
//            _windowThread.Start();
//        }

//        public void End()
//        {
//            _windowThread.Join();
//        }
//    }
//}