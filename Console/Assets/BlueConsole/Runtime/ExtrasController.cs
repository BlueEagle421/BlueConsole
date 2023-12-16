using TMPro;
using UnityEngine;

public class ExtrasController : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsTMP;
    [SerializeField] private RectTransform _consoleHeaderTextRect;

    private void Awake()
    {
        SetEvents(true);
    }

    private void OnDisable()
    {
        SetEvents(false);
    }

    private void Update()
    {
        FormatTextFPS();
    }

    private void SetEvents(bool subscribe)
    {
        Console.Current.OnConsoleToggled += OnConsoleToggled;
        Extras.OnFPSToggled += OnFPSToggled;
    }

    private void OnConsoleToggled(bool toggled)
    {
        SetConsoleHeaderTextRect();
    }

    private void OnFPSToggled(bool toggled)
    {
        _fpsTMP.gameObject.SetActive(toggled);
        SetConsoleHeaderTextRect();
    }
    private void FormatTextFPS()
    {
        if (!Extras.IsFPSToggled)
            return;

        _fpsTMP.text = Extras.CurrentFPSFormatted();
    }

    private void SetConsoleHeaderTextRect()
    {
        _consoleHeaderTextRect.gameObject.SetActive(!Extras.IsFPSToggled);
    }
}
