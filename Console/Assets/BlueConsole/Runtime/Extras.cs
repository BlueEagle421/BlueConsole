using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Extras : MonoBehaviour
{
    public static bool IsFPSToggled { get; private set; }
    public static Action<bool> OnFPSToggled;

    private static float[] _frameDeltaTimings = new float[50];
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
        _frameDeltaTimings[_lastFrameIndex] = UnityEngine.Time.unscaledDeltaTime;
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

    [Command("osinfo", "logs operating system information")]
    public static void OsInfo()
    {
        string hexDiagnosticsColor = ColorUtility.ToHtmlStringRGB(Console.Current.AdditionalColor);
        Debug.Log(string.Format("OS: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.operatingSystemFamily.ToString()));
    }

    [Command("hwinfo", "logs hardware information")]
    public static void HwInfo()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Other)
        {
            Debug.Log("hwinfo is disabled for this operating system");
            return;
        }

        string hexDiagnosticsColor = ColorUtility.ToHtmlStringRGB(Console.Current.AdditionalColor);
        Debug.Log(string.Format("CPU: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.processorType));
        Debug.Log(string.Format("GPU: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.graphicsDeviceName));
    }

    [Command("quit", "closes the application")]
    public static void Quit()
    {
        Application.Quit();
    }

    [Command("reset", "resets current scene")]
    public static void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    [Command("version", "logs project name and version")]
    public static void Version()
    {
        Debug.Log(string.Format("{0} version: {1}", Application.productName, Application.version));
    }

    [Command("log", "logs")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    [Command("log_error", "logs an error without stack trace")]
    public static void Log_Error(string message, bool trace)
    {
        Debug.LogError(message + (trace ? string.Empty : Console.NO_TRACE));
    }

    [Command("log_warning", "logs warning")]
    public static void Log_Warning(string message)
    {
        Debug.LogWarning(message);
    }

    [Command("time", "logs current time")]
    public static void Time()
    {
        Debug.Log(DateTime.Now);
    }
}
