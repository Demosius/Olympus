using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;


namespace Uranus;

public enum EPullType
{
    ObjectOnly,
    IncludeChildren,
    FullRecursive
}

public enum EPushType
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
        if (Database == null)
        {
            if (!Directory.Exists(BaseDataDirectory)) _ = Directory.CreateDirectory(BaseDataDirectory);
            var s = Path.Combine(BaseDataDirectory, DatabaseName);
            Database = new SQLiteConnection(s);
            if (Database == null)
                throw new FailedConnectionException($"Failed to connect to {DatabaseName}, might be an invalid path.");
        }

        _ = Database.CreateTables(CreateFlags.None, Tables);
    }

    public virtual void ResetConnection(string solLocation)
    {
        BaseDataDirectory = solLocation;
        Database = null;
        InitializeDatabaseConnection();
    }

    /***************************** CREATE Data *****************************/
    /// <summary>
    /// Most basic building block for updating db tables.
    /// Removes all previous data and fully replaces it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objList"></param>
    /// <returns>Difference between objects deleted and objects inserted.</returns>
    public int ReplaceFullTable<T>(List<T> objList)
    {
        var line = 0;
        Database.RunInTransaction(() =>
        {
            line -= Database.DeleteAll<T>();
            line += Database.InsertAll(objList);
        });
        return line;
    }

    // Insert data into a table. Assumes that there will be no issues with duplicate data.
    public int InsertIntoTable<T>(IEnumerable<T> objList)
    {
        var enumerable = objList as T[] ?? objList.ToArray();
        if (!enumerable.Any()) return 0;
        var lines = 0;
        Database.RunInTransaction(() =>
        {
            lines = Database.InsertAll(enumerable);
        });
        return lines;
    }

    public bool Create<T>(T item, EPushType pushType = EPushType.ObjectOnly)
    {
        if (pushType == EPushType.ObjectOnly)
            _ = Database.Insert(item);
        else
        {
            var recursive = pushType == EPushType.FullRecursive;
            Database.InsertWithChildren(item, recursive);
        }

        return true;
    }

    /**************************** READ Data ****************************/

    public List<T> PullObjectList<T>(Expression<Func<T, bool>> filter = null, EPullType pullType = EPullType.ObjectOnly) where T : new()
    {
        if (pullType == EPullType.ObjectOnly)
            return filter is null ? Database.Table<T>().ToList() : Database.Table<T>().Where(filter).ToList();
        var recursive = pullType == EPullType.FullRecursive;
        return Database.GetAllWithChildren(filter, recursive);
    }

    public T PullObject<T>(object primaryKey, EPullType pullType = EPullType.ObjectOnly) where T : new()
    {
        if (pullType == EPullType.ObjectOnly)
            return Database.Find<T>(primaryKey);
        var recursive = pullType == EPullType.FullRecursive;
        return Database.GetWithChildren<T>(primaryKey, recursive);
    }

    protected List<string> GetTableNames()
    {
        var tableMappings = Database.TableMappings.ToList();

        return tableMappings.Select(map => map.TableName).ToList();
    }

    public string GetTableName(Type type)
    {
        return Database.GetMapping(type).TableName;
    }

    /**************************** UPDATE Data ****************************/

    /// <summary>
    /// Update an existing table - replacing duplicate data (and adding new data).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objList"></param>
    /// <returns></returns>
    public bool UpdateTable<T>(List<T> objList)
    {
        if (objList.Count == 0) return false;
        Database.RunInTransaction(() =>
        {
            foreach (var obj in objList)
            {
                _ = Database.InsertOrReplace(obj);
            }
        });
        return true;
    }

    public int Update<T>(T item) => Database.Update(item);

    public int InsertOrUpdate<T>(T item) => Database.InsertOrReplace(item);

    /**************************** DELETE Data ****************************/
    public bool EmptyTable<T>()
    {
        var delCount = 0;
        Database.RunInTransaction(() =>
        {
            delCount = Database.DeleteAll<T>();
        });
        return delCount > 0;
    }

    public bool Delete(object obj)
    {
        var rowsDeleted = Database.Delete(obj);
        return rowsDeleted > 0;
    }

    public bool DeleteByKey<T>(object key)
    {
        var rowsDeleted = Database.Delete<T>(key);
        return rowsDeleted > 0;
    }

    // Basic database management on a higher level. 
    // Each simply returns true if the action was successful, often based
    // on whether the database is valid.
    public virtual bool DeleteDatabase()
    {
        if (!File.Exists(DatabaseName)) return true;
        File.Delete(DatabaseName);
        return true;
    }

    /// <summary>
    ///  Empties all tables of data.
    /// </summary>
    /// <returns></returns>
    public virtual bool EmptyDatabase()
    {
        var mappings = Database.TableMappings.ToList();
        Database.RunInTransaction(() =>
        {
            foreach (var map in mappings)
            {
                _ = Database.DeleteAll(map);
            }
        });
        return true;
    }

    /// <summary>
    ///  Goes through validation checks, but aims to repair the database if it fails.
    ///  Most avenues lead to rebuilding from scratch.
    /// </summary>
    /// <returns></returns>
    public virtual bool RepairDatabase()
    {
        _ = CreateTables();
        return true;
    }

    /// <summary>
    ///  Create table for certain type.
    /// </summary>
    /// <returns>True if successful.</returns>
    public virtual bool CreateTable<T>()
    {
        var res = Database.CreateTable<T>();
        return res == CreateTableResult.Created;
    }

    /// <summary>
    ///  Create all required tables based.
    /// </summary>
    /// <returns></returns>
    public virtual bool CreateTables()
    {
        var results = Database.CreateTables(CreateFlags.None, Tables);

        // Check all results, for each table. Will return false if ANY of them are false.

        return results.Results.Values.Aggregate(true, (current, res) => current && res == CreateTableResult.Created);
    }

    // Placeholder to be overriden with new.
    public abstract Type[] Tables { get; }
}