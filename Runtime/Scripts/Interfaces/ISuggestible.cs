using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DevConsole.Interfaces
{
    public interface ISuggestible
    {
        string[] GetSuggestions(string[] commandParts);
    }
}
