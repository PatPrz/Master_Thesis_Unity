using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine.Profiling;
using System.Runtime.InteropServices;

public class StartupResearchManager
{
    private static string startupCsvPath;

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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnGameStarted()
    {
        float startupDurationMs = Time.realtimeSinceStartup * 1000f;

        string rootPath = Directory.GetParent(Application.dataPath).FullName;

        string folderName = $"Unity_Research_{DateTime.Now:yyyyMMdd_HHmm}";
        string fullPath = Path.Combine(rootPath, folderName);

        if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

        startupCsvPath = Path.Combine(fullPath, $"StartupLog_{DateTime.Now:yyyyMMdd_HHmm}.csv");

        Process currentProcess = Process.GetCurrentProcess();
        PROCESS_MEMORY_COUNTERS_EX counters;
        float ramMB = 0;
        if (GetProcessMemoryInfo(currentProcess.Handle, out counters, (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS_EX))))
            ramMB = (long)counters.WorkingSetSize / 1024f / 1024f;

        float vramMB = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f;
        double cpuUsage = (currentProcess.TotalProcessorTime.TotalMilliseconds / (Time.realtimeSinceStartup * 1000.0 * Environment.ProcessorCount)) * 100;

        string header = "Startup_Duration_ms;RAM_Initial_MB;VRAM_Initial_MB;CPU_Initial_%\n";
        string data = string.Format("{0:F0};{1:F2};{2:F2};{3:F1}\n", startupDurationMs, ramMB, vramMB, cpuUsage);

        try
        {
            File.WriteAllText(startupCsvPath, header + data);
        }
        catch (Exception e) { UnityEngine.Debug.LogError(e.Message); }
    }
}