using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// StreamAggregator provides loadtest raw/masterdata (IResult) IEnumerable stream 
    /// </summary>
    public abstract class StreamAggregatorBase : IAggregator, IDisposable
    {
        #region Fields

        private BlockingCollectionSingle<IResult> _queue;
        private Task _writerTask;

        #endregion

        protected abstract void Process(IEnumerable<IResult> stream);

        #region IResultsAggregator

        public void Begin()
         {
            _queue = new BlockingCollectionSingle<IResult>();

            _writerTask = new Task(() => Process(_queue), TaskCreationOptions.LongRunning);
            _writerTask.Start();
        }

        public void Aggregate(IResult result)
        {
            AssertTask();

            _queue.Add(result);
        }

        public void End()
        {
            _queue.CompleteAdding();
            
            _writerTask.Wait();

            AssertTask();
        }

        #endregion

        #region IDisposable

        ~StreamAggregatorBase()
        {
            Dispose();
        }

        public void Dispose()
        {
            ((IAggregator)this).End();
        }

        #endregion

        #region Misc

        private void AssertTask()
        {
            if (_writerTask.Exception != null)
                throw _writerTask.Exception;
        }

        #endregion
    }
    

}