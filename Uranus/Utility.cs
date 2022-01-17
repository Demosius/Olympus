using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Uranus
{   

    /// <summary>
    ///  Exception for when a table of data is missing appropriate columns.
    /// </summary>
    public class InvalidDataException : Exception
    {
        public List<string> MissingColumns { get; }

        public InvalidDataException(string message, List<string> missingColumns): base($"Missing Columns:\n\n{string.Join("|", missingColumns)}{(message != "" ? $"\n\n{message}" : message)}")
        {
            MissingColumns = missingColumns;
        }

        public InvalidDataException(List<string> missingColumns)
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

}
