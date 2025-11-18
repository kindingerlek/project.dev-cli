using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Tools.DevConsole
{
    /// <summary>
    /// Singleton ScriptableObject that automatically registers assemblies for DevConsole command discovery.
    /// This runs before scene loading to ensure all commands are available early in the application lifecycle.
    /// </summary>
    [CreateAssetMenu(fileName = "DevConsoleSettings", menuName = "DevConsole/Settings")]
    public class DevConsoleSettings : ScriptableObject
    {
        [Header("Assembly Registration")]
        [SerializeField, Tooltip("Whether to automatically register all non-system assemblies")]
        private readonly bool autoRegisterAssemblies = true;

        [SerializeField, Tooltip("Assembly names to explicitly include (without .dll extension)")]
        private List<string> assemblyNamesToInclude = new List<string>();

        [SerializeField, Tooltip("Assembly names to explicitly exclude (without .dll extension)")]
        private List<string> assemblyNamesToExclude = new List<string>();

        [Header("Debug")]
        [SerializeField, Tooltip("Log assembly registration details to console")]
        private readonly bool debugLogging = false;

        private static DevConsoleSettings _instance;

        /// <summary>
        /// Get the singleton instance of DevConsoleSettings
        /// </summary>
        public static DevConsoleSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Try to load from Resources folder
                    _instance = Resources.Load<DevConsoleSettings>("DevConsoleSettings");

                    if (_instance == null)
                    {
                        // Create a default instance if none exists
                        _instance = CreateInstance<DevConsoleSettings>();
                        _instance.name = "DevConsoleSettings (Runtime)";
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initialize DevConsole assembly registration before any scenes load
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeDevConsole()
        {
            var settings = Instance;
            settings.RegisterAssemblies();
        }

        /// <summary>
        /// Register assemblies based on the current settings
        /// </summary>
        public void RegisterAssemblies()
        {
            if (debugLogging)
                Debug.Log("[DevConsole] Starting assembly registration...");

            var registeredCount = 0;

            if (autoRegisterAssemblies)
            {
                // Register all loaded assemblies automatically
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (ShouldRegisterAssembly(assembly))
                    {
                        DevConsole.RegisterAssembly(assembly);
                        registeredCount++;

                        if (debugLogging)
                            Debug.Log(
                                $"[DevConsole] Registered assembly: {assembly.GetName().Name}"
                            );
                    }
                }
            }

            // Register explicitly included assemblies
            foreach (var assemblyName in assemblyNamesToInclude)
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    DevConsole.RegisterAssembly(assembly);
                    registeredCount++;

                    if (debugLogging)
                        Debug.Log($"[DevConsole] Explicitly registered assembly: {assemblyName}");
                }
                catch (System.Exception ex)
                {
                    if (debugLogging)
                        Debug.LogWarning(
                            $"[DevConsole] Failed to load assembly '{assemblyName}': {ex.Message}"
                        );
                }
            }

            if (debugLogging)
                Debug.Log(
                    $"[DevConsole] Assembly registration complete. Registered {registeredCount} assemblies."
                );
        }

        /// <summary>
        /// Determine if an assembly should be automatically registered
        /// </summary>
        private bool ShouldRegisterAssembly(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;

            if (assemblyNamesToExclude.Contains(assemblyName))
                return false;

            var fullName = assembly.FullName;

            // Skip for performance
            if (
                fullName.StartsWith("System.")
                || fullName.StartsWith("Microsoft.")
                || fullName.StartsWith("mscorlib")
                || fullName.StartsWith("netstandard")
                || fullName.StartsWith("Unity.")
                || fullName.StartsWith("UnityEngine.")
                || fullName.StartsWith("UnityEditor.")
                || fullName.StartsWith("Mono.")
                || fullName.StartsWith("Newtonsoft.")
                || fullName.StartsWith("ExCSS.")
                || fullName.StartsWith("log4net")
            )
                return false;

            // Skip assemblies without any types (usually dynamic assemblies)
            try
            {
                assembly.GetTypes();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Add an assembly name to the include list
        /// </summary>
        public void AddAssemblyToInclude(string assemblyName)
        {
            if (!assemblyNamesToInclude.Contains(assemblyName))
                assemblyNamesToInclude.Add(assemblyName);
        }

        /// <summary>
        /// Add an assembly name to the exclude list
        /// </summary>
        public void AddAssemblyToExclude(string assemblyName)
        {
            if (!assemblyNamesToExclude.Contains(assemblyName))
                assemblyNamesToExclude.Add(assemblyName);
        }

        /// <summary>
        /// Force re-registration of all assemblies
        /// </summary>
        public void ForceReregisterAssemblies()
        {
            RegisterAssemblies();
        }

#if UNITY_EDITOR
        [Header("Editor Tools")]
        [SerializeField, Tooltip("Click to force re-register assemblies in editor")]
        private bool _forceReregister;

        private void OnValidate()
        {
            if (!_forceReregister)
                return;

            _forceReregister = false;
            if (Application.isPlaying)
                ForceReregisterAssemblies();
        }

        [UnityEditor.MenuItem("Tools/DevConsole/Create Settings Asset")]
        private static void CreateSettingsAsset()
        {
            var settings = CreateInstance<DevConsoleSettings>();
            var path = "Assets/Resources/DevConsoleSettings.asset";

            // Create Resources folder if it doesn't exist
            if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEditor.AssetDatabase.CreateAsset(settings, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = settings;

            Debug.Log($"DevConsoleSettings created at: {path}");
        }
#endif
    }
}
