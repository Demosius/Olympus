using System;
using System.IO;
using Deimos.Properties;
using Uranus;

namespace Deimos;

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