using System;
using UnityEngine;

public class Extras : MonoBehaviour
{
    [SerializeField] private RectTransform _diagnosticsGUIParent;
    [SerializeField] private Color _diagnosticsLogColor;
    public static bool IsFPSToggled { get; private set; }
    public static Action<bool> OnFPSToggled;

    private static float[] _frameDeltaTimings;
    private int _lastFrameIndex;

    private void Start()
    {
        if (IsFPSToggled)
            FPS(true);
    }

    private void Update()
    {
        UpdateFPSLastFrame();
    }

    private void UpdateFPSLastFrame()
    {
        _frameDeltaTimings[_lastFrameIndex] = Time.unscaledDeltaTime;
        _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimings.Length;
    }

    public static float CurrentFPS()
    {
        float total = 0f;
        foreach (float deltaTime in _frameDeltaTimings)
        {
            total += deltaTime;
        }
        return _frameDeltaTimings.Length / total;
    }

    public static string CurrentFPSFormatted()
    {
        return Mathf.RoundToInt(CurrentFPS()).ToString();
    }

    [Command("fps", "toggles fps counter", InstanceTargetType.First)]
    public void FPS(bool on)
    {
        IsFPSToggled = on;
        OnFPSToggled.Invoke(on);
    }

    [Command("osinfo", "logs operating system information", InstanceTargetType.First)]
    public void OsInfo()
    {
        string hexDiagnosticsColor = ColorUtility.ToHtmlStringRGB(_diagnosticsLogColor);
        UnityEngine.Debug.Log(string.Format("OS: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.operatingSystemFamily.ToString()));
    }

    [Command("hwinfo", "logs hardware information", InstanceTargetType.First)]
    public void HwInfo()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Other)
        {
            UnityEngine.Debug.Log("hwinfo is disabled for this operating system");
            return;
        }

        string hexDiagnosticsColor = ColorUtility.ToHtmlStringRGB(_diagnosticsLogColor);
        UnityEngine.Debug.Log(string.Format("CPU: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.processorType));
        UnityEngine.Debug.Log(string.Format("GPU: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.graphicsDeviceName));
    }
}
