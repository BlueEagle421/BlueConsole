using TMPro;
using UnityEngine;

public class DiagnosticsController : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsTMP;
    [SerializeField] private TMP_Text _usageTMP;
    [SerializeField] private RectTransform _consoleHeaderTextRect;
    [SerializeField] private Color _fpsColor;
    [SerializeField] private Gradient _usageGradient;

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
        ConsoleUtils.SetActionListener(ref Diagnostics.OnFPSToggled, OnFPSToggled, subscribe);
    }

    private void OnConsoleToggled(bool toggled)
    {
        SetConsoleHeaderTextRect();
    }

    private void OnUsageToggled(bool toggled)
    {
        _usageTMP.gameObject.SetActive(toggled);
    }

    private void OnFPSToggled(bool toggled)
    {
        _fpsTMP.gameObject.SetActive(toggled);
        SetConsoleHeaderTextRect();
    }
    private void FormatTextFPS()
    {
        if (!Diagnostics.IsFPSToggled)
            return;

        string colorHex = ColorUtility.ToHtmlStringRGB(_fpsColor);
        _fpsTMP.text = string.Format("<color=#{0}>{1}</color>", colorHex, Diagnostics.CurrentFPSFormatted());
    }

    private void SetConsoleHeaderTextRect()
    {
        _consoleHeaderTextRect.gameObject.SetActive(!Diagnostics.IsFPSToggled);
    }
}
