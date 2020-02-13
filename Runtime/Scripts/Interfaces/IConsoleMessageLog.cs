using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DevConsole.Interfaces
{
    public interface IConsoleLogMessage
    {
        MessageType Type { get; }
        string Text { get; }
        DateTime DateTime { get; }
    }
}
