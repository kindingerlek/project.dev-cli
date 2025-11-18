namespace Tools.DevConsole
{
    public static class DevConsole
    {
        private static string repeatCmdName = "!!";
        private static readonly int maxCommandHistory = 50;
        private static Dictionary<string, BaseCommand> _commandList;
        private static List<Assembly> _customAssemblies = new List<Assembly>();
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

                foreach (var command in value)
                    CommandList.TryAdd(command.Key, command.Value);
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
                        .Select(
                            x =>
                                new
                                {
                                    commandName = x.Key,
                                    similarity = x.Key.CalculateSimilarity(command)
                                }
                        )
                        .OrderByDescending(x => x.similarity)
                        .FirstOrDefault();

                    // Print if there are a similar command
                    if (similarCommand != null && similarCommand.similarity >= 0.6f)
                        errorMessage +=
                            $"\nThe most similar command is\n\t{similarCommand.commandName}";

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
            _commandList ??= new Dictionary<string, BaseCommand>();

            // Get all assemblies to search
            var assembliesToSearch = GetAssembliesToSearch();

            // Search for BaseCommand subclasses
            var allCommandsTypes = new List<Type>();

            foreach (var assembly in assembliesToSearch)
            {
                try
                {
                    var types = assembly
                        .GetTypes()
                        .Where(
                            t =>
                                typeof(BaseCommand).IsAssignableFrom(t)
                                && !t.IsAbstract
                                && t != typeof(BaseCommand)
                        );

                    allCommandsTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException)
                {
                    // Skip assemblies that can't be loaded
                    continue;
                }
                catch (System.IO.FileNotFoundException)
                {
                    // Skip missing dependency assemblies
                    continue;
                }
            }

            // Instantiate and register all command types
            foreach (var commandType in allCommandsTypes)
            {
                try
                {
                    var commandObj = Activator.CreateInstance(commandType, Logger) as BaseCommand;
                    var commandName = commandObj.CommandName;

                    _commandList.TryAdd(commandName, commandObj);
                }
                catch (Exception ex)
                {
                    Logger?.Error($"Failed to register command {commandType.Name}: {ex.Message}");
                }
            }
        }

        private static Assembly[] GetAssembliesToSearch()
        {
            var assemblies = new List<Assembly>();

            // Always include the DevCLI assembly itself
            assemblies.Add(Assembly.GetAssembly(typeof(BaseCommand)));

            // Add explicitly registered assemblies
            assemblies.AddRange(_customAssemblies);

            // Add all currently loaded assemblies, excluding system ones
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyName = assembly.FullName;

                // Skip system assemblies for performance
                if (
                    assemblyName.StartsWith("System.")
                    || assemblyName.StartsWith("Microsoft.")
                    || assemblyName.StartsWith("mscorlib")
                    || assemblyName.StartsWith("netstandard")
                    || assemblyName.StartsWith("Unity.")
                    || assemblyName.StartsWith("UnityEngine.")
                    || assemblyName.StartsWith("UnityEditor.")
                )
                    continue;

                if (!assemblies.Contains(assembly))
                    assemblies.Add(assembly);
            }

            return assemblies.ToArray();
        }

        /// <summary>
        /// Register a specific assembly to be searched for BaseCommand subclasses.
        /// This is useful when you want to ensure commands from specific assemblies are discovered.
        /// </summary>
        /// <param name="assembly">The assembly to register for command discovery</param>
        public static void RegisterAssembly(Assembly assembly)
        {
            if (assembly == null)
                return;

            if (!_customAssemblies.Contains(assembly))
            {
                _customAssemblies.Add(assembly);

                // Force re-registration of commands if they were already loaded
                if (_commandList != null && _commandList.Count > 0)
                {
                    _commandList.Clear();
                    RegisterCommands();
                }
            }
        }

        /// <summary>
        /// Register the calling assembly to be searched for BaseCommand subclasses.
        /// Call this from your game code to ensure your custom commands are discovered.
        /// </summary>
        public static void RegisterCallingAssembly()
        {
            RegisterAssembly(Assembly.GetCallingAssembly());
        }

        public static void RegisterNewCommand<T>()
        {
            if (!typeof(T).IsSubclassOf(typeof(BaseCommand)))
                throw new DevConsoleException(
                    $"Can not register {typeof(T).Name} because is not inherent of BaseCommand class."
                );

            var commandObj = Activator.CreateInstance(typeof(T), Logger) as BaseCommand;
            var commandName = commandObj.CommandName;

            CommandList.TryAdd(commandName, commandObj);
        }

        public static string[] GetSuggestions(string commandLine)
        {
            var commandParts = Helper.GetCommandParts(commandLine).ToArray();

            if (commandParts.Length == 0)
                return null;

            if (commandParts.Length == 1)
                return CommandList.Keys.Where(x => x.StartsWith(commandParts[0])).ToArray();

            if (
                (CommandList.FirstOrDefault(x => x.Value.CommandName == commandParts[0]).Value)
                is not ISuggestible suggestible
            )
                return null;

            return suggestible.GetSuggestions(commandParts);
        }
    }
}
