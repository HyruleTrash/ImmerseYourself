using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class DisplayChanger : MonoBehaviour 
{
    [DllImport("user32.dll")]
    static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    private List<RECT> monitors = new List<RECT>();

    void Start()
    {
        // Initialize monitors list
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, 
            delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                monitors.Add(lprcMonitor);
                return true;
            }, IntPtr.Zero);
    }

    public void MoveToMonitor(int monitorId)
    {
        if (monitorId < 0 || monitorId >= monitors.Count)
        {
            Debug.LogError($"Invalid monitor ID: {monitorId}. Available monitors: {monitors.Count}");
            return;
        }

        var targetRect = monitors[monitorId];
    
        // Get current window handle
        IntPtr hwnd = GetActiveWindow();
    
        // Calculate centered position
        int centerX = targetRect.left + (targetRect.right - targetRect.left) / 2;
        int centerY = targetRect.top + (targetRect.bottom - targetRect.top) / 2;
    
        // Use target monitor's dimensions instead of current window size
        int newX = centerX - (targetRect.right - targetRect.left) / 2;
        int newY = centerY - (targetRect.bottom - targetRect.top) / 2;
    
        // Move window to center of target monitor with monitor's dimensions
        MoveWindow(hwnd, newX, newY, 
            targetRect.right - targetRect.left,  // Use monitor width
            targetRect.bottom - targetRect.top,  // Use monitor height
            true);
    }
}