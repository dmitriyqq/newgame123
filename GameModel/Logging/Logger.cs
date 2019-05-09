using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameModel
{
    public enum MessageType
    {
        Info, Warning, Error,
    }

    public class Logger
    {
        private string scope;

        public Logger(string scope)
        {
            this.scope = scope;
        }

        private Queue<LogMessage> logs = new Queue<LogMessage>();

        private void Log(MessageType type, string message, Exception exception = null)
        {
            
            var frame = new StackTrace().GetFrame(2);
            var caller = $"{frame.GetMethod().ReflectedType.Name}.{frame.GetMethod().Name}";
                
            var log = new LogMessage(scope, type, caller, message, exception);
            OnNewMessage?.Invoke(log);
            logs.Enqueue(log);
        }

        public void Info(string message)
        {
            Log(MessageType.Info, message);
        }
        
        public void Warning(string message)
        {
            Log(MessageType.Warning, message);
        }
        
        public void Error(string message)
        {
            Log(MessageType.Error, message);
        }
        
        public void Error(Exception exception)
        {
            Log(MessageType.Error, exception.Message, exception);
        }

        public event Action<LogMessage> OnNewMessage;

        public IEnumerable<LogMessage> oldMessages => logs;
    }
}