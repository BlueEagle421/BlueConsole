using System;
using UnityEngine;

public class Diagnostics : MonoBehaviour
{
    [SerializeField] private RectTransform _diagnosticsGUIParent;
    [SerializeField] private Color _diagnosticsColor;
    public static bool IsFullySupported { get; private set; }
    public static bool IsToggled { get; private set; }
    private static bool _wasEnabledGlobally;
    private static bool _wasToggledGlobally;
    private bool _wasToggledInScene;
    public Action<bool> OnDiagnosticsToggled;

    private void OnEnable()
    {
        if (!_wasEnabledGlobally)
            FirstGlobalEnable();
    }

    private void FirstGlobalEnable()
    {
        _wasEnabledGlobally = true;
        IsFullySupported = SystemInfo.operatingSystemFamily != OperatingSystemFamily.Other;
    }

    private void Start()
    {
        if (IsToggled)
            ToggleDiagnostics(true);
    }

    private void ToggleDiagnostics(bool toggle)
    {
        CheckFirstToggles();

        if (toggle)
            ToggledOn();

        IsToggled = toggle;

    }

    private void CheckFirstToggles()
    {
        if (!_wasToggledGlobally)
            FirstGlobalToggleOn();

        if (!_wasToggledInScene)
            FirstToggleOnInScene();
    }

    private void FirstGlobalToggleOn()
    {
        _wasToggledGlobally = true;

    }

    private void FirstToggleOnInScene()
    {
        _wasToggledInScene = true;
    }

    private void ToggledOn()
    {
        LogSystemInfo();
    }

    private void LogSystemInfo()
    {
        string hexDiagnosticsColor = ColorUtility.ToHtmlStringRGB(_diagnosticsColor);
        Debug.Log(string.Format("OS type: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.operatingSystemFamily.ToString()));

        if (IsFullySupported)
        {
            Debug.Log(string.Format("CPU type: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.processorType));
            Debug.Log(string.Format("GPU type: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.graphicsDeviceName));
        }
        else
        {
            Debug.LogWarning("This operating system is not fully supported, and some diagnostics functionality might be missing");
        }
    }

    [Command("hwinfo", "logs os, cpu and gpu types", InstanceTargetType.First)]
    public void Hwinfo()
    {
        LogSystemInfo();
    }

    [Command("diagnose", "toggles fps, hwinfo and hwusage", InstanceTargetType.First)]
    public void Diagnose(bool on)
    {
        Hwinfo();
        ToggleDiagnostics(on);
    }
}
