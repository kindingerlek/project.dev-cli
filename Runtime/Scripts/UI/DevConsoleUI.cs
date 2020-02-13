using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.DevConsole.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools.DevConsole.UI
{
    public class DevConsoleUI : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        private InputField commandInputField;

        [Header("Buttons")]
        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button clearButton;

        [SerializeField]
        private Button submitButton;

        [Header("Toggles")]
        [SerializeField]
        private Toggle showDebugToggle;

        [SerializeField]
        private Toggle showWarningToggle;

        [SerializeField]
        private Toggle showErrorToggle;

        [SerializeField]
        private Toggle useUnityLogToggle;

        [Header("Labels")]
        [SerializeField]
        private Text logTextField;

        [SerializeField]
        private Text suggestionText;

        [Header("Colors")]
        [SerializeField]
        private Color debugColor = Color.cyan;
        
        [SerializeField]
        private Color warningColor = Color.yellow;
        
        [SerializeField]
        private Color errorColor = Color.red;
        
        [SerializeField]
        private Color logColor = Color.white;

        [SerializeField]
        private bool showOnStart = false;

        internal IConsoleLog consoleLog;


        private bool submitButtonTriggered = false;
        private int navigateHistory;
        private Canvas canvas;
        private float defaultTimeScale;

        void Start()
        {
            defaultTimeScale = Time.timeScale;
            canvas = this.transform.GetComponentInChildren<Canvas>();
            canvas.enabled = true;

            ShowConsoleUI(showOnStart);

            consoleLog.OnReceiveMessage += OnReceiveNewMessage;
            consoleLog.OnClear += Refresh;

            closeButton.onClick.AddListener(OnCloseButtonClick);
            clearButton.onClick.AddListener(OnClearButtonClick);
            submitButton.onClick.AddListener(OnSubmitButtonClick);

            commandInputField.onEndEdit.AddListener(OnCommandInputSubmit);
            commandInputField.onValueChanged.AddListener(OnCommandInputChange);
            useUnityLogToggle.onValueChanged.AddListener(OnToggleUnityLogChange);

            showDebugToggle.onValueChanged.AddListener(OnToggleDebugChange);
            showErrorToggle.onValueChanged.AddListener(OnToggleErrorChange);
            showWarningToggle.onValueChanged.AddListener(OnToggleWarningChange);

            logTextField.text = "";

            useUnityLogToggle.isOn = true;
        }
       
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                var enabled = !canvas.gameObject.activeSelf;
                ShowConsoleUI(enabled);
            }

            if (!canvas.gameObject.activeSelf)
                return;
            
            if (commandInputField.isFocused)
            {
                // The caret is at last position and accept suggestions keys are triggered
                if (commandInputField.caretPosition == commandInputField.text.Length
                    && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    var suggestions = DevConsole.GetSuggestions(commandInputField.text);

                    
                    if (suggestions != null)
                    {
                        if (suggestions.Length == 1)
                        {
                            //commandInputField.text = suggestions.First();
                            var list = Helper.GetCommandParts(commandInputField.text).ToList();
                            list[list.Count - 1] = suggestions.First();

                            commandInputField.text = string.Join(" ", list);
                        }
                        else
                        {
                            consoleLog.Log($" > {commandInputField.text}");
                            consoleLog.Log(string.Join("\n", suggestions));
                        }
                    }

                    commandInputField.caretPosition = commandInputField.text.Length;
                }

                // Navigate to previous commands
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    navigateHistory = Mathf.Clamp(--navigateHistory, 0, DevConsole.CommandHistory.Count - 1);
                    commandInputField.text = DevConsole.CommandHistory.ElementAt(navigateHistory);

                    commandInputField.caretPosition = commandInputField.text.Length;
                }

                // Navigate to next commands
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    navigateHistory = Mathf.Clamp(++navigateHistory, 0, DevConsole.CommandHistory.Count - 1);
                    commandInputField.text = DevConsole.CommandHistory.ElementAt(navigateHistory);

                    commandInputField.caretPosition = commandInputField.text.Length;
                }


            }
        }

        private void ShowConsoleUI(bool hide)
        {
            if (hide)
                defaultTimeScale = Time.timeScale;

            Time.timeScale = hide ? 0f : defaultTimeScale;

            canvas.gameObject.SetActive(hide);
        }

        #region [Events]
        private void OnCommandInputChange(string value)
        {
            // Change the placeholder text to fit to a plausible desirable command
            var inputValue = commandInputField.text;
            var plausibleCommand = DevConsole.CommandList.Where(x => x.Value.CommandName.StartsWith(inputValue));

            suggestionText.text = plausibleCommand.Count() == 1 && !string.IsNullOrEmpty(inputValue) ? plausibleCommand.First().Value.CommandName : string.Empty;
        }
        private void OnToggleUnityLogChange(bool value)
        {
            consoleLog.UseUnityLogCallback = value;
        }
        private void OnToggleDebugChange(bool value)
        {
            Refresh();
        }
        private void OnToggleErrorChange(bool value)
        {
            Refresh();
        }
        private void OnToggleWarningChange(bool value)
        {
            Refresh();
        }
        public void OnSubmitButtonClick()
        {
            submitButtonTriggered = true;
            OnCommandInputSubmit(commandInputField.text);
        }
        public void OnClearButtonClick()
        {
            consoleLog.Clear();
            logTextField.text = string.Empty;
        }
        private void OnCloseButtonClick()
        {
            ShowConsoleUI(false);
        }
        public void OnCommandInputSubmit(string arg)
        {
            //Prevent submit command when inputfield loose the focus
            if (!Input.GetButtonDown("Submit") && !submitButtonTriggered)
                return;

            submitButtonTriggered = false;
            commandInputField.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(commandInputField.gameObject, null);
            commandInputField.OnPointerClick(new PointerEventData(EventSystem.current));
            DevConsole.RunCommand(arg);
            navigateHistory = consoleLog.LogHistory.Count - 1;
        } 
        #endregion

        public void Refresh()
        {
            logTextField.text = string.Empty;

            foreach (var logMessage in consoleLog.LogHistory.Where(x => FilterMessage(x)))
            {
                logTextField.text += FormatPrint(logMessage);
            }
        }

        private bool FilterMessage(IConsoleLogMessage message)
        {
            return
                (message.Type == MessageType.LOG)
                    || (showDebugToggle.isOn && message.Type == MessageType.DEBUG)
                    || (showErrorToggle.isOn && message.Type == MessageType.ERROR)
                    || (showWarningToggle.isOn && message.Type == MessageType.WARNING);
        }

        public void OnReceiveNewMessage(IConsoleLogMessage message)
        {
            if(logTextField != null)
                logTextField.text += FormatPrint(message);
        }

        private string FormatPrint(IConsoleLogMessage message)
        {
           var m =  message.Text.TrimEnd(Environment.NewLine.ToCharArray());

            switch(message.Type)
            {
                case MessageType.DEBUG:
                    return $"<color=\"#{ColorUtility.ToHtmlStringRGB(debugColor)}\">{m}</color>\n";
                case MessageType.WARNING:
                    return $"<color=\"#{ColorUtility.ToHtmlStringRGB(warningColor)}\">{m}</color>\n";
                case MessageType.ERROR:
                    return $"<color=\"#{ColorUtility.ToHtmlStringRGB(errorColor)}\">{m}</color>\n";
                case MessageType.LOG:
                default:
                    return $"<color=\"#{ColorUtility.ToHtmlStringRGB(logColor)}\">{m}</color>\n";

            }
        }
    } 
}
