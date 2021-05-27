﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Olympus
{
    public class FileDoesNotExistException : Exception
    {
        public FileDoesNotExistException() : base() { }
        public FileDoesNotExistException(string message) : base(message) { }
    }

    public class WrongFileExtensionException : Exception
    {
        public WrongFileExtensionException() : base() { }
        public WrongFileExtensionException(string message) : base(message) { }
    }

    public static class Toolbox
    {

        public static Settings GetSettings()
        {
            string fileName = "./settings.json";
            string data = File.ReadAllText(fileName);
            Settings settings = JsonSerializer.Deserialize<Settings>(data);
            return settings;
        }

        public static void SetSettings(Settings settings)
        {
            string data = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("./settings.json", data);
        }

        public static string GetSol()
        {
            Settings settings = GetSettings();

            return Path.GetFullPath(settings.SolLocation);
        }

        public static void SetSol(string newSolLocation)
        {
            // Check validity of the path string.
            try
            {
                newSolLocation = Path.GetFullPath(newSolLocation);
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("Invalid file path.","Error");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception: \n" + ex);
                throw;
            }
            // Make sure the final filder is Sol, and create the directory if it doesn't exist.
            if (Path.GetFileName(newSolLocation) != "Sol") newSolLocation += "/Sol";
            if (!Directory.Exists(newSolLocation)) Directory.CreateDirectory(newSolLocation);
            Settings settings = GetSettings();
            settings.SolLocation = newSolLocation;
            SetSettings(settings);
        }

        public static string GetItemCSV()
        {
            Settings settings = GetSettings();
            return Path.GetFullPath(settings.ItemCSVLocation);
        }

        public static void SetItemCSV(string newItemCSVLocation)
        {
            // Check validity of the path string: -Valid Path -Exists -Is CSV
            try
            {
                newItemCSVLocation = Path.GetFullPath(newItemCSVLocation);
                if (!File.Exists(newItemCSVLocation)) throw new FileDoesNotExistException();
                if (Path.GetExtension(newItemCSVLocation) != ".csv") throw new WrongFileExtensionException();
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("Invalid file path.", "Error");
                return;
            }
            catch (FileDoesNotExistException)
            {
                MessageBox.Show("File does not exist.\nMust be an existing CSV File.", "Error");
                return;
            }
            catch (WrongFileExtensionException)
            {
                MessageBox.Show("Wrong file extension.\nMust be a valid CSV File.", "Error");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception: \n" + ex);
                throw;
            }
            Settings settings = GetSettings();
            settings.ItemCSVLocation = newItemCSVLocation;
            SetSettings(settings);
        }
    }

    public class Settings
    {
        public string SolLocation { get; set; }
        public string ItemCSVLocation { get; set; }

        public Settings() { }
    }
}
