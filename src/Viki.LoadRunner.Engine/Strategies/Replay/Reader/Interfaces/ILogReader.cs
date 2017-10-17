namespace Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces
{
    public interface IDataReader<TLog>
    {
        void Start();

        LogItem<TLog> Next();

        void End();
    }
}