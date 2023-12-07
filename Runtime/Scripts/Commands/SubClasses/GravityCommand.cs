using System.Collections.Generic;
using Tools.DevConsole.Interfaces;
using UnityEngine;

namespace Tools.DevConsole.Commands
{
    public class GravityCommand : BaseCommand
    {
        public GravityCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description => "Get and set the gravity of game";

        public override List<CommandArgument> Arguments => new()
        {
            new CommandArgument(Name: "get",required: false, "returns the current value of gravity", null),
            new CommandArgument(Name: "set",required: false, "set the value of current gravity", typeof(float))
        };

        public override List<CommandOption> Options => new()
        {
        };


        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            if(args.Contains("set"))
            {
                Physics.gravity = Vector3.down * args["set"];
                logger.Log($"Enviroment gravity changed to: {-args["set"]}");
            }
            else
            {               
                logger.Log($"The value of environment gravity is: {Physics.gravity}");
            }

            return true;
        }
    }
}
