using UnityEngine;

public class HeaderEntriesVisuals : MonoBehaviour
{
    [SerializeField] private RectTransform _consoleHeaderTextRect;

    private void OnEnable()
    {
        ConsoleProcessor.Current.OnConsoleToggled += OnConsoleToggled;
    }

    private void OnDisable()
    {
        ConsoleProcessor.Current.OnConsoleToggled -= OnConsoleToggled;
    }

    private void OnConsoleToggled(bool toggled)
    {
        SetConsoleHeaderTextRect();
    }

    private void SetConsoleHeaderTextRect()
    {

    }
}
