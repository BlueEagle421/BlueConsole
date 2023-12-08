using TMPro;
using UnityEngine;

public class DiagnosticsController : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsTMP;
    [SerializeField] private TMP_Text _usageTMP;
    [SerializeField] private RectTransform _consoleHeaderTextRect;
    [SerializeField] private Gradient _usageGradient;

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

    }

    private void FormatTextUsage()
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(_usageGradient.Evaluate(Diagnostics.CpuUsage / 100f));
        _usageTMP.text = string.Format("CPU: <color=#{0}>({1}%)</color>", colorHex, (int)Diagnostics.CpuUsage);
        //_usageTMP.text = ((int)Diagnostics.CpuUsage).ToString("F1") + "% CPU";
    }

    private void SetConsoleHeaderTextRect()
    {
        _consoleHeaderTextRect.gameObject.SetActive(!Diagnostics.IsFPSToggled);
    }
}
