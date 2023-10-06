using System.Collections.Generic;

namespace valorant_settings.Setting
{
    internal class AppSetting : Settings<AppSetting>
    {
        public readonly HashSet<string> PTargets = new HashSet<string>
        {
            "VALORANT-Win64-Shipping"
        };

        public double Brightness = 0.5;
        public double Contrast = 0.5;

        public string Display = @"\\.\DISPLAY1";
        public double Gamma = 1.0;
        public bool MinimizeOnStart = false;

        public int Saturation = 0;
    }
}