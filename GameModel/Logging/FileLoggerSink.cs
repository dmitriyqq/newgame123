using System.IO;

namespace GameModel
{
    public class FileLoggerSink : ILoggerSink
    {
        private readonly StreamWriter _writer;
        private readonly object _lock = new object();
        public FileLoggerSink(string path)
        {
            _writer = File.CreateText(path);
        }
        
        public void Subscribe(Logger logger)
        {
            lock (_lock)
            {
                foreach (var message in logger.oldMessages)
                {
                    _writer.WriteLine(message.FormattedMessage);
                }

                logger.OnNewMessage += WriteLog;
            }
        }

        private void WriteLog(LogMessage message)
        {
            lock (_lock)
            {
                _writer.WriteLine(message.FormattedMessage);
                _writer.Flush();
            }
        }
    }
}