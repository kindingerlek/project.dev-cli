using Utils.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.DevConsole.Interfaces;

namespace Tools.DevConsole.Commands
{
    public abstract class BaseCommand
    {
        protected IConsoleLog logger;
        public BaseCommand(IConsoleLog logger)
        {
            this.logger = logger;
        }

        #region MandatoryProperties
        public abstract string Description { get; }        
        #endregion

        #region OptionalProperties
        public virtual bool IsSecretCommand => false;
        public virtual string CommandName => this.GetType().Name.Replace("Command", "").ToUnderscoreCase();
        public virtual string Alias => null;
        public virtual string Example => null;
        public virtual List<CommandOption> Options => null;        
        public virtual List<CommandArgument> Arguments => null;
        #endregion
        
        public string Usage
        {
            get
            {
                var str = $"{CommandName} ";

                if (Arguments != null)
                {
                    foreach (var arg in Arguments.Where(x => x.Required))
                        str += $"{arg.GetFormatedName()} ";
                }

                str += "[<String>]";

                return str.ToString();
            }
        }

        public string Help
        {
            get
            {
                StringBuilder str = new();

                str.AppendLine($"NAME:");
                str.AppendLine($"\t{CommandName} - {Description}");

                str.AppendLine($"\nUSAGE:");
                str.AppendLine($"\t{Usage}");
                
                if(Arguments != null)
                {
                    str.AppendLine($"\nARGUMENTS:");
                    foreach(var arg in Arguments)
                    {
                        str.AppendLine($"\t{arg}");
                    }
                }

                if (Options != null)
                {
                    str.AppendLine($"\nOPTIONS:");
                    foreach(var opt in Options)
                    {
                        str.AppendLine($"\t{opt}");
                    }
                }
                if (Example != null)
                {
                    str.AppendLine($"\nEXAMPLE:");
                    str.AppendLine($"\t{Example}");
                }
                return $"<color=#aaaaaa>{str}</color>";
            }
        }

        public bool Execute(string[] commandParts)
        {
            if (Arguments != null && Arguments.Where(x => x.Required).Any() && (commandParts == null || commandParts.Length == 0))
                throw new Exception($"No argument was provided! Type 'help commandName' to see available arguments.");

            OptionsList optList = new(commandParts, Options, out string[] remain);
            ArgumentsList argList = new(remain, Arguments);
            return Handle(argList, optList);
        }

        protected abstract bool Handle(ArgumentsList args, OptionsList opts);
    }
}
