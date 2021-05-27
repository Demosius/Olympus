using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Text.Json;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace Olympus.Helios
{
    public static class DataConversion
    {
        /// <summary>
        ///  Gets raw string data from the clipboard.
        /// </summary>
        /// <returns>Simple String</returns>
        public static string ClipboardToString()
        {
            string rawData="";
            Thread thread = new Thread(delegate ()
            {
                rawData = Clipboard.GetText(TextDataFormat.Text);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return rawData;
        }

        /// <summary>
        /// Reads data from the clipboard, assumes it is rectangular 2-dimensional data separated
        /// by tabs and newlines. Converts it into an array.
        /// </summary>
        /// <returns> 2 dimensional array. </returns>
        public static string[,] ClipboardToArray()
        {
            string rawData = ClipboardToString();

            string[] outerArray = rawData.Split(new [] { "\r\n" }, StringSplitOptions.None);
            int maxCol = outerArray[0].Split('\t').Length;
            int maxRow = outerArray.Length;
            string[,] fullArray = new string[maxRow, maxCol];
            string[] innerArray;

            for (int row=0; row < outerArray.Length; ++row)
            {
                innerArray = outerArray[row].Split('\t');
                for (int col=0; col < innerArray.Length && col < maxCol; ++col)
                {
                    fullArray[row,col] = innerArray[col];
                }
            }

            return fullArray;
        }

        /// <summary>
        /// Takes a table-like array of data and converts it to a JSON string.
        /// </summary>
        /// <param name="array">2 dimensional string array with headers in top row.</param>
        /// <returns>JSON string.</returns>
        public static string ArrayToJSON(string[,] array)
        {
            string returnString = "{\n\t";
            string line;
            int row, col, rowMax, colMax;
            rowMax = array.GetLength(0);
            colMax = array.GetLength(1);
            // Set headers.
            string[] headers = new string[colMax];
            for (col = 0; col<colMax; ++col)
            {
                headers[col] = array[0, col];
            }

            string head, val;

            // Set contents
            for (row=1; row<rowMax; ++row)
            {
                line = "{\n\t\t";

                for (col=0; col<colMax; ++col)
                {
                    head = headers[col] + ": ";
                    val = array[row, col];
                    line += head + ((col == colMax - 1) ? val + "\n\t" : val + ",\n\t\t");
                }

                line += (row == rowMax - 1) ? "}\n" : "},\n\t";
                
                returnString += line;
            }

            returnString += "}";

            return returnString;
        }

        /// <summary>
        ///  Takes data from the clipboard and converts it into a data table.
        ///  Assumes that data is separated by tabs and new lines.
        /// </summary>
        /// <returns>DataTable</returns>
        public static DataTable ClipboardToTable()
        {
            string rawData = ClipboardToString();
            byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
            MemoryStream stream = new MemoryStream(byteArray);

            DataTable data = new DataTable();

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headers = line.Split('\t');
                foreach (string header in headers)
                    data.Columns.Add(header);

                line = reader.ReadLine();

                // Add row data.
                while (line != null)
                {
                    data.Rows.Add(line.Split('\t'));
                    line = reader.ReadLine();
                }
            }

            return data;
        }

        /// <summary>
        ///  Pulls data from a .csv file into a DataTable
        /// </summary>
        /// <param name="csvPath">Valid path to .csv file.</param>
        /// <param name="columns">Leave empty to get all columns.</param>
        /// <param name="conditions">SQL style conditions.</param>
        /// <returns></returns>
        public static DataTable CSVToTable(string csvPath, List<string> columns, string conditions = "", bool includesHeaders = true)
        {
            string header = includesHeaders ? "Yes" : "No";

            DataTable data = new DataTable();
            // Check path validity.
            if (!(File.Exists(csvPath) && Path.GetExtension(csvPath) == ".csv")) return data;

            string csvDir = Path.GetDirectoryName(csvPath);
            string csvFile = Path.GetFileName(csvPath);
            string colStr = ((columns.Count > 0) ? string.Join(", ", columns) : "*");

            // Build query.
            string query = $"SELECT {colStr} FROM [{csvFile}] {((conditions == "") ? "" : $"WHERE {conditions}")};";

            // Connect and pull data.
            using (OleDbConnection connection = new OleDbConnection(
              @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + csvDir +
              ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(query, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                adapter.Fill(data);
                return data;
            }
        }
    }
}
