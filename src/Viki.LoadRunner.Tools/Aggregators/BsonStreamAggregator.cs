namespace Viki.LoadRunner.Tools.Aggregators
{
    //public class BsonStreamAggregator : FileStreamAggregatorBase
    //{
    //    #region Constructors

    //    public BsonStreamAggregator(string jsonOutputfile) : base(jsonOutputfile)
    //    {
    //    }

    //    public BsonStreamAggregator(Func<string> dynamicJsonOutputFile) : base(dynamicJsonOutputFile)
    //    {
    //    }

    //    #endregion

    //    #region FileStreamAggregatorBase Write()

    //    public override void Write(string filePath, IEnumerable<IResult> stream)
    //    {
    //        stream.SerializeToBson(filePath);
    //    }

    //    #endregion

    //    #region Replay functions

    //    public static void Replay(string jsonResultsFile, params IAggregator[] targetAggregators)
    //    {
    //        Replay<object>(jsonResultsFile, targetAggregators);
    //    }

    //    public static void Replay<TUserData>(string jsonResultsFile, params IAggregator[] targetAggregators)
    //    {
    //        IEnumerable<IResult> resultsStream = Load<TUserData>(jsonResultsFile);

    //        StreamAggregator.Replay(resultsStream, targetAggregators);
    //    }

    //    public static IEnumerable<ReplayResult<TUserData>> Load<TUserData>(string jsonResultsFile)
    //    {
    //        return JsonStream.DeserializeFromBson<ReplayResult<TUserData>>(jsonResultsFile);
    //    }

    //    #endregion
    //}
}