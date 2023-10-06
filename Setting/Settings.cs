using System.IO;
using Newtonsoft.Json;

namespace valorant_settings.Setting
{
    internal class Settings<T> where T : new()
    {
        private const string DefaultFilename = "settings.json";

        public void Save(string fileName = DefaultFilename)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static T Load(string fileName = DefaultFilename)
        {
            var t = new T();
            if (File.Exists(fileName))
                t = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
            return t;
        }
    }
}