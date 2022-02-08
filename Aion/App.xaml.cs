using System;
using System.IO;
using Aion.Properties;
using Styx;
using Uranus;

namespace Aion
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static Charon Charon { get; set; } = new(Settings.Default.SolLocation);
        public static Helios Helios { get; set; } = new(Settings.Default.SolLocation);

        public static string BaseDirectory()
        {
            return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
