using System;
using System.Threading;
using System.Windows.Forms;
using valorant_settings.GPU;

namespace valorant_settings.App
{
    internal static class Program
    {
        private static MainForm _mForm;

        [STAThread]
        private static void Main()
        {
            IGpu gpu = null;
            try
            {
                gpu = GpuDevice.Instance;
                if (gpu.Vendor == GpuVendor.Amd)
                    /* AMD Saturation (equals to Digital Vibrance of Nvidia) is not supported yet. */
                    MessageBox.Show(
                        "AMD Device Detected - Saturation is not supported yet.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
            }
            catch (NotImplementedException)
            {
                MessageBox.Show(
                    "Intel/Nvidia Optimus/Etc Device Detected - Will be supported soon",
                    "Nvidia GPU is not found!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Thread.Sleep(1000);
                Application.Exit();
            }

            // Open Main Form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mForm = new MainForm();
            Application.Run(_mForm);

            // Unload NvAPI dll after Application.Exit()
            gpu?.Close();
        }
    }
}