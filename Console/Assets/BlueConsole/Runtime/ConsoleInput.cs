using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;

public class ConsoleInput : MonoBehaviour
{
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
            _consoleInputField.onValueChanged.AddListener(Console.Current.GenerateHints);
        Console.Current.OnConsoleToggled += OnConsoleToggled;
        Console.Current.OnHistoryRecall += OnHistoryRecall;
        Console.Current.OnHintAccept += OnHintAccept;
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
        Console.Current.InvertToggle();
    }

    private void PerformEnterInput()
    {
        if (!IsInputFieldSelected())
            return;

        Console.Current.ReadInput(_consoleInputField.text);
        ClearAndSelectInputField();
    }

    private void PerformHistoryRecallUpInput()
    {
        Console.Current.RecallHistory(-1);
    }

    private void PerformHistoryRecallDownInput()
    {
        Console.Current.RecallHistory(1);
    }

    private void PerformAcceptHintInput()
    {
        Console.Current.AcceptHint();
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
