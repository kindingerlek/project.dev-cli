using Tools.DevConsole.Interfaces;
using Tools.DevConsole.UI;
using UnityEngine;

namespace Tools.DevConsole
{
    public class DIDevConsoleBootstrap : MonoBehaviour
    {
        static GameObject instance;

        IConsoleLog consoleLog;

        void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject;            
            consoleLog = new ConsoleLog(MessageType.LOG);
            consoleLog.MaxLogRegister = 1000;

            DevConsole.Logger = consoleLog;
            //GetComponent<DevConsoleUI>().consoleLog = consoleLog;
            GetComponent<DevConsoleUITK>().consoleLog = consoleLog;
        }
    } 
}
