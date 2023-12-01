using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.DevConsole.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Tools.DevConsole.UI
{
    [RequireComponent(typeof(UIDocument))]
    [ExecuteAlways]
    public class DevConsoleUITK : MonoBehaviour
    {
        const string CLEAR_BUTTON = "clear-button";
        const string SUBMIT_BUTTON = "submit-button";
        const string CLOSE_BUTTON = "close-button";

        const string INFO_TOGGLE = "info-toggle";
        const string WARN_TOGGLE = "warn-toggle";
        const string ERROR_TOGGLE = "error-toggle";
        const string SHOWLOGS_TOGGLE = "showlogs-toggle";

        const string COMMAND_INPUT = "command-input";

        const string LABEL_PLACEHOLDER = "placeholder-label";
        const string LABEL_CONTENT = "content-label";


        public UIDocument _UIDocument;
        public VisualTreeAsset _VisualTreeAsset;


        [Header("Colors")]
        [SerializeField] private Color debugColor = Color.cyan;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;
        [SerializeField] private Color logColor = Color.white;
        [SerializeField] private bool _ShowOnStart = false;

        internal IConsoleLog consoleLog;

        private Button _button__close;
        private Button _button__clear;
        private Button _button__submit;

        private Toggle _toggle__info;
        private Toggle _toggle__warn;
        private Toggle _toggle__error;
        private Toggle _toggle__showlogs;

        private Label _label__content;
        private Label _label__placeholder;
        private TextField _inputField;
        private VisualElement _inputFieldContainer;


        private float _defaultTimeScale = 1f;
        private bool _submitButtonTriggered = false;
        private int _navigateHistory;


        public void Start()
        {
            _UIDocument = GetComponent<UIDocument>();
            _VisualTreeAsset = _UIDocument.visualTreeAsset;

            _defaultTimeScale = Time.timeScale;
            ShowConsoleUI(_ShowOnStart);

            consoleLog ??= DevConsole.Logger ??= new ConsoleLog(MessageType.LOG);
            consoleLog.OnReceiveMessage += OnReceiveNewMessage;
            consoleLog.OnClear += Refresh;

            _button__close = _UIDocument.rootVisualElement.Q<Button>(CLOSE_BUTTON);
            _button__clear = _UIDocument.rootVisualElement.Q<Button>(CLEAR_BUTTON);
            _button__submit = _UIDocument.rootVisualElement.Q<Button>(SUBMIT_BUTTON);

            _toggle__info = _UIDocument.rootVisualElement.Q<Toggle>(INFO_TOGGLE);
            _toggle__warn = _UIDocument.rootVisualElement.Q<Toggle>(WARN_TOGGLE);
            _toggle__error = _UIDocument.rootVisualElement.Q<Toggle>(ERROR_TOGGLE);
            _toggle__showlogs = _UIDocument.rootVisualElement.Q<Toggle>(SHOWLOGS_TOGGLE);

            _label__placeholder = _UIDocument.rootVisualElement.Q<Label>(LABEL_PLACEHOLDER);
            _label__content = _UIDocument.rootVisualElement.Q<Label>(LABEL_CONTENT);

            _inputField = _UIDocument.rootVisualElement.Q<TextField>(COMMAND_INPUT);
            _inputFieldContainer = _inputField.Q<VisualElement>("unity-text-input");


            // Events assignment
            _button__close.clicked += OnCloseButtonClick;
            _button__clear.clicked += OnClearButtonClick;
            _button__submit.clicked += OnSubmitButtonClick;

            _toggle__info.RegisterValueChangedCallback(OnToggleFilterChange);
            _toggle__warn.RegisterValueChangedCallback(OnToggleFilterChange);
            _toggle__error.RegisterValueChangedCallback(OnToggleFilterChange);
            _toggle__showlogs.RegisterValueChangedCallback(OnToggleUnityLogChange);


            _inputField.RegisterValueChangedCallback(OnCommandInputChange);
            _inputField.RegisterCallback<KeyDownEvent>(OnKeydownInput);

            _label__content.text = string.Empty;
            _label__placeholder.text = string.Empty;

            _toggle__info.value = true;
            _toggle__warn.value = true;
            _toggle__error.value = true;

            _label__placeholder.SendToBack();
        }

        public void OnValidate()
        {
            if (_UIDocument == null)
                _UIDocument = GetComponent<UIDocument>();

            if (_VisualTreeAsset == null)
                _VisualTreeAsset = _UIDocument.visualTreeAsset;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                var enabled = _UIDocument.rootVisualElement.style.display == DisplayStyle.None;
                ShowConsoleUI(enabled);
            }

            if (_UIDocument.rootVisualElement.style.display == DisplayStyle.None)
                return;
        }

        public void OnKeydownInput(KeyDownEvent e)
        {
            if ((e.keyCode == KeyCode.Tab || e.keyCode == KeyCode.RightArrow)
                && _inputField.cursorIndex == _inputField.text.Length)
            {
                e.StopPropagation();
                e.PreventDefault();

                var suggestions = DevConsole.GetSuggestions(_inputField.text);
                if (suggestions != null)
                {
                    if (suggestions.Length == 1)
                    {
                        //commandInputField.text = suggestions.First();
                        var list = Helper.GetCommandParts(_inputField.text).ToList();
                        list[list.Count - 1] = suggestions.First();

                        _inputField.value = string.Join(" ", list);
                    }
                    else
                    {
                        consoleLog.Log($" > {_inputField.text}");
                        consoleLog.Log(string.Join("\n", suggestions));
                    }
                }
            }

            if (e.keyCode == KeyCode.UpArrow)
            {
                e.StopPropagation();
                e.PreventDefault();

                _navigateHistory = Mathf.Clamp(--_navigateHistory, 0, DevConsole.CommandHistory.Count - 1);
                _inputField.value = DevConsole.CommandHistory.ElementAt(_navigateHistory);
            }

            if (e.keyCode == KeyCode.DownArrow)
            {
                e.StopPropagation();
                e.PreventDefault();

                _navigateHistory = Mathf.Clamp(++_navigateHistory, 0, DevConsole.CommandHistory.Count - 1);
                _inputField.value = DevConsole.CommandHistory.ElementAt(_navigateHistory);
            }

            if (e.keyCode == KeyCode.Return)
            {
                e.StopPropagation();
                e.PreventDefault();

                OnCommandInputSubmit(_inputField.text);
                _inputField.value = string.Empty;
            }
        }

        private void ShowConsoleUI(bool hide)
        {
            if (hide)
                _defaultTimeScale = Time.timeScale;

            Time.timeScale = hide ? 0f : _defaultTimeScale;

            _UIDocument.rootVisualElement.style.display = hide ? DisplayStyle.Flex : DisplayStyle.None;
        }

        #region [Events]
        private void OnCommandInputChange(ChangeEvent<string> e)
        {
            // Change the placeholder text to fit to a plausible desirable command
            var inputValue = e.newValue;
            var plausibleCommand = DevConsole.CommandList.Where(x => x.Value.CommandName.StartsWith(inputValue));

            _label__placeholder.text = plausibleCommand.Count() == 1 && !string.IsNullOrEmpty(inputValue) ? plausibleCommand.First().Value.CommandName : string.Empty;
        }

        private void OnToggleUnityLogChange(ChangeEvent<bool> evt)
        {
            consoleLog.UseUnityLogCallback = evt.newValue;
            Refresh();
        }

        private void OnToggleFilterChange(ChangeEvent<bool> evt)
        {
            Refresh();
        }

        public void OnSubmitButtonClick()
        {
            _submitButtonTriggered = true;
            OnCommandInputSubmit(_inputField.text);
            Refresh();
        }

        public void OnClearButtonClick()
        {
            consoleLog.Clear();
        }

        private void OnCloseButtonClick()
        {
            ShowConsoleUI(false);
        }

        private void OnCommandInputSubmit(string arg)
        {

            //Prevent submit command when inputfield loose the focus
            if (!Input.GetButtonDown("Submit") && !_submitButtonTriggered)
                return;

            _submitButtonTriggered = false;

            _inputField.value = string.Empty;
            _inputField.Focus();

            DevConsole.RunCommand(arg);

            _navigateHistory = consoleLog.LogHistory.Count - 1;
        }


        #endregion

        private void Refresh()
        {
            var newContent = new StringBuilder();

            // FIXME: remove the call to ToArray() when the bug is fixed
            foreach (var message in consoleLog.LogHistory.ToArray().Where(FilterMessage))
                newContent.Append(FormatPrint(message));

            _label__content.text = newContent.ToString();
        }

        private bool FilterMessage(IConsoleLogMessage message)
        {
            if (message == null) return false;

            return
                (message.Type == MessageType.LOG)
                    || (_toggle__info.value && message.Type == MessageType.DEBUG)
                    || (_toggle__warn.value && message.Type == MessageType.WARNING)
                    || (_toggle__error.value && message.Type == MessageType.ERROR);
        }

        private void OnReceiveNewMessage(IConsoleLogMessage message)
        {
            if (_label__content != null)
                _label__content.text += FormatPrint(message);
        }

        private string FormatPrint(IConsoleLogMessage message)
        {
            var m = message.Text.TrimEnd(Environment.NewLine.ToCharArray());

            return message.Type switch
            {
                MessageType.DEBUG => $"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>{m}</color>\n",
                MessageType.WARNING => $"<color=#{ColorUtility.ToHtmlStringRGB(warningColor)}>{m}</color>\n",
                MessageType.ERROR => $"<color=#{ColorUtility.ToHtmlStringRGB(errorColor)}>{m}</color>\n",
                _ => $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>{m}</color>\n",
            };
        }
    }
}
