using Tools.DevConsole.Interfaces;
using System.Collections.Generic;

namespace Tools.DevConsole.Commands
{
    public class PrintCommand : BaseCommand
    {

        public PrintCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description =>
            "Print a string in console";
        
        public override string Example =>
            "print \"The quick brown fox jumps over the lazy dog\"";

        public override List<CommandArgument> Arguments => new()
        {
            new CommandArgument("test", false, "asd", typeof(string)),
        };

        public override List<CommandOption> Options => new()
        {
            new CommandOption('u', "upper", "print in uppercase", null),
            new CommandOption('l', "lower", "print in lowercase", null),
            new CommandOption('f', "float", "convert to float", typeof(float))
        };

        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            string str = args[""];

            if (opts.Contains("upper"))
                str = str.ToUpper();

            if (opts.Contains("lower"))
                str = str.ToLower();

            if (opts.Contains("float"))
                str = string.Format("{0:0.00}", opts["float"]);

            logger.Log(str);
            return true;
        }
    }
}
