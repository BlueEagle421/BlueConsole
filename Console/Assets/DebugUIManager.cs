using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class DebugUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text cpuCounterText;
    [SerializeField] private float updateInterval = 1;
    public float CpuUsage;

    private Thread _cpuThread;
    private float _lastCpuUsage;
    private int _processorCount;

    private void Start()
    {
        _processorCount = Environment.ProcessorCount;
        UnityEngine.Debug.Log("Processor count: " + _processorCount);

        Application.runInBackground = true;

        cpuCounterText.text = "0% CPU";

        // setup the thread
        _cpuThread = new Thread(UpdateCPUUsage)
        {
            IsBackground = true,
            // we don't want that our measurement thread
            // steals performance
            Priority = System.Threading.ThreadPriority.BelowNormal
        };

        // start the cpu usage thread
        _cpuThread.Start();
    }

    private void OnDisable()
    {
        // Just to be sure kill the thread if this object is disabled
        _cpuThread?.Abort();
    }

    private void Update()
    {
        // for more efficiency skip if nothing has changed
        if (Mathf.Approximately(_lastCpuUsage, CpuUsage)) return;

        // the first two values will always be "wrong"
        // until _lastCpuTime is initialized correctly
        // so simply ignore values that are out of the possible range
        if (CpuUsage < 0 || CpuUsage > 100) return;

        // I used a float instead of int for the % so use the ToString you like for displaying it
        cpuCounterText.text = ((int)CpuUsage).ToString("F1") + "% CPU";

        // Update the value of _lasCpuUsage
        _lastCpuUsage = CpuUsage;
    }

    private void UpdateCPUUsage()
    {
        var lastCpuTime = new TimeSpan(0);

        // This is ok since this is executed in a background thread
        while (true)
        {
            var cpuTime = new TimeSpan(0);

            // Get a list of all running processes in this PC
            var AllProcesses = Process.GetProcesses();

            // Sum up the total processor time of all running processes
            cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);

            // get the difference between the total sum of processor times
            // and the last time we called this
            var newCPUTime = cpuTime - lastCpuTime;

            // update the value of _lastCpuTime
            lastCpuTime = cpuTime;

            // The value we look for is the difference, so the processor time all processes together used
            // since the last time we called this divided by the time we waited
            // Then since the performance was optionally spread equally over all physical CPUs
            // we also divide by the physical CPU count
            CpuUsage = 100f * (float)newCPUTime.TotalSeconds / updateInterval / _processorCount;

            // Wait for UpdateInterval
            Thread.Sleep(Mathf.RoundToInt(updateInterval * 1000));
        }
    }
}