using System;

namespace Viki.LoadRunner.Engine.Presets.Replay.Reader.Interfaces
{
    public interface IDataReader<TLog>
    {
        void Start();

        LogItem<TLog> Next();

        void End();
    }
}