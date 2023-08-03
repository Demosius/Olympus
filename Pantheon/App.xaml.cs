using Styx;
using System;
using System.IO;
using Morpheus.ViewModels.Controls;
using Pantheon.Properties;
using Uranus;

namespace Pantheon;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static Charon Charon { get; set; } = new(Settings.Default.SolLocation);
    public static Helios Helios { get; set; } = new(Settings.Default.SolLocation);
    public static ProgressBarVM ProgressBar { get; set; } = new();

    public static string BaseDirectory()
    {
        return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
    }
}