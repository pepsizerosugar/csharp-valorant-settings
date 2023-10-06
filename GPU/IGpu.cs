namespace valorant_settings.GPU
{
    internal interface IGpu
    {
        GpuVendor Vendor { get; }
        int Saturation { get; set; }
        int MaxSaturation { get; }
        int MinSaturation { get; }
        int InitSaturation { get; }
        void ResetSaturation();
        void Load(string display);
        void Close();
    }
}