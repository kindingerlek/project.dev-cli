using NUnit.Framework;
using Tools.DevConsole.Interfaces;

namespace Tools.DevConsole.Tests
{
    public class BaseDevConsoleTests
    {
        protected IConsoleLog logger;

        public BaseDevConsoleTests()
        {
            logger = new ConsoleLog(MessageType.LOG);
            DevConsole.Logger = logger;
        }

        [Test]
        public void NoCommandCouldBeFound_SHOULD_LogAnError()
        {
            // ARRANGE
            string command = "non_existent test";
            string message = "";

            // PERFORM
            DevConsole.RunCommand(command);
            message = logger.LogHistory.Dequeue().Text;

            // ASSERT
            Assert.IsTrue(message.StartsWith("'non_existent' is an unknown command, type 'help' for list"));
        }
    }
}
