using Utils.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tools.DevConsole.Commands;
using Tools.DevConsole.Exceptions;
using Tools.DevConsole.Interfaces;

namespace Tools.DevConsole
{
    public static class DevConsole 
    {
        private static string repeatCmdName = "!!";
        private static int maxCommandHistory = 50;
        private static Dictionary<string, BaseCommand> _commandList;
        public static IConsoleLog Logger { get; set; }
        internal static Queue<string> CommandHistory { get; private set; } = new Queue<string>();
        internal static Dictionary<string, BaseCommand> CommandList
        {
            get
            {
                // Check if commands are not already registred, then register them
                if (_commandList == null || _commandList.Count == 0)
                    RegisterCommands();

                return _commandList;
            }
            set
            {
                _commandList = value;
            }
        }

        public static bool RunCommand(string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
                return false;

            Logger.Log($"\n> {commandString}");
            RegisterCommandHistory(commandString);

            // Check if command is repeatLast
            if (commandString.Equals(repeatCmdName))
                commandString = CommandHistory.Last();

            // Retrive the parts of command in array
            string[] commandParts = Helper.GetCommandParts(commandString);

            // Handle the commandParts separately
            int numArgs = commandParts.Length - 1;
            string command = commandParts.FirstOrDefault().ToLower();
            string[] args = new string[numArgs];

            // If there are args for this command, copy them to new array
            if (numArgs > 0)
                Array.Copy(commandParts, 1, args, 0, numArgs);

            // Try get command from dictionary
            if (!CommandList.TryGetValue(command, out BaseCommand reg))
            {
                reg = CommandList.FirstOrDefault(x => x.Value.Alias == command).Value;

                if (reg == null)
                {
                    var errorMessage = $"'{command}' is an unknown command, type 'help' for list.";

                    // Looking for similar command
                    var similarCommand = CommandList
                        .Select(x => new
                        {
                            commandName = x.Key,
                            similarity = x.Key.CalculateSimilarity(command)
                        })
                        .OrderByDescending(x => x.similarity)
                        .FirstOrDefault();

                    // Print if there are a similar command
                    if (similarCommand != null && similarCommand.similarity >= 0.6f)
                        errorMessage += $"\nThe most similar command is\n\t{similarCommand.commandName}";

                    Logger.Error(errorMessage);
                    return false;
                }
            }

            try
            {
                return reg.Execute(args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }
        }

        private static void RegisterCommandHistory(string commandString)
        {
            if (CommandHistory.Count > maxCommandHistory)
                CommandHistory.Dequeue();
            CommandHistory.Enqueue(commandString);
        }

        private static void RegisterCommands()
        {
            // Initialize commands dictionary
            if(_commandList == null)
                _commandList = new Dictionary<string, BaseCommand>();

            // Retrive all classes which derives from BaseCommand's class
            var allCommandsTypes = Assembly.GetAssembly(typeof(BaseCommand)).GetTypes()
                .Where(t => typeof(BaseCommand).IsAssignableFrom(t) && t.IsAbstract == false);

            // For all commandsTypes, intantiate it, and store in dictonary
            foreach (var commandType in allCommandsTypes)
            {
                var commandObj = Activator.CreateInstance(commandType, Logger) as BaseCommand;
                var commandName = commandObj.CommandName;
               
                _commandList[commandName] = commandObj;
            }
        }

        public static void RegisterNewCommand<T>()
        {
            if (!typeof(T).IsSubclassOf(typeof(BaseCommand)))
                throw new DevConsoleException($"Can not register {typeof(T).Name} because is not inherent of BaseCommand class.");

            var commandObj = Activator.CreateInstance(typeof(T), Logger) as BaseCommand;
            var commandName = commandObj.CommandName;

            if (_commandList == null)
                _commandList = new Dictionary<string, BaseCommand>();

            _commandList[commandName] = commandObj;
        }

        public static string[] GetSuggestions(string commandLine)
        {
            var commandParts = Helper.GetCommandParts(commandLine).ToArray();

            if (commandParts.Length == 1)
                return CommandList.Keys.Where(x => x.StartsWith(commandParts[0])).ToArray();
            
            ISuggestible suggestible = (CommandList.FirstOrDefault(x => x.Value.CommandName == commandParts[0]).Value) as ISuggestible;

            if (suggestible == null)
                return null;

            return suggestible.GetSuggestions(commandParts);
        }            
    }
}