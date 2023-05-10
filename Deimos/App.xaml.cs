using System;
using System.IO;
using Deimos.Properties;
using Morpheus.ViewModels.Controls;
using Uranus;

namespace Deimos;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static Helios Helios { get; set; } = new(Settings.Default.SolLocation);
    public static ProgressBarVM ProgressBar { get; set; } = new();

    public static string BaseDirectory()
    {
        return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
    }
}