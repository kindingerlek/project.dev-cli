using Tools.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DevConsole.Commands
{
    public class HelpCommand : BaseCommand, ISuggestible
    {
        public HelpCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override List<CommandArgument> Arguments => new List<CommandArgument>()
        {
            new CommandArgument("", false, "The command you need help", typeof(string)),
        };

        public override string Description =>
            "Show the list of availables commands";
        
        public override string Example =>
            "help resetprefab";

        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            StringBuilder str = new StringBuilder();

            var commandList = DevConsole.CommandList
                .Where(x => !x.Value.IsSecretCommand)
                .OrderBy(x => x.Key)
                .ToDictionary(k => k.Key, v => v.Value);


            if (string.IsNullOrEmpty(args[""]))
            {
                var maxWidth = commandList
                    .Select(x => x.Key)
                    .OrderByDescending(x => x.Count())
                    .First()
                    .Length;

                foreach (var reg in commandList)
                    str.AppendLine($"{reg.Key.PadRight(maxWidth)} - {reg.Value.Description}");
            }
            else if (commandList.ContainsKey(args[""]))
                str.AppendLine(commandList[args[""]].Help);


            logger.Log(str.ToString());
            return true;
        }
        public string[] GetSuggestions(string[] commandParts)
        {
            return DevConsole.CommandList.Values
                .Select(x => x.CommandName)
                .Where(x => x.StartsWith(commandParts[1]))
                .ToArray();
        }
    }
}
