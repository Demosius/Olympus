﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Globalization;
using Olympus.Styx.Model;
using System.IO;
using Olympus.Model;

namespace Olympus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        public static Charon Charon { get; set; } = new Charon();
        public static Settings Settings { get; set; } = new Settings();
        public static string BaseDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
    }
}
