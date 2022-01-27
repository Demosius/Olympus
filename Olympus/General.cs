﻿using Olympus.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
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

    public static class General
    {
        public static void ShowUnexpectedException(Exception ex)
        {
            _ = MessageBox.Show($"Unexpected Exception:\n\n{ex}\n\nNotify Olympus development when possible.");
        }

        // Gets raw string data from the clipboard.
        public static string ClipboardToString()
        {
            var rawData = "";
            Thread thread = new(delegate ()
            {
                rawData = Clipboard.GetText(TextDataFormat.Text);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return rawData;
        }
    }

    /// <summary>
    ///  General utility functions that are likely to be shared by multiple classes.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        ///  Takes a data table and a dictionary of required columns and validates the data.
        /// </summary>
        /// <param name="data">A data table.</param>
        /// <param name="columns">
        /// A dictionary of <string, string> value pairs.
        /// The key is the necessary column, and the value is a backup column name.
        /// </param>
        /// <returns>
        /// False if the data does not contain the correct columns, 
        /// and cannot be renamed appropriately. (Adjusts the data as necessary.
        /// </returns>
        public static List<string> ValidateTableData(DataTable data, Dictionary<string, string> columns)
        {
            List<string> missing = new();

            foreach (var column in columns)
            {
                if (!data.Columns.Contains(column.Key))
                {
                    if (data.Columns.Contains(column.Value))
                    {
                        data.Columns[column.Value].ColumnName = column.Key;
                    }
                    else
                    {
                        missing.Add(column.Value);
                    }

                }
            }
            return missing;
        }

        /// <summary>
        ///  Checks if the given list of columns are contained in the datatable.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static List<string> ValidateTableData(DataTable data, List<string> columns)
        {
            List<string> missing = new();
            foreach (var column in columns)
            {
                if (!data.Columns.Contains(column))
                {
                    missing.Add(column);
                }
            }
            return missing;
        }

        public static bool MoveDataBase(string newLocation)
        {
            // TODO: Expand on this to make sure everything that should happen during database re-assignement, does.
            try
            {
                Settings.Default.SolLocation = newLocation;
                return true;
            }
            catch (Exception ex)
            {
                General.ShowUnexpectedException(ex);
                return false;
            }
        }

    }
}
