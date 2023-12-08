using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Diagnostics : MonoBehaviour
{
    [SerializeField] private RectTransform _diagnosticsGUIParent;
    [SerializeField] private float updateInterval = 0.6f;
    [SerializeField] private Color _diagnosticsColor;
    [SerializeField] private Gradient _usageGradient;
    public static bool IsFullySupported { get; private set; }
    public static bool IsFPSToggled { get; private set; }
    public static bool IsHWUsageToggled { get; private set; }
    public static int PhysicalProcessors { get; private set; }
    public static float CpuUsage { get; private set; }
    private static bool _wasEnabledGlobally;
    public static Action<bool> OnFPSToggled;
    public static Action<bool> OnUsageToggled;
    public static Action OnCpuUsageChanged;



    private Thread _cpuThread;
    private float _lastCpuUsage;

    private void Start()
    {
        if (IsFPSToggled)
            FPS(true);

        if (IsHWUsageToggled)
            HWUsage(true);

        //cpuCounterText.text = "0% CPU";
    }

    private void OnEnable()
    {
        if (!_wasEnabledGlobally)
            FirstGlobalEnable();
    }

    private void OnDisable()
    {
        StopCPUUsageThread();
    }

    private void FirstGlobalEnable()
    {
        _wasEnabledGlobally = true;

        IsFullySupported = SystemInfo.operatingSystemFamily != OperatingSystemFamily.Other;

        PhysicalProcessors = Environment.ProcessorCount / 2;
    }

    private void Update()
    {
        UpdateCPUUsage();
    }

    private void UpdateCPUUsage()
    {
        if (Mathf.Approximately(_lastCpuUsage, CpuUsage)) return;

        if (CpuUsage < 0 || CpuUsage > 100) return;

        OnCpuUsageChanged?.Invoke();
        //cpuCounterText.text = ((int)CpuUsage).ToString("F1") + "% CPU";

        _lastCpuUsage = CpuUsage;
    }

    private void ThreadCPUUsage()
    {
        var lastCpuTime = new TimeSpan(0);

        while (true)
        {
            var cpuTime = new TimeSpan(0);

            var AllProcesses = Process.GetProcesses();

            cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);

            var newCPUTime = cpuTime - lastCpuTime;

            lastCpuTime = cpuTime;

            CpuUsage = 100f * (float)newCPUTime.TotalSeconds / updateInterval / PhysicalProcessors;

            Thread.Sleep(Mathf.RoundToInt(updateInterval * 1000));
        }
    }

    private void StartCPUUsageThread()
    {
        _cpuThread = new Thread(ThreadCPUUsage)
        {
            IsBackground = true,
            Priority = System.Threading.ThreadPriority.BelowNormal
        };

        _cpuThread.Start();

        OnCpuUsageChanged?.Invoke();
    }

    private void StopCPUUsageThread()
    {
        _cpuThread?.Abort();
    }

    [Command("fps", "toggles fps counter", InstanceTargetType.First)]
    public void FPS(bool on)
    {
        IsFPSToggled = on;
        OnFPSToggled.Invoke(on);
    }

    [Command("hwusage", "toggles hardware usage counter", InstanceTargetType.First)]
    public void HWUsage(bool on)
    {
        if (!IsFullySupported)
        {
            UnityEngine.Debug.Log("hwusage is disabled for this operating system");
            return;
        }

        if (on)
        {
            StartCPUUsageThread();
            Application.runInBackground = true;
        }
        else
        {
            StopCPUUsageThread();
            Application.runInBackground = false;
        }

        IsHWUsageToggled = on;
        OnUsageToggled.Invoke(on);
    }

    [Command("osinfo", "logs operating system information", InstanceTargetType.First)]
    public void OsInfo()
    {
        string hexDiagnosticsColor = ColorUtility.ToHtmlStringRGB(_diagnosticsColor);
        UnityEngine.Debug.Log(string.Format("OS: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.operatingSystemFamily.ToString()));
    }

    [Command("hwinfo", "logs hardware information", InstanceTargetType.First)]
    public void HwInfo()
    {
        if (!IsFullySupported)
        {
            UnityEngine.Debug.Log("hwinfo is disabled for this operating system");
            return;
        }

        string hexDiagnosticsColor = ColorUtility.ToHtmlStringRGB(_diagnosticsColor);
        UnityEngine.Debug.Log(string.Format("CPU: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.processorType));
        UnityEngine.Debug.Log(string.Format("GPU: <color=#{0}>{1}</color>", hexDiagnosticsColor, SystemInfo.graphicsDeviceName));
    }

    [Command("diagnose", "toggles fps, sysinfo, hwinfo and hwusage", InstanceTargetType.First)]
    public void Diagnose(bool on)
    {
        if (on)
        {
            OsInfo();
            HwInfo();
        }
        FPS(on);
        HWUsage(on);
    }
}
