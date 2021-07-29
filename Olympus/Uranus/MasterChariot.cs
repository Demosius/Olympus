using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using System.Data;
using System.Windows;
using System.Collections;
using SQLiteNetExtensions.Extensions;

namespace Olympus.Uranus
{
    public enum PullType
    {
        ObjectOnly,
        IncludeChildren,
        FullRecursive
    }

    public enum PushType
    {
        ObjectOnly,
        IncludeChildren,
        FullRecursive
    }

    /// <summary>
    ///     Master chariot abstract class to be used as the base class
    ///     for all database chariots.
    ///     
    ///     Each chariot is to be designed as the interface, to carry 
    ///     data to and from, for a specific database.
    ///     
    ///     Master chariot class to define expected functionality.
    /// </summary>
    public abstract class MasterChariot
    {
        public string BaseDataDirectory { get; set; }
        public abstract string DatabaseName { get; }
        public SQLiteConnection Database { get; set; }

        protected void InitializeDatabaseConnection()
        {
            try
            {
                if (Database == null)
                {
                    if (!Directory.Exists(BaseDataDirectory)) Directory.CreateDirectory(BaseDataDirectory);
                    string s = Path.Combine(BaseDataDirectory, DatabaseName);
                    Database = new SQLiteConnection(s);
                    if (Database == null)
                        throw new FailedConnectionException($"Failed to connect to {DatabaseName}, might be an invalid path.");
                }

                Database.CreateTables(CreateFlags.None, Tables);

            }
            catch (FailedConnectionException ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
        }

        public abstract void ResetConnection();

        /***************************** CREATE Data *****************************/

        // Most basic building block for updating db tables.
        // Removes all previous data and fully replaces it.
        public bool ReplaceFullTable<T>(List<T> objList)
        {
            try
            {
                Database.RunInTransaction(() =>
                {
                    Database.DeleteAll<T>();
                    Database.InsertAll(objList);
                });
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

        // Insert data into a table. Assumes that there will be no issues with duplicate data.
        public bool InsertIntoTable<T>(List<T> objList)
        {
            try
            {
                Database.RunInTransaction(() =>
                {
                    Database.InsertAll(objList);
                });
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

        public bool Create<T>(T item, PushType pushType = PushType.ObjectOnly)
        {
            try
            {
                if (pushType == PushType.ObjectOnly)
                    Database.Insert(item);
                else
                {
                    bool recursive = pushType == PushType.FullRecursive;
                    Database.InsertWithChildren(item, recursive);
                }

                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /**************************** READ Data ****************************/

        public List<T> PullObjectList<T>(PullType pullType = PullType.ObjectOnly) where T : new()
        {
            try
            {
                if (pullType == PullType.ObjectOnly)
                    return Database.Table<T>().ToList();
                bool recursive = pullType == PullType.FullRecursive;
                return Database.GetAllWithChildren<T>(null, recursive);
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return new List<T> { };
            }
        }

        public T PullObject<T>(object primaryKey, PullType pullType = PullType.ObjectOnly) where T : new()
        {
            try
            {
                if (pullType == PullType.ObjectOnly)
                    return Database.Find<T>(primaryKey);
                bool recursive = pullType == PullType.FullRecursive;
                return Database.GetWithChildren<T>(primaryKey, recursive);
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return new T();
            }
        }

        protected List<string> GetTableNames()
        {
            List<string> list = new List<string> { };
            try
            {
                List<TableMapping> tableMappings = Database.TableMappings.ToList();

                foreach (TableMapping map in tableMappings)
                {
                    list.Add(map.TableName);
                }
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return list;
        }

        public string GetTableName(Type type)
        {
            return Database.GetMapping(type).TableName;
        }

        /**************************** UPDATE Data ****************************/

        // Update an existing table - replacing duplicate data (and adding new data).
        public bool UpdateTable<T>(List<T> objList)
        {
            if (objList.Count == 0) return false;
            try
            {
                Database.RunInTransaction(() =>
                {
                    foreach (var obj in objList)
                    {
                        Database.InsertOrReplace(obj);
                    }
                });
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

        public bool Update<T>(T item)
        {
            try
            {
                Database.Update(item);
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /**************************** DELETE Data ****************************/
        public bool EmptyTable<T>()
        {
            try
            {
                int delCount = 0;
                Database.RunInTransaction(() =>
                {
                    delCount = Database.DeleteAll<T>();
                });
                return delCount > 0;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        // Basic database management on a higher level. 
        // Each simply retruns true if the action was successful, often based
        // on whether the database is valid.
        public virtual bool DeleteDatabase()
        {
            if (!File.Exists(DatabaseName)) return true;
            try
            {
                File.Delete(DatabaseName);
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
                List<TableMapping> mappings = Database.TableMappings.ToList();
                Database.RunInTransaction(() =>
                {
                    foreach (TableMapping map in mappings)
                    {
                        Database.DeleteAll(map);
                    }
                });
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
                CreateTables();
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
        ///  Create table for certain type.
        /// </summary>
        /// <returns>True if successful.</returns>
        public virtual bool CreateTable<T>()
        {
            try
            {
                CreateTableResult res = Database.CreateTable<T>();
                return res == CreateTableResult.Created;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Create all required tables based.
        /// </summary>
        /// <returns></returns>
        public virtual bool CreateTables()
        {
            try
            {
                bool returnValue = true;

                CreateTablesResult results = Database.CreateTables(CreateFlags.None, Tables);

                // Check all results, for each table. Will return false if ANY of them are false.
                foreach (CreateTableResult res in results.Results.Values)
                {
                    returnValue = returnValue && res == CreateTableResult.Created;
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        // Placeholder to be overriden with new.
        public abstract Type[] Tables { get; }
    }
}
