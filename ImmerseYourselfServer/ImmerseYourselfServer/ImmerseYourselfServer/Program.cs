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
            
            Server.Start(count);
            
            Console.ReadKey();
        }
    }
}