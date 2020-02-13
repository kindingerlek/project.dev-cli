using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DevConsole.Interfaces;
using UnityEngine.SceneManagement;

namespace Tools.DevConsole.Commands.SubClasses
{
    public class LoadSceneCommand : BaseCommand
    {
        public override List<CommandArgument> Arguments => new List<CommandArgument>()
        {
            new CommandArgument("sceneName", true, "The name of the scene you want load", typeof(string))
        };

        public LoadSceneCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description => "Load a scene by name";

        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            logger.Log($"Loading scene {args["sceneName"]}");
            SceneManager.LoadScene(args["sceneName"]);

            return true;
        }
    }
}
