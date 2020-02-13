using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DevConsole.Exceptions
{
    public class ExceededOptions : DevConsoleException
    {
        public ExceededOptions(string message) : base(message)
        {

        }
    }
}
