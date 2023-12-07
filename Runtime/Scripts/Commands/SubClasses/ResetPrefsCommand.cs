using System.Collections.Generic;
using Tools.DevConsole.Interfaces;
using UnityEngine;

namespace Tools.DevConsole.Commands
{
    public class ResetPrefsCommand : BaseCommand
    {
        public ResetPrefsCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description =>
            "Reset & saves PlayerPrefs.";
        
        public override List<CommandArgument> Arguments => new()
        {
            new CommandArgument("all",false,"Clear all keys in PlayerPrefs",null),
            new CommandArgument("key",false,"Clear specific key in PlayerPrefs",typeof(string))
        };

        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            logger.Debug("Reseting player prefs.");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            logger.Debug("Player Prefs cleaned!");
            return true;
        }
    }
}
