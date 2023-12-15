using TMPro;
using UnityEngine;

public class ExtrasController : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsTMP;
    [SerializeField] private RectTransform _consoleHeaderTextRect;
    [SerializeField] private Color _fpsColor;

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
        ConsoleUtils.SetActionListener(ref Console.OnConsoleToggled, OnConsoleToggled, subscribe);
        ConsoleUtils.SetActionListener(ref Extras.OnFPSToggled, OnFPSToggled, subscribe);
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

        string colorHex = ColorUtility.ToHtmlStringRGB(_fpsColor);
        _fpsTMP.text = string.Format("<color=#{0}>{1}</color>", colorHex, Extras.CurrentFPSFormatted());
    }

    private void SetConsoleHeaderTextRect()
    {
        _consoleHeaderTextRect.gameObject.SetActive(!Extras.IsFPSToggled);
    }
}
