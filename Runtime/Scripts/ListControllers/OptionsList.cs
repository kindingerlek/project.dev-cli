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
    public class OptionsList
    {
        private List<CommandOption> Options { get; set; }

        private Dictionary<string, (string value, Type type)> commandOptions;

        public OptionsList(string[] commandParts, List<CommandOption> availablesOptions, out string[] remain)
        {
            Options = availablesOptions;

            if (Options == null)
                Options = new List<CommandOption>();

            commandOptions = GetOptions(commandParts, out remain);
        }

        public int Count { get => commandOptions.Count; }

        public dynamic this[string key]
        {
            get
            {
                if (!commandOptions.ContainsKey(key))
                    throw new UnknowOption($"The command entered do not known option '{key}'. Make sure the check if the option is present in list, before used it!");

                if (commandOptions[key].type == null)
                    return true;

                return Helper.Parse(commandOptions[key].value, commandOptions[key].type);
            }
        }

        public bool Contains(string key)
        {
            return commandOptions.ContainsKey(key);
        }

        public bool TryGetValue(string key, out dynamic value)
        {
            value = null;
            if (!commandOptions.ContainsKey(key))
                return false;

            value = commandOptions[key];
            return true;
        }

        private Dictionary<string, (string value, Type type)> GetOptions(string[] commandParts, out string[] remain)
        {
            var optionsList = new Dictionary<string, (string value, Type type)>();
            remain = new string[0];

            var temp = new List<string>();

            // Para cada parte do comando...
            for (int i = 0; i < commandParts.Length; i++)
            {
                var commandPart = commandParts[i];

                if (!Helper.IsAnOption(commandPart))
                {
                    temp.Add(commandPart);
                    continue;
                }

                var isOptionGroup = IsAnOptionsGroup(commandPart);
                commandPart = commandPart.TrimStart('-');


                // An option is passed, but command do not has one
                if (Options == null || !Options.Any())
                    throw new ExceededOptions($"There no option for this command");

                // Busca a Option relativa a parte do comando lido
                var locatedOptions =
                        Options.Where(x =>
                        {
                            if (isOptionGroup)
                                return commandPart.Contains(x.Alias.ToString());
                            return x.Name == commandPart;
                        }).Reverse();

                // Caso não exista Option cadastrada para a parte do comando, lança uma exceção
                if (!locatedOptions.Any())
                    throw new UnknowOption($"There no option with '{commandPart}' for this command");

                foreach (var option in locatedOptions)
                {
                    if (commandParts.ElementAtOrDefault(i+1) == null && option.ParameterType != null)
                        throw new MissingParameter($"The given option '{option.Name}' requires a parameter and no one has provided");

                    // Adiciona a Option para a lista de opções disponíveis

                    var value = option.ParameterType == null ? null : commandParts[++i];
                    optionsList[option.Name] = (value: value, type: option.ParameterType);
                }
            }

            remain = temp.ToArray();
            return optionsList;
        }


        private bool IsAnOptionsGroup(string value)
        {
            return Helper.IsAnOption(value) && Regex.IsMatch(value, @"^-\w+");
        }
    }
}
