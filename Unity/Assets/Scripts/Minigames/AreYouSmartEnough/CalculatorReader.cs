
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class CalculatorReader : SingletonBehaviour<CalculatorReader>
{
    private NumberDetector detector;

    public Action<int> numberCallback
    {
        get => detector.numberCallback;
        set => detector.numberCallback = value;
    }

    public Action enterCallback
    {
        get => detector.enterCallback;
        set => detector.enterCallback = value;
    }

    private void Start()
    {
        if (detector == null)
            detector = new NumberDetector();
    }

    private void OnDestroy()
    {
        detector.Dispose();
    }

    private void OnDisable()
    {
        detector.Dispose();
    }

    private void OnEnable()
    {
        if (detector == null)
            detector = new NumberDetector();
    }

    public class NumberDetector : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WM_KEYDOWN = 0x0100;

        public NumberDetector()
        {
            proc = HookCallback;
            hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(null), 0);
            numberCallback += log;
        }

        public void log(int a)
        {
            Debug.Log(a);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool temp = false; // I don't know why but 7 and 2 use the same signal, this bool differentiates them
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
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

        ~NumberDetector()
        {
            Dispose(false);
        }
    }
}