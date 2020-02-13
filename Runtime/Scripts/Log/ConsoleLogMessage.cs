using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DevConsole.Interfaces;
using UnityEngine;

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
