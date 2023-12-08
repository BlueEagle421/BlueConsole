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
        ConsoleUtils.SetActionListener(ref Diagnostics.OnUsageToggled, OnUsageToggled, subscribe);
        ConsoleUtils.SetActionListener(ref Diagnostics.OnCpuUsageChanged, FormatTextUsage, subscribe);
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
        FormatTextUsage();
    }
    private void FormatTextFPS()
    {
        if (!Diagnostics.IsFPSToggled)
            return;

        string colorHex = ColorUtility.ToHtmlStringRGB(_fpsColor);
        _fpsTMP.text = string.Format("<color=#{0}>{1}</color>", colorHex, Diagnostics.CurrentFPSFormatted());
    }

    private void FormatTextUsage()
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(_usageGradient.Evaluate(Diagnostics.CpuUsage / 100f));
        _usageTMP.text = string.Format("CPU:<color=#{0}>({1}%)</color>", colorHex, (int)Diagnostics.CpuUsage);
    }

    private void SetConsoleHeaderTextRect()
    {
        _consoleHeaderTextRect.gameObject.SetActive(!Diagnostics.IsFPSToggled);
    }
}
