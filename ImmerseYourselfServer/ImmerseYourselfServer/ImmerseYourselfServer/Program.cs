using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ImmerseYourselfServer
{
    class ServerProgram
    {
        [DllImport("user32.dll")]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }
        
        private static bool isRunning = false;
        private static DiscordBotManager discordBotManager;
        
        static void Main(string[] args)
        {
            Console.Title = "ImmerseYourselfServer";
            
            int count = 0;
            DISPLAY_DEVICE device = new DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);

            // Enumerate all display devices
            uint deviceIndex = 0;
            while (EnumDisplayDevices(null, deviceIndex, ref device, 0))
            {
                // Only count devices that are actually connected (attached)
                if ((device.StateFlags & 0x01) != 0) // DEVICE_ATTACHED_TO_DESKTOP
                {
                    count++;
                    Console.WriteLine($"Found monitor: {device.DeviceString}");
                }
            
                device.cb = Marshal.SizeOf(device);
                deviceIndex++;
            }
            
            Console.WriteLine($"Number of monitors: {count}");
            
            // Server.Start(count); TODO: remove temp monitor detect
            Server.Start(2);
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
            
            // try // TODO: uncomment when testing release
            // {
            //     string path = AppDomain.CurrentDomain.BaseDirectory;
            //     int levelsUp = 7; // how many directories to go up
            //
            //     for (int i = 0; i < levelsUp; i++)
            //     {
            //         path = Directory.GetParent(path)!.FullName;
            //     }
            //
            //     string exePath = Path.Combine(path, "Unity/.Builds/ImmerseYourself.exe");
            //     
            //     ProcessStartInfo psi = new ProcessStartInfo()
            //     {
            //         FileName = exePath,                     // change to your exe path
            //         Verb = "runas",                         // request admin privileges
            //         UseShellExecute = true                  // must be true for Verb to work
            //     };
            //
            //     for (int i = 0; i < count; i++)
            //     {
            //         Process.Start(psi);
            //     }
            // }
            // catch (System.ComponentModel.Win32Exception ex)
            // {
            //     Console.WriteLine("The user refused the elevation: " + ex.Message);
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine("Error launching process: " + ex.Message);
            // }

            discordBotManager = new DiscordBotManager();
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SECOND} ticks per second.");
            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    GameLoop.Update();
                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}