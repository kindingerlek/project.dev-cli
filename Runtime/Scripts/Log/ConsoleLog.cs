using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DevConsole.Interfaces;
using UnityEngine;

namespace Tools.DevConsole
{
    public delegate void ReceiveMessageHandler(IConsoleLogMessage message);
    public delegate void ClearHandler();

    public class ConsoleLog : IConsoleLog
    {
        public Queue<IConsoleLogMessage> LogHistory { get; internal set; }
        public int MaxLogRegister { get; set; }

        public event ReceiveMessageHandler OnReceiveMessage;
        public event ClearHandler OnClear;

        private bool useUnityLogCallback;

        public MessageType MaxLogLevel { get; set ; }

        public bool UseUnityLogCallback
        {
            get
            {
                return useUnityLogCallback;
            }
            set
            {
                useUnityLogCallback = value;

                if(value)
                    Application.logMessageReceived += UnityLogCallbackReceiver;
                else
                    Application.logMessageReceived -= UnityLogCallbackReceiver;
            }
        }


        public ConsoleLog(MessageType maxLogLevel)
        {
            MaxLogLevel = maxLogLevel;
            LogHistory = new Queue<IConsoleLogMessage>();
        }

        private void UnityLogCallbackReceiver(string condition, string stackTrace, LogType type)
        {
            switch(type)
            {
                case LogType.Log:
                    Debug(condition);
                    break;

                case LogType.Warning:
                    Warning(condition);
                    break;

                case LogType.Error:
                case LogType.Exception:
                    Error(condition);
                    break;

                default:
                    Log(condition);
                    break;
            }
        }

        public void Debug(string message)
        {
            Enqueue(new ConsoleLogMessage(MessageType.DEBUG, message));
        }
        public void Error(string message)
        {
            Enqueue(new ConsoleLogMessage(MessageType.ERROR, message));
        }
        public void Log(string message)
        {
            Enqueue(new ConsoleLogMessage(MessageType.LOG, message));
        }
        public void Warning(string message)
        {
            Enqueue(new ConsoleLogMessage(MessageType.WARNING, message));
        }

        public void Clear()
        {
            LogHistory.Clear();
            OnClear?.Invoke();
        }

        private void Enqueue(IConsoleLogMessage message)
        {
            if (message.Type > MaxLogLevel)
                return;

            if (LogHistory.Count > MaxLogRegister)
                LogHistory.Dequeue();
            LogHistory.Enqueue(message);

            OnReceiveMessage?.Invoke(message);
        }
    }
}
