using System.Windows.Forms;
using NvAPIWrapper;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;

namespace valorant_settings.GPU
{
    internal class Nvidia : IGpu
    {
        private int _currentSaturation;
        private DisplayHandle _displayHandle;

        public Nvidia(GpuVendor vendor)
        {
            try
            {
                NVIDIA.Initialize();
            }
            catch (NVIDIAApiException)
            {
                MessageBox.Show("NvAPI Intialize Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Vendor = vendor;
        }

        public GpuVendor Vendor { get; }

        public int MaxSaturation { get; private set; }

        public int MinSaturation { get; private set; }

        public int InitSaturation { get; private set; }

        public int Saturation
        {
            get => _currentSaturation;
            set
            {
                if (value > MaxSaturation)
                    value = MaxSaturation;
                if (value < MinSaturation)
                    value = MinSaturation;

                DisplayApi.SetDVCLevel(_displayHandle, value);
                _currentSaturation = value;
            }
        }

        public void ResetSaturation()
        {
            Saturation = InitSaturation;
        }

        public void Load(string display)
        {
            _displayHandle = DisplayApi.GetAssociatedNvidiaDisplayHandle(display);
            var dvcInfo = DisplayApi.GetDVCInfo(_displayHandle);
            MaxSaturation = dvcInfo.MaximumLevel;
            MinSaturation = dvcInfo.MinimumLevel;
            InitSaturation = _currentSaturation = dvcInfo.CurrentLevel;
        }

        public void Close()
        {
            try
            {
                NVIDIA.Unload();
            }
            catch (NVIDIAApiException)
            {
                MessageBox.Show("NvAPI Unload Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}