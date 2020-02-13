using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tools.DevConsole.Commands;
using Tools.DevConsole.Exceptions;

namespace Tools.DevConsole
{
    public class ArgumentsList
    {

        public List<CommandArgument> Arguments { get; private set; }

        private Dictionary<string, (string value, Type type)> commandArguments;

        public ArgumentsList(string[] commandParts, List<CommandArgument> arguments)
        {
            Arguments = arguments;

            if (Arguments == null)
                Arguments = new List<CommandArgument>();

            //if ((Arguments == null || Arguments.Count == 0) && Defau
            //    throw new Exception($"There are no additional arguments for this command");

            commandArguments = GetArguments(commandParts);
        }

        public dynamic this[string key]
        {
            get
            {
                return Helper.Parse(commandArguments[key].value, commandArguments[key].type);
            }
        }

        public bool Contains(string key)
        {
            return commandArguments.ContainsKey(key);
        }

        public bool TryGetValue(string key, out dynamic value)
        {
            value = null;
            if (!commandArguments.ContainsKey(key))
                return false;

            value = commandArguments[key];
            return true;
        }

        private Dictionary<string, (string value, Type type)> GetArguments(string[] commandParts)
        {
            var argList = new Dictionary<string, (string value, Type type)>();

            argList[""] = (value: null, type: typeof(string));

            // Para cada parte do comando...
            for (int i = 0; i < commandParts.Length; i++)
            {
                if (Helper.IsAnOption(commandParts[i]))
                    throw new Exception($"Something goes wrong. Please check your command");

                var commandPart = commandParts[i];
                var arg = Arguments.FirstOrDefault(x => x.Name == commandPart);

                if (arg == null)
                {
                    if(i != commandParts.Length - 1)
                        throw new UnknowArgument($"There are no argument with '{commandPart}' for this command");

                    argList[""] = (commandPart, typeof(string));
                    continue;
                }

                if (commandParts.ElementAtOrDefault(i + 1) == null && arg.ParameterType != null)
                    throw new MissingParameter($"The given argument '{arg.Name}' requires a parameter and no one has provided");
                
                var value = arg.ParameterType == null ? null : commandParts[++i];
                argList[arg.Name] = (value: value, type: arg.ParameterType);
            }

            var requiredArg = Arguments
                                    .Where(x => x.Required)
                                    .Select(x => x.Name)
                                    .Intersect(argList.Keys)
                                    .ToList();

            if (Arguments != null && Arguments.Where(x => x.Required).Any() && !requiredArg.Any())
                throw new MissingRequiredArgument("There are missing required arguments!");

            return argList;
        }
    }
}
