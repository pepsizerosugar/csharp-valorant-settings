using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using valorant_settings.GPU;

namespace valorant_settings.Display
{
    internal abstract class Display
    {
        private static readonly IGpu Gpu;

        public static readonly List<string> Displays;
        private static string _primary;

        static Display()
        {
            Displays = GetWinDisplays();
            Gpu = GpuDevice.Instance;
        }

        public static string Primary
        {
            get => _primary;
            set
            {
                _primary = Displays.Contains(value) ? value : Displays[0];
                try
                {
                    Gpu.Load(_primary);
                }
                catch (NotImplementedException)
                {
                }
            }
        }

        [DllImport("gdi32")]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput,
            IntPtr lpInitData);

        [DllImport("gdi32")]
        public static extern bool DeleteDC([In] IntPtr hdc);

        private static List<string> GetWinDisplays()
        {
            return Screen.AllScreens.Select(screen => screen.DeviceName).ToList();
        }
    }
}