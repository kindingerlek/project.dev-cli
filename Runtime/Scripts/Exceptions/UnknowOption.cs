using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DevConsole.Exceptions
{
    public class UnknowOption : DevConsoleException
    {
        public UnknowOption(string message) : base(message)
        {

        }
    }
}
