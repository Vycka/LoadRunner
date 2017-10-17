using System;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Reader
{
    public class LogItem<TLog>
    {
        TimeSpan TimeStamp { get; }

        TLog Value { get; }
    }
}