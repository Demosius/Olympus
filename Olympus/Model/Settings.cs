using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Olympus.Model
{
    public class Settings
    {
        private readonly string jsonFile;
        private JSONSettings jsonSettings;

        public string SolLocation
        {
            get { return jsonSettings.SolLocation; }
            set
            {
                jsonSettings.SolLocation = value;
                WriteToFile();
            }
        }

        public string ItemCSVLocation
        {
            get { return jsonSettings.ItemCSVLocation; }
            set
            {
                jsonSettings.ItemCSVLocation = value;
                WriteToFile();
            }
        }

        public Settings()
        {
            jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            ReadFromFile();
        }

        private void ReadFromFile()
        {
            string data = File.ReadAllText(jsonFile);
            jsonSettings = JsonSerializer.Deserialize<JSONSettings>(data);
        }

        private void WriteToFile()
        {
            string data = JsonSerializer.Serialize(jsonSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFile, data);
        }
    }

    public class JSONSettings
    {
        public string SolLocation { get; set; }
        public string ItemCSVLocation { get; set; }
    }
}
