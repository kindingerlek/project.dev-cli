using System;

namespace Tools.DevConsole.Exceptions
{
    public class DevConsoleException : Exception
    {
        public DevConsoleException()
        {
        }

        public DevConsoleException(string message) : base(message)
        {
            message = $"DevConsoleException: {message}";
        }
    }
}
