using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Windows;

/// <summary>
///     Master chariot abstract class to be used as the base class
///     for all database chariots.
///     
///     Each chariot is to be designed as the interface, to carry 
///     data to and from, for a specific database.
///     
///     Master chariot class to define expected functionality.
/// </summary>

namespace Olympus.Helios
{
    public abstract class MasterChariot
    {
        public string FilePath { get; set; }
        public SQLiteConnection Conn { get; set; }

        protected void Connect()
        {
            try
            {
                Conn = new SQLiteConnection($@"URI=file:{FilePath}");
                if (Conn == null) 
                    throw new FailedConnectionException($"Failed to connect to {FilePath}, might be invalid path.");
            }
            catch (FailedConnectionException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected exception:\n\n{ex}");
            }
        }

        /***************************** Upload Data *****************************/

        // Most basic building block for updating db tables.
        protected bool ReplaceFullTable(DataTable data, Dictionary<string, string> columns, string tableName)
        {
            try
            {
                // Check for any missing columns.
                List<string> missingCols = Utility.ValidateTableData(data, columns);
                if (missingCols.Count > 0) throw new InvalidDataException("Invalid Bin Data.", missingCols);

                Conn.Open();
                SQLiteCommand delCommand = new SQLiteCommand(Conn)
                {
                    CommandText = $"DELETE FROM {tableName};"
                };
                delCommand.ExecuteNonQuery();

                using (var transaction = Conn.BeginTransaction())
                {
                    SQLiteCommand command = Conn.CreateCommand();
                    command.CommandText =
                    $@"
                        INSERT INTO {tableName}
                        ({string.Join(", ", columns.Keys)}) 
                        VALUES (${string.Join(", $",columns.Keys)})
                    ";

                    foreach (string col in columns.Keys)
                    {
                        command.Parameters.Add(new SQLiteParameter('$' + col));
                    }

                    foreach (DataRow row in data.Rows)
                    {
                        int i = 0;
                        string[] arrCols = columns.Keys.ToArray();
                        foreach (SQLiteParameter param in command.Parameters)
                        {
                            param.Value = row[arrCols[i]];
                            i++;
                        }
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();

                }

                Conn.Close();
                return true;
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show($"Missing Columns:\n\n{string.Join("|", ex.MissingColumns)}");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failure to update {tableName} data - Unexpected Exception:\n\n{ex}");
                return false;
            }
        }

        /**************************** Download Data ****************************/

        protected DataTable PullFullTable(string tableName)
        {
            DataTable data = new DataTable();
            try
            {
                Conn.Open();
                SQLiteCommand command = new SQLiteCommand(Conn)
                {
                    CommandText = $"SELECT * FROM {tableName};"
                };
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected exception:\n\n{ex}");
            }
            return data;
        }

        // Basic database management on a higher level. 
        // Each simply retruns true if the action was successful, often based
        // on whether the database is valid.
        public abstract bool RepairDatabase();

        public abstract bool ValidateDatabase();

        public abstract bool DeleteDatabase();

        public abstract bool BuildDatabase();

        public abstract bool EmptyDatabase();

        public abstract void CreateTable();

    }
}
