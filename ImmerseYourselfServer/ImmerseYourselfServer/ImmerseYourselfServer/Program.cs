using System;
using System.Runtime.InteropServices;

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
        
        static void Main(string[] args)
        {
            Console.Title = "ImmerseYourselfServer";
            
            int count = 0;
            DISPLAY_DEVICE device = new DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);

            while (EnumDisplayDevices(null, count, ref device, 0))
            {
                count++;
                device.cb = Marshal.SizeOf(device);
            }
            
            Console.WriteLine($"Number of monitors: {count}");
            
            Server.Start(5);
            
            Console.ReadKey();
        }
    }
}