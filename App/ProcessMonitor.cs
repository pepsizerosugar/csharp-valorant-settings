using System;
using System.Collections.Generic;
using System.Threading;

namespace valorant_settings.App
{
    internal class ProcessMonitor
    {
        private readonly ColorController _cController = ColorController.Instance;

        private readonly HashSet<string> _pTargets = new HashSet<string>();
        private NativeMethods.WinEventDelegate _processHook;

        private ProcessMonitor()
        {
        }

        public MainForm Parent { get; set; }

        public void Add(string process)
        {
            _pTargets.Add(process);
        }

        public void Init()
        {
            _processHook = WinEventProc;
            NativeMethods.Dele += _processHook;
            NativeMethods.SetHook();

            // Init ColorController
            _cController.Init();
        }

        /**
         * Window Focus changed Event Handler
         */
        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            Console.WriteLine("\n[INFO][pMonitor] Running Tasks : {0}", GetWorkingThreads());
            Console.WriteLine(@"[INFO][pMonitor] Focused Process : {0}", NativeMethods.GetActiveWindowTitle());

            if (_pTargets.Contains(NativeMethods.GetActiveWindowTitle().ToLower()) && Parent.IsEnabled)
            {
                Console.WriteLine(@"[INFO][pMonitor] Target Process is focused");

                var (b, c, g, dvl) = Parent.GetColorValue();
                _cController.ChangeColorRamp(b,
                    c,
                    g,
                    false);
                _cController.Dvl = dvl;
            }
            else
            {
                Console.WriteLine(@"[INFO][pMonitor] Target Process is not focused");
                _cController.ChangeColorRamp(reset: true);
                _cController.ResetDvl();
            }
        }

        /**
         * Reset to original color settings before exit
         */
        public void Close()
        {
            Console.WriteLine(@"[INFO][pMonitor] Remove Delegates");
            NativeMethods.Dele -= _processHook;
            NativeMethods.UnHook();

            Console.WriteLine(@"[INFO][pMonitor] Resetting Color");
            _cController.Close();
        }

        private static int GetWorkingThreads()
        {
            ThreadPool.GetMaxThreads(out var maxThreads, out _);
            ThreadPool.GetAvailableThreads(out var availableThreads, out _);
            return maxThreads - availableThreads;
        }

        #region Singleton Pattern implement

        private static readonly Lazy<ProcessMonitor> instance =
            new Lazy<ProcessMonitor>(() => new ProcessMonitor());

        public static ProcessMonitor Instance => instance.Value;

        #endregion
    }
}