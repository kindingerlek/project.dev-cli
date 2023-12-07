using System;
using Tools.DevConsole.Interfaces;

namespace Tools.DevConsole
{
    internal class ConsoleLogMessage : IConsoleLogMessage
    {
        public MessageType Type { get; }
        public string Text { get;  }
        public DateTime DateTime { get;  }

        public ConsoleLogMessage(MessageType type, string message)
        {
            Type = type;
            Text = message;
            DateTime = DateTime.Now;
        }
    }
}
