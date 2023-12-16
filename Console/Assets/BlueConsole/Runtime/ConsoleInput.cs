using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;

public class ConsoleInput : MonoBehaviour
{
    [SerializeField] private Console _targetConsole;
    [SerializeField] private TMP_InputField _consoleInputField;

    private void Awake()
    {
        SetEvents(true);
    }

    private void OnDisable()
    {
        SetEvents(false);
    }

    private void SetEvents(bool subscribe)
    {
        if (subscribe)
            _consoleInputField.onValueChanged.AddListener(_targetConsole.InputFieldChangedInput);
        ConsoleUtils.SetActionListener(ref Console.OnConsoleToggled, OnConsoleToggled, subscribe);
        ConsoleUtils.SetActionListener(ref Console.OnHistoryRecall, OnHistoryRecall, subscribe);
        ConsoleUtils.SetActionListener(ref Console.OnHintAccept, OnHintAccept, subscribe);
    }

    private void OnConsoleToggled(bool toggled)
    {
        if (toggled)
            ClearAndSelectInputField();
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

    private bool IsInputFieldSelected()
    {
        return EventSystem.current.currentSelectedGameObject == _consoleInputField.gameObject;
    }

    private void ClearAndSelectInputField()
    {
        _consoleInputField.text = string.Empty;
        _consoleInputField.ActivateInputField();
    }
}
