using System.Collections.Generic;
using GameModel;

namespace GameUI
{
    public class LoggerManager
    {
        private readonly List<Logger> _loggers;
        private readonly List<ILoggerSink> _sinks;
        
        public LoggerManager(List<ILoggerSink> sinks, List<Logger> loggers)
        {
            _sinks = sinks;
            _loggers = loggers;

            foreach (var logger in _loggers)
            {
                SubscribeToLogger(logger);
            }
        }

        public void AddSink(ILoggerSink sink)
        {
            _sinks.Add(sink);
            foreach (var logger in _loggers)
            {
                sink.Subscribe(logger);
            }
        }

        public void AddLogger(Logger logger)
        {
            _loggers.Add(logger);
            SubscribeToLogger(logger);
        }

        private void SubscribeToLogger(Logger logger)
        {
            foreach (var sink in _sinks)
            {
                sink.Subscribe(logger);
            }
        }
    }
}