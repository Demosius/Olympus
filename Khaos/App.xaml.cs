using System;
using System.IO;
using Khaos.Properties;
using Uranus;

namespace Khaos;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static Helios Helios { get; set; } = new(Settings.Default.SolLocation);

    public static string BaseDirectory()
    {
        return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
    }
}