using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using valorant_settings.Display;
using valorant_settings.GPU;

namespace valorant_settings.App
{
    internal class ColorController
    {
        private readonly IGpu _gpu = GpuDevice.Instance;

        /**
         * _canceller : Token Source to abort Async-Task (Gamma Value Change)
         * WHY : *I don't know why* set gamma ramp keeps revert soon after modified
         */
        private CancellationTokenSource _canceller;

        // Gamma Ramps
        private Ramp _currentRamps;
        private Ramp _originalRamps;

        private ColorController()
        {
        }

        public int Dvl
        {
            get => _gpu.Saturation;
            set => _gpu.Saturation = value;
        }

        public void Init()
        {
            // Backup Gamma Ramp
            var hdc = IntPtr.Zero;
            try
            {
                hdc = Display.Display.CreateDC(null, Display.Display.Primary, null, IntPtr.Zero);
                _currentRamps = new Ramp();
                _originalRamps = new Ramp();
                GetDeviceGammaRamp(hdc, ref _originalRamps);
            }
            finally
            {
                if (!IntPtr.Zero.Equals(hdc))
                    Display.Display.DeleteDC(hdc);
            }
        }

        public async void ChangeColorRamp(double brightness = 0.5, double contrast = 0.5, double gamma = 1.0,
            bool reset = true)
        {
            var hdc = IntPtr.Zero;
            try
            {
                hdc = Display.Display.CreateDC(null, Display.Display.Primary, null, IntPtr.Zero);

                try
                {
                    if (_canceller != null)
                    {
                        _canceller.Cancel();
                        _canceller.Dispose();
                    }
                }
                catch (ObjectDisposedException)
                {
                }

                if (reset)
                {
                    SetDeviceGammaRamp(hdc, ref _originalRamps);
                }
                else
                {
                    var iArrayValue = CalculateLut(brightness, contrast, gamma);
                    _currentRamps.Red = _currentRamps.Blue = _currentRamps.Green = iArrayValue;

                    _canceller = new CancellationTokenSource();
                    CancellationToken token;
                    try
                    {
                        token = _canceller.Token;
                    }
                    catch (ObjectDisposedException)
                    {
                    }

                    await Task.Run(() =>
                    {
                        try
                        {
                            do
                            {
                                SetDeviceGammaRamp(hdc, ref _currentRamps);
                                Thread.Sleep(250);
                                if (token.IsCancellationRequested)
                                    break;
                            } while (true);
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                    }, token);
                }
            }
            finally
            {
                if (!IntPtr.Zero.Equals(hdc))
                    Display.Display.DeleteDC(hdc);
            }
        }

        /*
         * Code from
         * https://github.com/falahati/NvAPIWrapper/issues/20#issuecomment-634551206
         */
        private static ushort[] CalculateLut(double brightness = 0.5, double contrast = 0.5, double gamma = 2.8)
        {
            const int dataPoints = 256;

            // Limit gamma in range [0.4-2.8]
            gamma = Math.Min(Math.Max(gamma, 0.4), 2.8);
            // Normalize contrast in range [-1,1]
            contrast = (Math.Min(Math.Max(contrast, 0), 1) - 0.5) * 2;
            // Normalize brightness in range [-1,1]
            brightness = (Math.Min(Math.Max(brightness, 0), 1) - 0.5) * 2;
            // Calculate curve offset resulted from contrast
            var offset = contrast > 0 ? contrast * -25.4 : contrast * -32;
            // Calculate the total range of curve
            var range = dataPoints - 1 + offset * 2;
            // Add brightness to the curve offset
            offset += brightness * (range / 5);
            // Fill the gamma curve
            var result = new ushort[dataPoints];
            for (var i = 0; i < result.Length; i++)
            {
                var factor = (i + offset) / range;
                factor = Math.Pow(factor, 1 / gamma);
                factor = Math.Min(Math.Max(factor, 0), 1);
                result[i] = (ushort)Math.Round(factor * ushort.MaxValue);
            }

            return result;
        }

        public void ResetDvl()
        {
            try
            {
                _gpu.ResetSaturation();
                Console.WriteLine(@"[INFO][ColorController] Reset to : {0}", _gpu.InitSaturation);
            }
            catch (NotImplementedException)
            {
            }
        }

        internal void Close()
        {
            ResetDvl();
            ChangeColorRamp(reset: true);

            try
            {
                if (_canceller == null) return;
                _canceller.Cancel();
                _canceller.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        #region Singleton Pattern implement

        private static readonly Lazy<ColorController> instance =
            new Lazy<ColorController>(() => new ColorController());

        public static ColorController Instance => instance.Value;

        #endregion

        #region Win32 API Calls

        [DllImport("gdi32")]
        private static extern bool GetDeviceGammaRamp(IntPtr hDc, ref Ramp lpRamp);

        [DllImport("gdi32")]
        private static extern bool SetDeviceGammaRamp(IntPtr hDc, ref Ramp lpRamp);

        #endregion
    }
}