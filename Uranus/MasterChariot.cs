using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
// ReSharper disable CommentTypo


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
    public SQLiteConnection? Database { get; set; }

    protected MasterChariot()
    {
        BaseDataDirectory = string.Empty;
    }

    protected MasterChariot(string baseDataDirectory)
    {
        BaseDataDirectory = baseDataDirectory;
    }

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

    /***************************** Common Overrides For Direct Database Access Data *****************************/

    /// <summary>
    ///     Executes action within a (possibly nested) transaction by wrapping it in a SAVEPOINT.
    ///     If an exception occurs the whole transaction is rolled back, not just the current
    ///     savepoint. The exception is rethrown.
    /// </summary>
    /// <param name="action">The System.Action to perform within a transaction. action can contain any number
    ///     of operations on the connection but should never call SQLite.SQLiteConnection.BeginTransaction
    ///     or SQLite.SQLiteConnection.Commit.</param>
    public void RunInTransaction(Action action) =>  Database?.RunInTransaction(action);
    
    /// <summary>
    ///     Creates a SQLiteCommand given the command text (SQL) with arguments. Place a
    ///     '?' in the command text for each of the arguments and then executes that command.
    ///     Use this method instead of Query when you don't expect rows back. Such cases
    ///     include INSERTs, UPDATEs, and DELETEs. You can set the Trace or TimeExecution
    ///     properties of the connection to profile execution. 
    /// </summary>
    /// <param name="query">The fully escaped SQL.</param>
    /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
    /// <returns>The number of rows modified in the database as a result of this execution.</returns>
    public int Execute(string query, params object?[] args) => Database?.Execute(query, args) ?? 0;
     
    /// <summary>
    /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a
    ///     '?' in the command text for each of the arguments and then executes that command.
    ///     Use this method when return primitive values. You can set the Trace or TimeExecution
    ///     properties of the connection to profile execution.
    /// </summary>
    /// <param name="query">The fully escaped SQL.</param>
    /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
    /// <returns>The number of rows modified in the database as a result of this execution.</returns>
    public T ExecuteScalar<T>(string query, params object[] args) => Database!.ExecuteScalar<T>(query, args);
     
    /// <summary>
    /// Inserts the given object (and updates its auto incremented primary key if it
    /// has one). The return value is the number of rows added to the table. If a UNIQUE
    /// constraint violation occurs with some pre-existing object, this function deletes
    /// the old object.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <returns>The number of rows modified.</returns>
    public int InsertOrReplace(object? obj) => Database?.InsertOrReplace(obj) ?? 0;
    /// <summary>
    /// Inserts the given object (and updates its auto incremented primary key if it
    /// has one). The return value is the number of rows added to the table. If a UNIQUE
    /// constraint violation occurs with some pre-existing object, this function deletes
    /// the old object.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <param name="objType">The type of object to insert.</param>
    /// <returns>The number of rows modified.</returns>
    public int InsertOrReplace(object obj, Type objType) => Database?.InsertOrReplace(obj, objType) ?? 0;

    /// <summary>
    /// Inserts the given object (and updates its auto incremented primary key if it
    /// has one). The return value is the number of rows added to the table.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <returns>The number of rows added to the table.</returns>
    public int Insert(object obj) => Database?.Insert(obj) ?? 0;
    /// <summary>
    /// Inserts the given object (and updates its auto incremented primary key if it
    /// has one). The return value is the number of rows added to the table.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <param name="objType">The type of object to insert.</param>
    /// <returns>The number of rows added to the table.</returns>
    public int Insert(object obj, Type objType) => Database?.Insert(obj, objType) ?? 0;
    /// <summary>
    /// Inserts the given object (and updates its auto incremented primary key if it
    /// has one). The return value is the number of rows added to the table.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <param name="extra">Literal SQL code that gets placed into the command. INSERT {extra} INTO ...</param>
    /// <returns>The number of rows added to the table.</returns>
    public int Insert(object obj, string extra) => Database?.Insert(obj, extra) ?? 0;
    /// <summary>
    /// Inserts the given object (and updates its auto incremented primary key if it
    /// has one). The return value is the number of rows added to the table.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <param name="extra">Literal SQL code that gets placed into the command. INSERT {extra} INTO ...</param>
    /// <param name="objType">The type of object to insert.</param>
    /// <returns>The number of rows added to the table.</returns>
    public int Insert(object obj, string extra, Type objType) => Database?.Insert(obj, extra, objType) ?? 0;

    /// <summary>
    /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a
    /// '?' in the command text for each of the arguments and then executes that command.
    /// It returns each row of the result using the mapping automatically generated for
    /// the given type.
    /// </summary>
    /// <param name="query">The fully escaped SQL.</param>
    /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
    /// <returns>An enumerable with one result for each row returned by the query.</returns>
    public List<T> Query<T>(string query, params object?[] args) where T: new() => Database?.Query<T>(query, args) ?? new List<T>();
     
    /// <summary>
    /// Deletes the given object from the database using its primary key.
    /// </summary>
    /// <param name="objToDelete">The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.</param>
    /// <returns>The number of rows deleted.</returns>
    public int Delete(object objToDelete) => Database?.Delete(objToDelete) ?? 0; 

    /***************************** CREATE Data *****************************/
    /// <summary>
    /// Most basic building block for updating db tables.
    /// Removes all previous data and fully replaces it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objList"></param>
    /// <returns>Difference between objects deleted and objects inserted.</returns>
    public int ReplaceFullTable<T>(IEnumerable<T> objList)
    {
        var line = 0;

        void Action()
        {
            line -= Database.DeleteAll<T>();
            line += Database.InsertAll(objList);
        }

        Database?.RunInTransaction(Action);

        return line;
    }

    public async Task<int> ReplaceFullTableAsync<T>(IEnumerable<T> objList)
    {
        var line = 0;

        void Action()
        {
            line -= Database.DeleteAll<T>();
            line += Database.InsertAll(objList);
        }

        await Task.Run(() => Database?.RunInTransaction(Action)).ConfigureAwait(false);

        return line;
    }

    // Insert data into a table. Assumes that there will be no issues with duplicate data.
    public int InsertIntoTable<T>(IEnumerable<T> objList)
    {
        var objects = objList as T[] ?? objList.ToArray();
        if (!objects.Any()) return 0;

        var lines = Database?.InsertAll(objects) ?? 0;

        return lines;
    }

    public async Task<int> InsertIntoTableAsync<T>(IEnumerable<T> objList)
    {
        var objects = objList as T[] ?? objList.ToArray();
        if (!objects.Any()) return 0;
        var lines = 0;

        void Action()
        {
            lines = Database.InsertAll(objects);
        }

        await Task.Run(() => Database?.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }

    public bool Create<T>(T item, EPushType pushType = EPushType.ObjectOnly)
    {
        if (pushType == EPushType.ObjectOnly)
        {
            if (Database != null) _ = Database.Insert(item);
        }
        else
        {
            var recursive = pushType == EPushType.FullRecursive;
            Database.InsertWithChildren(item, recursive);
        }

        return true;
    }

    /**************************** READ Data ****************************/

    public async Task<List<T>> PullObjectListAsync<T>(Expression<Func<T, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) where T : new()
    {
        if (pullType == EPullType.ObjectOnly)
            return await Task.Run(() =>
                (filter is null ? Database?.Table<T>().ToList() : Database?.Table<T>().Where(filter).ToList())??
                new List<T>()).ConfigureAwait(false);
        var recursive = pullType == EPullType.FullRecursive;
        return await Task.Run(() => Database.GetAllWithChildren(filter, recursive)).ConfigureAwait(false);
    }

    public List<T> PullObjectList<T>(Expression<Func<T, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) where T : new()
    {
        if (pullType == EPullType.ObjectOnly)
            return (filter is null ? Database?.Table<T>().ToList() : Database?.Table<T>().Where(filter).ToList()) ??
                new List<T>();
        var recursive = pullType == EPullType.FullRecursive;
        return Database.GetAllWithChildren(filter, recursive);
    }

    public T? PullObject<T>(object primaryKey, EPullType pullType = EPullType.ObjectOnly) where T : new()
    {
        if (pullType == EPullType.ObjectOnly)
        {
            return Database is null ? new T() : Database.Find<T>(primaryKey);
        }

        var recursive = pullType == EPullType.FullRecursive;

        try
        {
            return Database.GetWithChildren<T>(primaryKey, recursive);
        }
        catch (InvalidOperationException)
        {
            return default;
        }
    }

    public async Task<T?> PullObjectAsync<T>(object primaryKey, EPullType pullType = EPullType.ObjectOnly)
        where T : new()
        => await Task.Run(() => PullObject<T>(primaryKey, pullType));

    protected List<string> GetTableNames()
    {
        var tableMappings = Database?.TableMappings.ToList();

        return tableMappings is null ? new List<string>() : tableMappings.Select(map => map.TableName).ToList();
    }

    public string GetTableName(Type type)
    {
        return Database?.GetMapping(type).TableName ?? string.Empty;
    }

    /**************************** UPDATE Data ****************************/

    /// <summary>
    /// Update an existing table - replacing duplicate data (and adding new data).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objList"></param>
    /// <returns>The number of rows modified in the database by this transaction.</returns>
    public int UpdateTable<T>(IEnumerable<T> objList)
    {
        var lines = 0;

        void Action()
        {
            lines += objList.Select(InsertOrReplace).Sum();
        }

        Database?.RunInTransaction(Action);

        return lines;
    }

    public async Task<int> UpdateTableAsync<T>(IEnumerable<T> objList)
    {
        var lines = 0;

        void Action()
        {
            lines += objList.Select(InsertOrReplace).Sum();
        }

        await Task.Run(() => Database?.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    /// <summary>
    /// Updates the given type value in the appropriate database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns>The number of database rows affected.</returns>
    public int Update<T>(T item) => Database?.Update(item) ?? 0;

    public int InsertOrReplace<T>(T item) => Database?.InsertOrReplace(item) ?? 0;

    public async Task<int> InsertOrReplaceAsync<T>(T item) =>
        await Task.Run(() => Database?.InsertOrReplace(item) ?? 0).ConfigureAwait(false);

    /**************************** DELETE Data ****************************/
    public bool EmptyTable<T>()
    {
        var delCount = 0;
        Database?.RunInTransaction(() =>
        {
            delCount = Database.DeleteAll<T>();
        });
        return delCount > 0;
    }

    public bool DeleteByKey<T>(object key)
    {
        var rowsDeleted = Database?.Delete<T>(key);
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
        var mappings = Database?.TableMappings.ToList();
        if (mappings is null) return false;
        Database?.RunInTransaction(() =>
        {
            foreach (var map in mappings)
            {
                _ = Database.DeleteAll(map);
            }
        });
        return true;
    }

    /**************************** Async Overrides  ****************************/
    public async Task<int> ExecuteAsync(string query, params object[] args)
    {
        return await Task.Run(() => Database?.Execute(query, args) ?? 0).ConfigureAwait(false);
    }

    public async Task<int> UpdateAsync(object obj)
    {
        return await Task.Run(() => Database?.Update(obj) ?? 0).ConfigureAwait(false);
    }

    public async Task<int> UpdateAsync(object obj, Type objType)
    {
        return await Task.Run(() => Database?.Update(obj, objType) ?? 0).ConfigureAwait(false);
    }
    
    public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
    {
        return await Task.Run(() => Database?.Query<T>(query, args) ?? new List<T>()).ConfigureAwait(false);
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
        var res = Database?.CreateTable<T>();
        return res == CreateTableResult.Created;
    }

    /// <summary>
    ///  Create all required tables based.
    /// </summary>
    /// <returns></returns>
    public virtual bool CreateTables()
    {
        var results = Database?.CreateTables(CreateFlags.None, Tables);

        // Check all results, for each table. Will return false if ANY of them are false.

        return results is not null && results.Results.Values.Aggregate(true, (current, res) => current && res == CreateTableResult.Created);
    }

    // Placeholder to be overriden with new.
    public abstract Type[] Tables { get; }
}