using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class Extras : MonoBehaviour
{
    public static bool IsFPSToggled { get; private set; }
    public static Extras Current;
    public Action<bool> OnFPSToggled;

    private static float[] _frameDeltaTimings = new float[50];
    private int _lastFrameIndex;

    private void Awake()
    {
        Current = this;
    }

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

    [Command("os-info", "logs operating system information")]
    public static void OsInfo()
    {
        Debug.Log(string.Format("OS: {0}", SystemInfo.operatingSystemFamily.ToString()));
    }

    [Command("hw-info", "logs hardware information")]
    public static void HwInfo()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Other)
        {
            Debug.Log("hwinfo is disabled for this operating system");
            return;
        }

        Debug.Log(string.Format("CPU: {0}", SystemInfo.processorType));
        Debug.Log(string.Format("GPU: {0}", SystemInfo.graphicsDeviceName));
    }

    [Command("player-info", "logs player information")]
    public static void PlayerInfo()
    {
        Debug.Log(string.Format("Company Name: {0}", Application.companyName));
        Debug.Log(string.Format("Product Name: {0}", Application.productName));
        Debug.Log(string.Format("Version: {0}", Application.version));
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

    [Command("log-error", "logs an error without stack trace")]
    public static void Log_Error(string message, bool trace)
    {
        Debug.LogError(message + (trace ? string.Empty : ConsoleProcessor.NO_TRACE));
    }

    [Command("log-warning", "logs warning")]
    public static void Log_Warning(string message)
    {
        Debug.LogWarning(message);
    }

    [Command("date", "logs current date")]
    public static void Date()
    {
        Debug.Log(DateTime.Now);
    }

    [Command("time", "logs engine time")]
    public static void Time()
    {
        Debug.Log("Time.time: " + UnityEngine.Time.time);
        Debug.Log("Time.unscaledTime: " + UnityEngine.Time.unscaledTime);
        Debug.Log("Time.timeScale: " + UnityEngine.Time.timeScale);
    }

    [Command("timescale", "sets time scale")]
    public static void Timescale(float value)
    {
        UnityEngine.Time.timeScale = value;
    }
}
