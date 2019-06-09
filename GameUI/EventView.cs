using System.Drawing;
using GameModel;
using Gwen.Control;

namespace GameUI
{
    public class EventView : ListBox, ILoggerSink
    {
        private int MaxMessages = 20;
        
        public EventView(Base parent) : base(parent)
        {
        }

        
        
        public void Subscribe(Logger logger)
        {
            foreach (var message in logger.oldMessages)
            {
                HandleMessage(message);
            }

            logger.OnNewMessage += HandleMessage;
        }

        public void HandleMessage(LogMessage message)
        {

            if (RowCount > MaxMessages)
            {
                RemoveRow(0);
            }

            var row = AddRow(message.FormattedMessage);
            row.SetTextColor(getMessageColor(message.MessageType));
            ScrollToBottom();
        }
        
        private Color getMessageColor(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    return Color.Lime;
                case MessageType.Warning:
                    return Color.Gold;
                case MessageType.Error:
                    return Color.Red;
                default: return Color.Black;
            }
        }
    }
}