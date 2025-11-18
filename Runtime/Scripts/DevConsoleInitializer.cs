using System.Reflection;
using UnityEngine;

namespace Tools.DevConsole
{
    /// <summary>
    /// Simple static initializer for DevConsole that runs before scene loading.
    /// This provides a code-based alternative to the ScriptableObject settings approach.
    /// </summary>
    public static class DevConsoleInitializer
    {
        /// <summary>
        /// Initialize DevConsole before any scenes load
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Register the calling assembly (typically the game assembly)
            RegisterGameAssembly();

            // Register any other common game assemblies
            RegisterCommonAssemblies();
        }

        /// <summary>
        /// Register the main game assembly
        /// </summary>
        private static void RegisterGameAssembly()
        {
            try
            {
                // Register the assembly that contains this initializer
                DevConsole.RegisterAssembly(Assembly.GetExecutingAssembly());
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[DevConsole] Failed to register game assembly: {ex.Message}");
            }
        }

        /// <summary>
        /// Register other commonly used assemblies
        /// </summary>
        private static void RegisterCommonAssemblies()
        {
            // Common Unity assembly names that might contain custom commands
            string[] commonAssemblyNames =
            [
                "Assembly-CSharp",
                "Assembly-CSharp-firstpass",
                "GameAssembly",
                "Scripts"
            ];

            foreach (var assemblyName in commonAssemblyNames)
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    DevConsole.RegisterAssembly(assembly);
                }
                catch
                {
                    // Assembly doesn't exist, which is fine
                }
            }
        }

        /// <summary>
        /// Manually register a specific assembly by name
        /// Call this method if you have custom assemblies that need registration
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to register</param>
        public static void RegisterAssemblyByName(string assemblyName)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                DevConsole.RegisterAssembly(assembly);
                Debug.Log($"[DevConsole] Successfully registered assembly: {assemblyName}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(
                    $"[DevConsole] Failed to register assembly '{assemblyName}': {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Register an assembly by type (useful when you have a reference to a type in that assembly)
        /// </summary>
        /// <typeparam name="T">Any type from the assembly you want to register</typeparam>
        public static void RegisterAssemblyByType<T>()
        {
            try
            {
                var assembly = Assembly.GetAssembly(typeof(T));
                DevConsole.RegisterAssembly(assembly);
                Debug.Log(
                    $"[DevConsole] Successfully registered assembly containing type: {typeof(T).Name}"
                );
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(
                    $"[DevConsole] Failed to register assembly for type '{typeof(T).Name}': {ex.Message}"
                );
            }
        }
    }
}
