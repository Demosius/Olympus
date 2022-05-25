﻿using Hydra.Properties;
using Styx;
using System;
using System.IO;
using Uranus;

namespace Hydra;

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