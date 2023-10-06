using System;
using System.Management;

namespace valorant_settings.GPU
{
    internal abstract class GpuDevice
    {
        private static readonly Lazy<IGpu> instance =
            new Lazy<IGpu>(() =>
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
                {
                    foreach (var o in searcher.Get())
                    {
                        var obj = (ManagementObject)o;
                        var deviceName = obj["Name"].ToString();

                        if (deviceName.Contains("NVIDIA"))
                            return new Nvidia(GpuVendor.Nvidia);
                        if (deviceName.Contains("AMD") || deviceName.Contains("Radeon"))
                            return new Amd(GpuVendor.Amd);
                        throw new NotImplementedException();
                    }

                    throw new NotImplementedException();
                }
            });

        public static IGpu Instance => instance.Value;
    }
}