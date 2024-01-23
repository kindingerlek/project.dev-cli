using System.Collections.Generic;
using Tools.DevConsole.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.DevConsole.Commands
{
    public class ExitCommand : BaseCommand
    {
        public ExitCommand(IConsoleLog logger) : base(logger)
        {
        }

        public override string Alias => "quit";

        public override string Description => "Exits the application";

        protected override bool Handle(ArgumentsList args, OptionsList opts)
        {
            logger.Log("Exiting application...");

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
            return true;
        }
    }
}
