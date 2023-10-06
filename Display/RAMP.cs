using System;
using System.Runtime.InteropServices;

namespace valorant_settings.Display
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Ramp
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public UInt16[] Red;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public UInt16[] Green;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public UInt16[] Blue;
    }
}