using System;
using System.IO;
using System.Text.Json;

namespace Aion.ViewModel.Utility
{
    public class Options
    {
        private static readonly string JSONFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.json");

        public string DBLocation { get; set; }

        public Options()
        {
            var data = File.ReadAllText(JSONFile);
            var o = JsonSerializer.Deserialize<Opt>(data);

            // Set properties.
            if (o != null) DBLocation = o.DBLocation;
        }

        public static string GetDBLocation()
        {
            var data = File.ReadAllText(JSONFile);
            return JsonSerializer.Deserialize<Options>(data)?.DBLocation;
        }

        public void LoadOptions()
        {
            SetFromOther(new());
        }

        public void SaveOptions()
        {
            var data = JsonSerializer.Serialize(this, new() { WriteIndented = true });
            File.WriteAllText(JSONFile, data);
        }

        public void SetFromOther(Options options)
        {
            DBLocation = options.DBLocation;
        }
    }

    /// <summary>
    /// Intermediate option class for converting to and from JSON
    /// </summary>
    public class Opt
    {
        public string DBLocation { get; set; }
    }
}
