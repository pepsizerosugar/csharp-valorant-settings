using System;

namespace valorant_settings.GPU
{
    internal class Amd : IGpu
    {
        private int _currentSaturation;

        public Amd(GpuVendor vendor)
        {
            Vendor = vendor;
        }

        public GpuVendor Vendor { get; }

        public int Saturation
        {
            get => _currentSaturation;
            set
            {
                if (value > MaxSaturation)
                    value = MaxSaturation;
                if (value < MinSaturation)
                    value = MinSaturation;

                _currentSaturation = value;
            }
        }

        public int MaxSaturation { get; private set; }

        public int MinSaturation { get; private set; }

        public int InitSaturation { get; private set; }

        public void ResetSaturation()
        {
            Saturation = InitSaturation;
        }

        public void Load(string display)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}