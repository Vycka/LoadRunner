using System;

namespace Viki.LoadRunner.Engine.Presets.Replay.Reader
{
    public class LogItem<TLog>
    {
        TimeSpan TimeStamp { get; }

        TLog Value { get; }
    }
}