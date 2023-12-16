using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private Console _targetConsole;
    [SerializeField] private float _height;
    [SerializeField] private float _scale;
    [SerializeField] private RectTransform _consoleGUIParent, _GUIParent;
    [SerializeField] private TMP_InputField _consoleContentField;
    [SerializeField] private ScrollRect _consoleContentScrollRect;
    [SerializeField] private RectTransform _consoleContentRect;
    [SerializeField] private TMP_InputField _hintInputField;
    private readonly List<TMP_InputField> _hintsInputFields = new();
    [SerializeField] private List<ScalableRect> _reckTransformsToScale;


    private void Awake()
    {
        SetEvents(true);
        CheckEventSystem();
    }

    private void OnDisable()
    {
        SetEvents(false);
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

    private void SetEvents(bool subscribe)
    {
        ConsoleUtils.SetActionListener(ref Console.OnConsoleToggled, OnConsoleToggled, subscribe);
        ConsoleUtils.SetActionListener(ref Console.OnContentChanged, OnConsoleContentChanged, subscribe);
        ConsoleUtils.SetActionListener(ref Console.OnHintsChanged, OnHintsChanged, subscribe);
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

    private void OnConsoleToggled(bool toggled)
    {
        _consoleGUIParent.gameObject.SetActive(toggled);

        if (toggled)
        {
            _consoleContentField.text = Console.Content;

            ResizeContentRect();
            ScrollDown();
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

    private void ScrollDown()
    {
        if (_consoleContentScrollRect)
            _consoleContentScrollRect.verticalNormalizedPosition = 0f;
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
