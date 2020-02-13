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
    public class GravityCommand : BaseCommand
    {
        public GravityCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description => "Get and set the gravity of game";

        public override List<CommandArgument> Arguments => new List<CommandArgument>()
        {
            new CommandArgument(Name: "get",required: false, "returns the current value of gravity", null),
            new CommandArgument(Name: "set",required: false, "set the value of current gravity", typeof(float))
        };

        public override List<CommandOption> Options => new List<CommandOption>()
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
