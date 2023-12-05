using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private Console _targetConsole;
    [SerializeField] private float _height;
    [SerializeField] private float _scale;
    [SerializeField] private RectTransform _consoleGUIParent;
    [SerializeField] private RectTransform _GUIParent;
    [SerializeField] private TMP_InputField _consoleInputField, _consoleContentField;
    [SerializeField] private ScrollRect _consoleContentScrollRect;
    [SerializeField] private RectTransform _consoleContentRect;
    [SerializeField] private TMP_InputField _hintInputField;
    private List<TMP_InputField> _hintsInputFields = new();
    [SerializeField] private List<ScalableRect> _reckTransformsToScale;


    private void Awake()
    {
        SetEvents();
        CheckEventSystem();
    }

    private void Start()
    {
        CloneHintField();
        SetGUIHeight(_height);
        SetGUIScale(_scale);
    }

    private void CheckEventSystem()
    {
        if (EventSystem.current == null)
        {
            _targetConsole.ToggleConsoleInput();
            Debug.LogError("Missing EventSystem. Console will not work properly.");
            enabled = false;
        }
    }

    private void SetEvents()
    {
        _consoleInputField.onValueChanged.AddListener(_targetConsole.InputFieldChangedInput);
        _targetConsole.OnConsoleToggled += OnConsoleToggled;
        _targetConsole.OnContentChanged += OnConsoleContentChanged;
        _targetConsole.OnHintsChanged += OnHintsChanged;
        _targetConsole.OnHistoryRecall += OnHistoryRecall;
        _targetConsole.OnHintAccept += OnHintAccept;
    }

    private void CloneHintField()
    {
        for (int i = 0; i < _targetConsole.MaxHintsAmount; i++)
        {
            TMP_InputField toAdd = Instantiate(_hintInputField);
            toAdd.transform.SetParent(_hintInputField.transform.parent, false);
            _hintsInputFields.Add(toAdd);
            _reckTransformsToScale.Add(new ScalableRect(toAdd.GetComponent<RectTransform>(), ScaleType.Height));
            _reckTransformsToScale.Add(new ScalableRect(toAdd.textComponent.GetComponent<RectTransform>(), ScaleType.FontSize));
        }
        Destroy(_hintInputField.gameObject);
    }

    private void Update()
    {
        CheckConsoleInput();
    }

    private void CheckConsoleInput()
    {
        CheckInputManagerInput();
        CheckInputSystemInput();
    }

    private void CheckInputManagerInput()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            PerformToggleInput();

        if (Input.GetKeyDown(KeyCode.Return))
            PerformEnterInput();

        if (Input.GetKeyDown(KeyCode.DownArrow))
            PerformHistoryRecallDownInput();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            PerformHistoryRecallUpInput();

        if (Input.GetKeyDown(KeyCode.Tab))
            PerformAcceptHintInput();

    }

    private void CheckInputSystemInput()
    {
        //uncomment this to use the InputSystem

        // if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        //     PerformToggleInput();

        // if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        //     PerformEnterInput();

        // if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        //     PerformHistoryRecallDownInput();

        // if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        //     PerformHistoryRecallUpInput();

        // if (Keyboard.current.tabKey.wasPressedThisFrame)
        //     PerformAcceptHintInput();
    }

    private void PerformToggleInput()
    {
        _targetConsole.ToggleConsoleInput();
    }

    private void PerformEnterInput()
    {
        if (!IsInputFieldSelected())
            return;

        _targetConsole.EnterInput(_consoleInputField.text);
        ClearAndSelectInputField();
    }

    private void PerformHistoryRecallUpInput()
    {
        _targetConsole.RecallHistoryUpInput();
    }

    private void PerformHistoryRecallDownInput()
    {
        _targetConsole.RecallHistoryDownInput();
    }

    private void PerformAcceptHintInput()
    {
        _targetConsole.AcceptHintInput();
    }

    private void OnConsoleToggled(bool toggled)
    {
        _consoleGUIParent.gameObject.SetActive(toggled);

        if (toggled)
        {
            _consoleContentField.text = Console.Content;

            ResizeContentRect();
            ScrollDown();
            ClearAndSelectInputField();
        }
    }

    private void OnConsoleContentChanged()
    {
        _consoleContentField.text = Console.Content;

        ResizeContentRect();
        ScrollDown();
    }

    private void OnHintsChanged()
    {
        int hintsCount = Console.Hints.Count;
        for (int i = 0; i < _hintsInputFields.Count; i++)
        {
            if (i < hintsCount)
            {
                _hintsInputFields[i].text = Console.Hints[i];
                _hintsInputFields[i].textComponent.ForceMeshUpdate();
            }

            _hintsInputFields[i].gameObject.SetActive(i < hintsCount);
        }
    }

    private void OnHistoryRecall(string recalledInput)
    {
        _consoleInputField.text = recalledInput;
        RepositionInputFieldCaret();
    }

    private void OnHintAccept(string hint)
    {
        _consoleInputField.text = hint;
        RepositionInputFieldCaret();
    }

    private void RepositionInputFieldCaret()
    {
        _consoleInputField.caretPosition = _consoleInputField.text.Length;
    }

    private void SetGUIHeight(float height)
    {
        _GUIParent.sizeDelta = new Vector2(_GUIParent.sizeDelta.x, height);
    }

    private void SetGUIScale(float scale)
    {
        for (int i = 0; i < _reckTransformsToScale.Count; i++)
        {
            ScalableRect scalableRect = _reckTransformsToScale[i];
            RectTransform rectToScale = scalableRect.RectTransform;
            switch (scalableRect.ScaleType)
            {
                case ScaleType.Height:
                    {
                        rectToScale.sizeDelta = new(rectToScale.sizeDelta.x, rectToScale.sizeDelta.y * scale);
                        break;
                    }
                case ScaleType.Width:
                    {
                        rectToScale.sizeDelta = new(rectToScale.sizeDelta.x * scale, rectToScale.sizeDelta.y);
                        break;
                    }
                case ScaleType.Both:
                    {
                        rectToScale.sizeDelta *= scale;
                        break;
                    }
                case ScaleType.FontSize:
                    {
                        TMP_Text componentTMP = rectToScale.GetComponent<TMP_Text>();
                        componentTMP.fontSize *= scale;
                        break;
                    }
            }
        }
    }

    private void ResizeContentRect()
    {
        TMP_Text textComponent = _consoleContentField.textComponent;

        textComponent.ForceMeshUpdate();

        if (textComponent.textInfo == null)
            return;

        float height = textComponent.margin.y * 2;

        for (int i = 0; i < textComponent.textInfo.lineCount; i++)
            height += textComponent.textInfo.lineInfo[i].lineHeight;

        _consoleContentRect.sizeDelta = new Vector2(0, height);
    }

    private void ClearAndSelectInputField()
    {
        _consoleInputField.text = string.Empty;
        _consoleInputField.ActivateInputField();
    }

    private void ScrollDown()
    {
        if (_consoleContentScrollRect)
            _consoleContentScrollRect.verticalNormalizedPosition = 0f;
    }

    private bool IsInputFieldSelected()
    {
        return EventSystem.current.currentSelectedGameObject == _consoleInputField.gameObject;
    }

    [Serializable]
    private class ScalableRect
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private ScaleType scaleType;

        public ScalableRect(RectTransform rectTransform, ScaleType scaleType)
        {
            this.rectTransform = rectTransform;
            this.scaleType = scaleType;
        }

        public RectTransform RectTransform { get { return rectTransform; } }
        public ScaleType ScaleType { get { return scaleType; } }
    }

    private enum InputType
    {
        InputManager,
        InputSystem
    }

    private enum ScaleType
    {
        Height,
        Width,
        Both,
        FontSize,
    }
}
