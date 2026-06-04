using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine.Profiling;
using System.Runtime.InteropServices;

public class EventResearchManager : MonoBehaviour
{
    public static EventResearchManager Instance;
    private string eventCsvPath;
    private Process currentProcess;
    private TimeSpan lastCpuTime;
    private float lastSampleTime;

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_MEMORY_COUNTERS_EX
    {
        public uint cb; public uint PageFaultCount; public UIntPtr PeakWorkingSetSize;
        public UIntPtr WorkingSetSize; public UIntPtr QuotaPeakPagedPoolUsage;
        public UIntPtr QuotaPagedPoolUsage; public UIntPtr QuotaPeakNonPagedPoolUsage;
        public UIntPtr QuotaNonPagedPoolUsage; public UIntPtr PagefileUsage;
        public UIntPtr PeakPagefileUsage; public UIntPtr PrivateUsage;
    }

    [DllImport("psapi.dll", SetLastError = true)]
    static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS_EX counters, uint size);

    void Awake()
    {
        if (Instance == null) Instance = this;
        currentProcess = Process.GetCurrentProcess();
        lastCpuTime = currentProcess.TotalProcessorTime;
        lastSampleTime = Time.realtimeSinceStartup;
    }

    void Start()
    {
        try
        {
            string rootPath = Directory.GetParent(Application.dataPath).FullName;

            string folderName = ResearchManager.SessionFolderName ?? $"Unity_Research_{DateTime.Now:yyyyMMdd_HHmm}";
            string fullPath = Path.Combine(rootPath, folderName);

            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

            eventCsvPath = Path.Combine(fullPath, $"EventLog_{DateTime.Now:yyyyMMdd_HHmm}.csv");
            string header = "Elapsed_Time;Event_Type;RAM_TaskMgr_MB;VRAM_MB;CPU_Snapshot_%;Latency_ms\n";
            File.WriteAllText(eventCsvPath, header);
        }
        catch (Exception e) { UnityEngine.Debug.LogError($"B³¹d pliku: {e.Message}"); }
    }

    public void LogEvent(string eventName, float inputStartTime = 0)
    {
        float elapsed = Time.realtimeSinceStartup;
        PROCESS_MEMORY_COUNTERS_EX counters;
        float ramMB = 0;
        if (GetProcessMemoryInfo(currentProcess.Handle, out counters, (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS_EX))))
            ramMB = (long)counters.WorkingSetSize / 1024f / 1024f;
        float vramMB = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f;

        TimeSpan currentCpuTime = currentProcess.TotalProcessorTime;
        float currentTime = Time.realtimeSinceStartup;
        double cpuUsedMs = (currentCpuTime - lastCpuTime).TotalMilliseconds;
        double totalMsPassed = (currentTime - lastSampleTime) * 1000.0;
        double cpuUsage = (cpuUsedMs / (totalMsPassed * Environment.ProcessorCount)) * 100;
        if (cpuUsage < 0 || double.IsInfinity(cpuUsage)) cpuUsage = 0;

        lastCpuTime = currentCpuTime;
        lastSampleTime = currentTime;

        float latency = (inputStartTime > 0) ? (currentTime - inputStartTime) * 1000f : 0;
        string line = string.Format("{0:F1}s;{1};{2:F2};{3:F2};{4:F1};{5:F4}\n", elapsed, eventName, ramMB, vramMB, cpuUsage, latency);
        File.AppendAllText(eventCsvPath, line);
    }
}