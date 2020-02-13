using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DevConsole;
using Tools.DevConsole.Commands;
using Tools.DevConsole.Interfaces;
using UnityEngine;

namespace DevConsole.Commands.SubClasses
{
    public class TimeScaleCommand : BaseCommand
    {
        public TimeScaleCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description => "Get and set the time scale of game";

        public override List<CommandArgument> Arguments => new List<CommandArgument>()
        {
            new CommandArgument(Name: "get",required: false, "returns the current value of time scale", null),
            new CommandArgument(Name: "set",required: false, "set the value of time scale", typeof(float))
        };


        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            if(args.Contains("set"))
            {
                logger.Log($"TimeScale change from {Time.timeScale} to: {args["set"]}");
                Time.timeScale = args["set"];
            }
            else
            {
                logger.Log($"The current timescale of game is: {Time.timeScale}");
            }

            return true;
        }
    }
}
