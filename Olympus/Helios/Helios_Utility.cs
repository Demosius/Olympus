using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Olympus.Helios
{   

    /// <summary>
    ///  Exception for when a table of data is missing appropriate columns.
    /// </summary>
    public class InvalidDataException : Exception
    {
        public List<string> MissingColumns { get; }

        public InvalidDataException(string message, List<string> missingColumns): base(message)
        {
            MissingColumns = missingColumns;
        }
    }

    /// <summary>
    ///  Exception for when a database connection has failed.
    /// </summary>
    public class FailedConnectionException : Exception
    {
        public FailedConnectionException(string message) : base(message) { }
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
            List<string> missing = new List<string> { };

            foreach (KeyValuePair<string, string> column in columns)
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
            List<string> missing = new List<string> { };
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

            return false;
        }

    }
}
