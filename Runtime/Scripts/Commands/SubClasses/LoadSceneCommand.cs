using System.Collections.Generic;
using Tools.DevConsole.Interfaces;
using UnityEngine.SceneManagement;

namespace Tools.DevConsole.Commands
{
    public class LoadSceneCommand : BaseCommand
    {
        public override List<CommandArgument> Arguments => new()
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
