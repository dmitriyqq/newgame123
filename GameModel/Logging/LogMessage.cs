using System;
using System.Collections.Generic;

namespace GameModel
{
    public class LogMessage
    {
        public string Scope;

        public MessageType MessageType;

        public string RawMessage;

        public string Caller;

        public Exception e;

        public DateTime Time;

        public LogMessage(string scope, MessageType messageType, string caller, string rawMessage, Exception e = null)
        {
            Scope = scope;
            MessageType = messageType;
            RawMessage = rawMessage;
            Time = DateTime.Now;
            Caller = caller;
        }
        
        public string formattedMessage => $"[{Scope}][{MessageType}][{Caller}][{Time.ToShortTimeString()}]:{e?.Message ?? RawMessage}";
    }
}