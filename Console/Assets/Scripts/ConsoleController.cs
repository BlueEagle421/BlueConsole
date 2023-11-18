using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private Console _targetConsole;
    [SerializeField] private GameObject _consoleParent;
    [SerializeField] private InputActionReference _toggleActionReference, _enterActionReference;
    [SerializeField] private TMP_InputField _consoleInputField, _consoleContentField;
    [SerializeField] private ScrollRect _consoleContentScrollRect;
    [SerializeField] private RectTransform _consoleContentRect;
    [SerializeField] private List<TMP_InputField> _hintsInputFields;


    private void Awake()
    {
        _consoleInputField.onValueChanged.AddListener(_targetConsole.InputFieldChangedInput);
        _targetConsole.OnConsoleToggled += OnConsoleToggled;
        _targetConsole.OnContentChanged += OnConsoleContentChanged;
        _targetConsole.OnHintsChanged += OnHintsChanged;
    }

    private void OnEnable()
    {
        _toggleActionReference.action.actionMap.Enable();
        _enterActionReference.action.actionMap.Enable();
    }

    private void Update()
    {
        CheckConsoleInput();
    }

    private void CheckConsoleInput()
    {
        if (_toggleActionReference.action.WasPerformedThisFrame())
            _targetConsole.ToggleConsoleInput();

        if (_enterActionReference.action.WasPerformedThisFrame() && IsInputFieldSelected())
        {
            _targetConsole.EnterInput(_consoleInputField.text);
            ClearAndSelectInputField();
        }
    }

    private void OnConsoleToggled(bool toggled)
    {
        _consoleParent.SetActive(toggled);

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
                _hintsInputFields[i].text = Console.Hints[i];

            _hintsInputFields[i].gameObject.SetActive(i < hintsCount);
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
}
