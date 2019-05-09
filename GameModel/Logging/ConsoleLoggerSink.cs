using System;

namespace GameModel
{
    public class ConsoleLoggerSink : ILoggerSink
    {
        public void Subscribe(Logger logger)
        {
            foreach (var msg in logger.oldMessages)
            {
                HandleMessage(msg);
            }

            logger.OnNewMessage += HandleMessage;
        }

        private void HandleMessage(LogMessage message)
        {
            Console.WriteLine(message.formattedMessage);
        }
    }
}