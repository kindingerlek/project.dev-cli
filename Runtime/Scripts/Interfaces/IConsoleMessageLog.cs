using System;

namespace Tools.DevConsole.Interfaces
{
    public interface IConsoleLogMessage
    {
        MessageType Type { get; }
        string Text { get; }
        DateTime DateTime { get; }
    }
}
