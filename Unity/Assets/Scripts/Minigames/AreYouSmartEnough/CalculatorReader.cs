
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityRawInput;

public class CalculatorReader : SingletonBehaviour<CalculatorReader>
{
    public bool runButton = false;
    
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam); // Changed return type to IntPtr
    private LowLevelKeyboardProc proc;
    private IntPtr hookId;

    public Action<int> numberCallback;
    public Action enterCallback;
    
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hInstance, uint threadId);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    public CalculatorReader()
    {
        proc = HookCallback;
    }

    private uint currentThreadId;
    public void TurnOn()
    {
        if (hookId != IntPtr.Zero)
            return;
        
        hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, IntPtr.Zero, 0);
        if (hookId == IntPtr.Zero)
        {
            int errorCode = Marshal.GetLastWin32Error();
            Debug.LogError($"Failed to set hook: Error code {errorCode}");
        }
        else
        {
            Debug.Log($"Hook successful! {hookId}");
        }
    }
    
    public void TurnOff()
    {
        if (hookId != IntPtr.Zero)
        {
            bool success = UnhookWindowsHookEx(hookId);
            hookId = IntPtr.Zero;
            
            if (!success)
                Debug.LogError("Failed to remove hook");
        }
    }

    private void Update()
    {
        if (runButton)
        {
            TurnOn();
            runButton = false;
        }
    }

    private void OnApplicationQuit()
    {
        TurnOff();
    }

    bool temp = false; // I don't know why but 7 and 2 use the same signal, this bool differentiates them
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        Debug.Log(nCode);
        
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);

            switch (vkCode)
            {
                case 0xA0:
                    temp = true;
                    break;
                // Check for numpad numbers (0-9)
                case 114:
                    numberCallback?.Invoke(0);
                    break;
                case 113:
                    numberCallback?.Invoke(1);
                    break;
                case 122:
                    if (temp)
                    {
                        numberCallback?.Invoke(2);
                        temp = false;
                    }
                    else
                        numberCallback?.Invoke(7);
                    break;
                case 118:
                    numberCallback?.Invoke(3);
                    break;
                case 112:
                    numberCallback?.Invoke(4);
                    break;
                case 121:
                    numberCallback?.Invoke(5);
                    break;
                case 117:
                    numberCallback?.Invoke(6);
                    break;
                case 120:
                    numberCallback?.Invoke(8);
                    break;
                case 116:
                    numberCallback?.Invoke(9);
                    break;
                case 0x0D:
                case 0xE0:
                    enterCallback?.Invoke();
                    break;
                default:
                    Debug.Log($"pressed: {vkCode:X4}");
                    break;
            }
            Debug.Log($"pressed: {vkCode:X4}");
        }
        return CallNextHookEx(hookId, nCode, wParam, lParam);
    }
}