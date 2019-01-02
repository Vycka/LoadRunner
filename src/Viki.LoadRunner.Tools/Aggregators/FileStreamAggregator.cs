namespace Viki.LoadRunner.Tools.Aggregators
{
    //public abstract class FileStreamAggregator : StreamAggregator
    //{
    //    #region Fields

    //    private readonly Func<string> _outFileNameFunc;

    //    #endregion

    //    #region Constructors

    //    protected FileStreamAggregator(string filePath) 
    //        : this(() => filePath)
    //    {
    //    }

    //    protected FileStreamAggregator(Func<string> filePathSelector)
    //      : base(results => Write( results, filePathSelector))
    //    {
    //        _outFileNameFunc = filePathSelector;
    //    }

    //    #endregion

    //    private static void Write(FileStreamAggregator aggregator, IEnumerable<IResult> results, Func<string> filePathSelector)
    //    {
    //        string filePath = filePathSelector();

    //        aggregator.Write(results, filePath);
    //    }

    //    #region Default write function

    //    private void StreamWriterFunction(IEnumerable<IResult> results)
    //    {
    //        string fileName = _outFileNameFunc();

    //        results.SerializeToBson(fileName);
    //    }

    //    #endregion

    //    #region Read/Write abstracts

    //    protected abstract IEnumerable<IResult> Read(string filePath);

    //    protected abstract void Write(IEnumerable<IResult> items, string filePath);

    //    #endregion
    //}
}