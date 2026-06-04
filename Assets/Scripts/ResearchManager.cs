using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UnityEngine.Profiling;
using System.Collections;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance;
    public static string SessionFolderName { get; private set; }

    public TextMeshProUGUI timerText;
    public GameObject completionPanel;
    public float sessionDurationInMinutes = 10f;
    public float logInterval = 1.0f;

    private float timeRemaining;
    private bool isResearchFinished = false;
    private bool isInitialized = false;
    private string csvPath;
    private Process currentProcess;

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

        if (string.IsNullOrEmpty(SessionFolderName))
        {
            SessionFolderName = $"Unity_Research_{DateTime.Now:yyyyMMdd_HHmm}";
        }
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        timeRemaining = sessionDurationInMinutes * 60f;
        if (timeRemaining > 0) isInitialized = true;
        if (completionPanel != null) completionPanel.SetActive(false);
        Time.timeScale = 1f;

        try
        {
            string rootPath = Directory.GetParent(Application.dataPath).FullName;
            string fullPath = Path.Combine(rootPath, SessionFolderName);
            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

            csvPath = Path.Combine(fullPath, $"ResearchLog_{DateTime.Now:yyyyMMdd_HHmm}.csv");
            string header = "Elapsed_Time;FPS;RAM_TaskMgr_MB;VRAM_MB;CPU_Snapshot_%\n";
            File.WriteAllText(csvPath, header);
        }
        catch (Exception e) { UnityEngine.Debug.LogError($"B³¹d CSV: {e.Message}"); }

        StartCoroutine(LoggingRoutine());
    }

    IEnumerator LoggingRoutine()
    {
        while (!isResearchFinished)
        {
            TimeSpan startCpuTime = currentProcess.TotalProcessorTime;
            float startTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(0.2f);
            TimeSpan endCpuTime = currentProcess.TotalProcessorTime;
            float endTime = Time.realtimeSinceStartup;

            double cpuUsedMs = (endCpuTime - startCpuTime).TotalMilliseconds;
            double totalMsPassed = (endTime - startTime) * 1000.0;
            double cpuUsage = (cpuUsedMs / (totalMsPassed * Environment.ProcessorCount)) * 100;

            PROCESS_MEMORY_COUNTERS_EX counters;
            GetProcessMemoryInfo(currentProcess.Handle, out counters, (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS_EX)));
            float ramTaskMgr = (long)counters.WorkingSetSize / 1024f / 1024f;
            float vramMB = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f;
            float fps = 1.0f / Time.unscaledDeltaTime;

            float elapsed = (sessionDurationInMinutes * 60) - timeRemaining;
            string logLine = string.Format("{0:F1}s;{1:F1};{2:F2};{3:F2};{4:F1}\n", elapsed, fps, ramTaskMgr, vramMB, cpuUsage);
            File.AppendAllText(csvPath, logLine);

            yield return new WaitForSeconds(logInterval - 0.2f);
        }
    }

    void Update()
    {
        if (isResearchFinished || !isInitialized) return;
        if (timeRemaining > 0) { timeRemaining -= Time.deltaTime; UpdateTimerUI(); }
        else { FinishResearch(); }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;
        float elapsed = (sessionDurationInMinutes * 60) - timeRemaining;
        int minutes = Mathf.FloorToInt(Mathf.Max(0, elapsed) / 60);
        int seconds = Mathf.FloorToInt(Mathf.Max(0, elapsed) % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void FinishResearch()
    {
        if (isResearchFinished) return;
        isResearchFinished = true;
        Time.timeScale = 0f;
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CloseApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}