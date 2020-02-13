using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DevConsole.Interfaces
{
    public interface IConsoleLog
    {
        MessageType MaxLogLevel { get; set; }
        Queue<IConsoleLogMessage> LogHistory { get; }
        int MaxLogRegister { get; set; }
        bool UseUnityLogCallback { get; set; }

        event ClearHandler OnClear;
        event ReceiveMessageHandler OnReceiveMessage;

        void Log(string message);
        void Debug(string message);
        void Warning(string message);
        void Error(string message);
        void Clear();
    }
}