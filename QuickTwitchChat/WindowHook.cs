using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace QuickTwitchChat;

public static class WindowHook
{
    [DllImport("user32.dll")]
    internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void FocusProcess()
    {
        IntPtr hWnd;
        Process[] processRunning = Process.GetProcesses();
        foreach (Process pr in processRunning)
        {
            if (pr.ProcessName.Contains("QuickTwitchChat"))
            {
                hWnd = pr.MainWindowHandle;
                ShowWindow(hWnd, 1);
                SetForegroundWindow(hWnd);
            }
        }
    }
}