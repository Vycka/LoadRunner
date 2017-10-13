//using System;
//using Viki.LoadRunner.Engine.Aggregators;
//using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
//using Viki.LoadRunner.Engine.Executor.Timer;
//using Viki.LoadRunner.Engine.Strategies;

//namespace Viki.LoadRunner.Engine.Executor.Threads
//{
//    public class ThreadPoolContext : IThreadPoolContext
//    {
//        public IUniqueIdFactory<int> IdFactory { get; set; }
//        public ISpeedStrategy Speed { get; set; }
//        public object UserData { get; set; }
//        public Type Scenario { get; set; }
//        public IResultsAggregator Aggregator { get; set; }

//        IThreadPoolStats IThreadPoolContext.ThreadPool => this.ThreadPool;

//        public IThreadPool ThreadPool;

//        public ITimer Timer { get; set; }
//    }
//}