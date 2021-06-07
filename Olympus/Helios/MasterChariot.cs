using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Windows;
using System.Collections;

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
                Conn = new SQLiteConnection($@"URI=file:{FilePath}", true);
                if (Conn == null) 
                    throw new FailedConnectionException($"Failed to connect to {FilePath}, might be an invalid path.");
            }
            catch (FailedConnectionException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
        }

        /***************************** Upload Data *****************************/

        // Most basic building block for updating db tables.
        // Removes all previous data and fully replaces it.
        protected bool ReplaceFullTable(DataTable data, Dictionary<string, string> columns, string tableName)
        {
            try
            {
                // Check for any missing columns.
                List<string> missingCols = Utility.ValidateTableData(data, columns);
                if (missingCols.Count > 0) throw new InvalidDataException("Invalid Bin Data.", missingCols);

                Conn.Open();
                // Remove old data.
                SQLiteCommand delCommand = new SQLiteCommand(Conn)
                {
                    CommandText = $"DELETE FROM {tableName};"
                };
                delCommand.ExecuteNonQuery();

                // Build Insert transaction.
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
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Likely a fault with the data being input:\n\n{ex}\n\nCommon issues include newlines in coppied text that shouldn't be there. Check the data, and try again.","Database Error:");
                return false;
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show($"Missing Columns:\n\n{string.Join("|", ex.MissingColumns)}");
                return false;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        // User Replace Into to put a table into an existing table and replacing duplicate data.
        protected bool OverlayTable(DataTable data, Dictionary<string, string> columns, string tableName)
        {
            try
            {
                // Check for any missing columns.
                List<string> missingCols = Utility.ValidateTableData(data, columns);
                if (missingCols.Count > 0) throw new InvalidDataException("Invalid Bin Data.", missingCols);

                Conn.Open();

                // Build Insert transaction.
                using (var transaction = Conn.BeginTransaction())
                {
                    SQLiteCommand command = Conn.CreateCommand();
                    command.CommandText =
                    $@"
                        REPLACE INTO {tableName}
                        ({string.Join(", ", columns.Keys)}) 
                        VALUES (${string.Join(", $", columns.Keys)})
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
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Likely a fault with the data being input:\n\n{ex}\n\nCommon issues include newlines in coppied text that shouldn't be there. Check the data, and try again.", "Database Error:");
                return false;
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show($"Missing Columns:\n\n{string.Join("|", ex.MissingColumns)}");
                return false;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        // Insert data into a table. Assumes that there will be no issues with duplicate data.
        protected bool InsertIntoTable(DataTable data, Dictionary<string, string> columns, string tableName)
        {
            try
            {
                // Check for any missing columns.
                List<string> missingCols = Utility.ValidateTableData(data, columns);
                if (missingCols.Count > 0) throw new InvalidDataException("Invalid Bin Data.", missingCols);

                Conn.Open();

                // Build Insert transaction.
                using (var transaction = Conn.BeginTransaction())
                {
                    SQLiteCommand command = Conn.CreateCommand();
                    command.CommandText =
                    $@"
                        INSERT INTO {tableName}
                        ({string.Join(", ", columns.Keys)}) 
                        VALUES (${string.Join(", $", columns.Keys)})
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
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Likely a fault with the data being input:\n\n{ex}\n\nCommon issues include newlines in coppied text that shouldn't be there. Check the data, and try again.", "Database Error:");
                return false;
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show($"Missing Columns:\n\n{string.Join("|", ex.MissingColumns)}");
                return false;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
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
                    CommandText = $"SELECT * FROM [{tableName}];"
                };
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                data.TableName = tableName;
                Conn.Close();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return data;
        }

        public DataTable PullTableWithQuery(string query)
        {
            DataTable data = new DataTable();
            try
            {
                Conn.Open();
                SQLiteCommand command = new SQLiteCommand(Conn) { CommandText = query };
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                Conn.Close();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return data;
        }

        public DataSet PullFullDataSet()
        {
            DataSet set = new DataSet();
            try
            {
                foreach (string tableName in GetTables())
                {
                    set.Tables.Add(PullFullTable(tableName));
                }
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return set;
        }
        protected List<string> GetTables()
        {
            List<string> list = new List<string> { };
            // executes query that select names of all tables in master table of the database
            String query = "SELECT name FROM sqlite_master " +
                    "WHERE type = 'table'" +
                    "ORDER BY 1;";
            try
            {
                DataTable table = PullTableWithQuery(query);

                // Return all table names in the ArrayList

                foreach (DataRow row in table.Rows)
                {
                    list.Add(row.ItemArray[0].ToString());
                }
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return list;
        }

        // Basic database management on a higher level. 
        // Each simply retruns true if the action was successful, often based
        // on whether the database is valid.
        public virtual bool DeleteDatabase()
        {
            if (!File.Exists(FilePath)) return true;
            try
            {
                File.Delete(FilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool BuildDatabase()
        {
            if (File.Exists(FilePath)) return false;
            try
            {
                // Create File and reconnect.
                SQLiteConnection.CreateFile(FilePath);
                Connect();
                // Build Tables
                CreateTables();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        ///  Empties all tables of data.
        /// </summary>
        /// <returns></returns>
        public virtual bool EmptyDatabase()
        {
            try
            {
                Conn.Open();
                using (var transaction = Conn.BeginTransaction())
                {
                    foreach (string table in TableDefinitions.Keys)
                    {
                        string sql = $"DELETE FROM {table};";
                        SQLiteCommand command = new SQLiteCommand(sql, Conn);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Goes through validation checks, but aims to repair the database if it failes.
        ///  Most avenues lead to rebuilding from scratch.
        /// </summary>
        /// <returns></returns>
        public virtual bool RepairDatabase()
        {
            try
            {
                // If file does not exist, build from scratch.
                if (!File.Exists(FilePath))
                {
                    if (!BuildDatabase())
                        return false;
                }
                // If connection cannot be made, try to delete and then rebuild from scratch.
                Conn = new SQLiteConnection($@"URI=file:{FilePath}");
                if (Conn == null)
                {
                    DeleteDatabase();
                    BuildDatabase();
                }
                Conn.Open();
                // If Opening failes, rebuild from scratch.
                if (Conn.State == ConnectionState.Closed)
                {
                    DeleteDatabase();
                    BuildDatabase();
                }

                // Check for each table, and create it if it is missing.
                // Check tables exist.
                DataTable tables = Conn.GetSchema("Tables");
                string tblName;
                // Create a dictionary for checking tables.
                Dictionary<string, bool> tableCheck = new Dictionary<string, bool> { };
                foreach (string name in TableDefinitions.Keys)
                    tableCheck.Add(name, false);

                // Check through table schema to make sure all necessary tables exist.
                // (Extra tables are not of concern.)
                foreach (DataRow row in tables.Rows)
                {
                    tblName = row["TABLE_NAME"].ToString();
                    if (tableCheck.ContainsKey(tblName))
                    {
                        tableCheck[tblName] = true;
                    }
                }

                foreach (KeyValuePair<string, bool> pair in tableCheck)
                {
                    if (!pair.Value)
                    {
                        CreateTable(pair.Key);
                    }
                }

                Conn.Close();
                return true;
            }
            catch (FailedConnectionException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Checks if the database is valid.
        ///  Checks to the level of table existance - not columns at this point.
        /// </summary>
        /// <returns></returns>
        public virtual bool ValidateDatabase()
        {
            try
            {
                // Check file exists.
                if (!File.Exists(FilePath)) return false;

                // Check connection works.
                Connect();

                // Check Connection can be opened.
                Conn.Open();

                // Check tables exist.
                DataTable tables = Conn.GetSchema("Tables");
                string tblName;
                // Create a dictionary for checking tables.
                Dictionary<string, bool> tableCheck = new Dictionary<string, bool> { };
                foreach (string name in TableDefinitions.Keys)
                    tableCheck.Add(name, false);

                // Check through table schema to see if all necessary tables exist.
                // (Extra tables are not of concern.)
                foreach (DataRow row in tables.Rows)
                {
                    tblName = row["TABLE_NAME"].ToString();
                    if (tableCheck.ContainsKey(tblName))
                    {
                        tableCheck[tblName] = true;
                    }
                }

                foreach (bool pass in tableCheck.Values)
                {
                    if (!pass)
                    {
                        return false;
                    }
                }

                // Close the connection.
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Create table based on definition derived by string.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual bool CreateTable(string tableName)
        {
            tableName = tableName.ToLower();
            if (!TableDefinitions.ContainsKey(tableName))
            {
                MessageBox.Show($"{tableName} is not a valid table name.");
                return false;
            }
            try
            {
                Conn.Open();
                SQLiteCommand command = new SQLiteCommand(TableDefinitions[tableName], Conn);
                command.ExecuteNonQuery();
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Create all required tables based on table definitions.
        /// </summary>
        /// <returns></returns>
        public virtual bool CreateTables()
        {
            try
            {
                Conn.Open();
                using (var transaction = Conn.BeginTransaction())
                {
                    foreach (string sql in TableDefinitions.Values)
                    {
                        SQLiteCommand command = new SQLiteCommand(sql, Conn);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        // Placeholder to be overriden with new.
        public abstract Dictionary<string, string> TableDefinitions { get; }
    }
}
