﻿using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;


namespace Uranus
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

        protected virtual void InitializeDatabaseConnection()
        {
            try
            {
                if (Database == null)
                {
                    if (!Directory.Exists(BaseDataDirectory)) _ = Directory.CreateDirectory(BaseDataDirectory);
                    string s = Path.Combine(BaseDataDirectory, DatabaseName);
                    Database = new SQLiteConnection(s);
                    if (Database == null)
                        throw new FailedConnectionException($"Failed to connect to {DatabaseName}, might be an invalid path.");
                }

                _ = Database.CreateTables(CreateFlags.None, Tables);

            }
            catch (FailedConnectionException) { throw; }
            catch (Exception) { throw; }
        }

        public virtual void ResetConnection(string solLocation)
        {
            BaseDataDirectory = solLocation;
            Database = null;
            InitializeDatabaseConnection();
        }

        /***************************** CREATE Data *****************************/

        // Most basic building block for updating db tables.
        // Removes all previous data and fully replaces it.
        public bool ReplaceFullTable<T>(List<T> objList)
        {
            try
            {
                Database.RunInTransaction(() =>
                {
                    _ = Database.DeleteAll<T>();
                    _ = Database.InsertAll(objList);
                });
                return true;
            }
            catch (SQLiteException)
            {
                throw;
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Insert data into a table. Assumes that there will be no issues with duplicate data.
        public bool InsertIntoTable<T>(List<T> objList)
        {
            if (objList.Count == 0) return false;
            try
            {
                Database.RunInTransaction(() =>
                {
                    _ = Database.InsertAll(objList);
                });
                return true;
            }
            catch (SQLiteException) { throw; }
            catch (InvalidDataException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Create<T>(T item, PushType pushType = PushType.ObjectOnly)
        {
            try
            {
                if (pushType == PushType.ObjectOnly)
                    _ = Database.Insert(item);
                else
                {
                    bool recursive = pushType == PushType.FullRecursive;
                    Database.InsertWithChildren(item, recursive);
                }

                return true;
            }
            catch (Exception) { throw; }
        }

        /**************************** READ Data ****************************/

        public List<T> PullObjectList<T>(Expression<Func<T, bool>> filter = null, PullType pullType = PullType.ObjectOnly) where T : new()
        {
            try
            {
                if (pullType == PullType.ObjectOnly)
                    return filter is null ? Database.Table<T>().ToList() : Database.Table<T>().Where(filter).ToList();
                bool recursive = pullType == PullType.FullRecursive;
                return Database.GetAllWithChildren<T>(filter, recursive);
            }
            catch (Exception) { throw; }
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
            catch (Exception) { throw; }
        }

        protected List<string> GetTableNames()
        {
            List<string> list = new() { };
            try
            {
                List<TableMapping> tableMappings = Database.TableMappings.ToList();

                foreach (TableMapping map in tableMappings)
                {
                    list.Add(map.TableName);
                }
            }
            catch (Exception) { throw; }
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
                        _ = Database.InsertOrReplace(obj);
                    }
                });
                return true;
            }
            catch (SQLiteException) { throw; }
            catch (InvalidDataException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Update<T>(T item)
        {
            try
            {
                _ = Database.Update(item);
                return true;
            }
            catch (Exception) { throw; }
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
            catch (Exception) { throw; }
        }

        public bool Delete(object obj)
        {
            try
            {
                int rowsDeleted = Database.Delete(obj);
                return rowsDeleted > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteByKey<T>(object key)
        {
            try
            {
                int rowsDeleted = Database.Delete<T>(key);
                return rowsDeleted > 0;
            }
            catch (Exception)
            {
                throw;
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
                throw;
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
                        _ = Database.DeleteAll(map);
                    }
                });
                return true;
            }
            catch (Exception)
            {
                throw;
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
                _ = CreateTables();
                return true;
            }
            catch (FailedConnectionException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }

        // Placeholder to be overriden with new.
        public abstract Type[] Tables { get; }
    }
}
