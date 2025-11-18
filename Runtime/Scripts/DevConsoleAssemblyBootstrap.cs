using UnityEngine;
using Tools.DevConsole;

namespace Tools.DevConsole
{
    /// <summary>
    /// Simple bootstrap component to automatically register the current assembly for DevConsole command discovery.
    /// Add this to a GameObject in your scene or create it programmatically to ensure your custom commands are found.
    /// </summary>
    [AddComponentMenu("DevConsole/Assembly Bootstrap")]
    public class DevConsoleAssemblyBootstrap : MonoBehaviour
    {
        [SerializeField, Tooltip("Whether to register the assembly automatically on Awake")]
        private readonly bool registerOnAwake = true;

        [SerializeField, Tooltip("Whether to destroy this GameObject after registration")]
        private readonly bool destroyAfterRegistration = false;

        void Awake()
        {
            if (registerOnAwake)
                RegisterAssembly();
        }

        /// <summary>
        /// Manually register the calling assembly for command discovery
        /// </summary>
        public void RegisterAssembly()
        {
            DevConsole.RegisterCallingAssembly();

            if (destroyAfterRegistration)
                Destroy(gameObject);
        }
    }
}
