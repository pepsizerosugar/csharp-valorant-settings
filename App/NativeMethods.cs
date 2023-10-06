using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace valorant_settings.App
{
    internal static class NativeMethods
    {
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject,
            int idChild, uint dwEventThread, uint dwmsEventTime);

        private const uint WineventOutofcontext = 0;
        private const uint EventSystemForeground = 3;

        public static WinEventDelegate Dele;

        private static IntPtr _mHhook;

        public static void SetHook()
        {
            _mHhook = SetWinEventHook(EventSystemForeground,
                EventSystemForeground,
                IntPtr.Zero,
                Dele,
                0, 0, WineventOutofcontext | 2);
        }

        public static void UnHook()
        {
            UnhookWinEvent(_mHhook);
        }

        public static string GetActiveWindowTitle()
        {
            try
            {
                var handle = GetForegroundWindow();
                var threadId = GetWindowThreadProcessId(handle, out var processId);
                return Process.GetProcessById(Convert.ToInt32(processId)).ProcessName;
            }
            catch
            {
                return null;
            }
        }

        #region Win32 API Calls

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #endregion
    }
}