namespace GameModel
{
    public interface ILoggerSink
    {
        void Subscribe(Logger logger);
    }
}