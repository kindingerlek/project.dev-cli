using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DevConsole.Interfaces;
using UnityEngine.SceneManagement;

namespace Tools.DevConsole.Commands
{
    public class ReloadCommand : BaseCommand
    {
        public ReloadCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Description =>
            "Reload game.";

        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            logger.Debug($"Reloading the scene: {SceneManager.GetActiveScene().name}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            logger.Debug($"Scene reloaded!");
            return true;
        }
    }
}
