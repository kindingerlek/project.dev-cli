using Tools.DevConsole.Interfaces;

namespace Tools.DevConsole.Commands
{
    public class ClearCommand : BaseCommand
    {
        public ClearCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description =>
            "Clear log";

        public override string Alias =>
            "cls";

        protected override bool Handle(ArgumentsList args, OptionsList opts)        
        {
            logger.Clear();
            return true;
        }
    }
}
