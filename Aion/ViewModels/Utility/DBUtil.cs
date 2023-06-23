using Aion.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Uranus.Staff.Models;
using EClockStatus = Aion.Models.EClockStatus;

namespace Aion.ViewModels.Utility;

public enum ETransactionType
{
    ObjectOnly,
    IncludeChildren,
    FullRecursive
}

public class ObsoleteDBUtil
{
    public static string BaseDataDirectory { get; set; } = Options.GetDBLocation();
    public static string DatabaseName { get; set; } = @"aion.sqlite";

    public static void InitializeDatabase()
    {
        BaseDataDirectory = Options.GetDBLocation();
        InitializeDatabase(BaseDataDirectory);
    }

    public static void InitializeDatabase(string directory)
    {
        var tables = new[]
        {
            typeof(Employee), typeof(ClockTime), typeof(DailyEntry)
        };

        try
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var s = Path.Combine(directory, DatabaseName);
            SQLiteConnection connection = new(s);

            _ = connection.CreateTables(CreateFlags.None, tables);

        }
        catch
        {
            MessageBox.Show("Unexpected Error. DBUtil");
        }
    }

    /// <summary>
    /// Returns a fresh database connection based on the targeted connection string.
    /// </summary>
    /// <returns></returns>
    public static SQLiteConnection Conn()
    {
        return Conn(BaseDataDirectory);
    }

    public static SQLiteConnection Conn(string dbDirectory)
    {
        try
        {
            if (!Directory.Exists(dbDirectory)) Directory.CreateDirectory(dbDirectory);
            var s = Path.Combine(dbDirectory, DatabaseName);
            return new SQLiteConnection(s);

        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil");
            throw;
        }
    }

    public static List<T> PullObjectList<T>(ETransactionType transactionType = ETransactionType.ObjectOnly) where T : new()
    {
        try
        {
            var conn = Conn();
            if (transactionType == ETransactionType.ObjectOnly)
                return conn.Table<T>().ToList();
            else
            {
                var recursive = transactionType == ETransactionType.FullRecursive;
                return conn.GetAllWithChildren<T>(null, recursive);
            }
        }
        catch
        {
            _ = MessageBox.Show($"Unexpected Error. DBUtil - PullObjectList {typeof(T)}");
            return new List<T>();
        }
    }

    public static void UpdateClock(ClockTime clockTime)
    {
        try
        {
            var conn = Conn();
            conn.InsertOrReplace(clockTime);
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - UpdateClock");
        }
    }

    public static List<ClockTime> GetPendingClocks()
    {
        try
        {
            var conn = Conn();
            return conn.Query<ClockTime>("SELECT * FROM ClockTime WHERE Status = ?", EClockStatus.Pending).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetPendingClocks");
            return new List<ClockTime>();
        }
    }

    public static List<ClockTime> GetPendingClocks(int managerCode, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var conn = Conn();
            return conn.Query<ClockTime>(
                "SELECT ClockTime.* FROM ClockTime JOIN Employee ON ClockTime.EmployeeID = Employee.Code " +
                "WHERE Employee.ReportsToCode = ? " +
                "AND ClockTime.Status = ?" +
                "AND Timestamp BETWEEN ? AND ?;",
                managerCode, EClockStatus.Pending,
                fromDate.ToString("yyyy-MM-dd 00:00:00"), toDate.ToString("yyyy-MM-dd 23:59:59")).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetPendingClocks");
            return new List<ClockTime>();
        }
    }

    public static List<Employee> GetEmployeesFromList(List<int> employeeCodes)
    {
        try
        {
            var conn = Conn();
            return conn.Query<Employee>($"SELECT * FROM Employee WHERE Code in ({String.Join(", ", employeeCodes)});").ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmployeesFromList");
            return new List<Employee>();
        }
    }

    public static List<DailyEntry> GetEmployeeEntries(List<int> employeeCodes) //, DateTime startDate, DateTime endDate)
    {
        try
        {
            var conn = Conn();
            return conn.GetAllWithChildren<DailyEntry>(de => employeeCodes.Contains(de.EmployeeCode));
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmployeeEntriesByDate");
            return new List<DailyEntry>();
        }
    }

    public static Dictionary<int, Employee> GetEmployees(ETransactionType transactionType = ETransactionType.ObjectOnly)
    {
        try
        {
            var conn = Conn();
            if (transactionType == ETransactionType.ObjectOnly)
                return conn.Table<Employee>().ToDictionary(e => e.ID, e => e);
            else
            {
                var recursive = transactionType == ETransactionType.FullRecursive;
                return conn.GetAllWithChildren<Employee>(null, recursive).ToDictionary(e => e.ID, e => e);
            }
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmployees");
            return new Dictionary<int, Employee>();
        }
    }

    public static List<ClockTime> GetEmployeeClockTimes(int employeeCode)
    {
        try
        {
            return Conn().Query<ClockTime>("SELECT * FROM ClockTime WHERE EmployeeID = ?", employeeCode);
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmployeeClockTimes");
            return new List<ClockTime>();
        }
    }

    public static List<DailyEntry> GetEmployeeDailyEntries(int employeeCode)
    {
        try
        {
            return Conn().Query<DailyEntry>("SELECT * FROM ShiftEntry WHERE EmployeeID = ?", employeeCode);
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmployeeDailyEntries");
            return new List<DailyEntry>();
        }
    }

    /// <summary>
    /// Gets count of clocks for today.
    /// </summary>
    /// <param name="employeeCode"></param>
    /// <returns></returns>
    public static int GetClockCount(int employeeCode)
    {
        return GetEmployeeClockTimes(employeeCode).Where(c => c.DtDate == DateTime.Now.Date).ToList().Count;
    }

    public static void AddTimestamp(ClockTime clock)
    {
        try
        {
            Conn().InsertOrReplace(clock);
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - AddTimestamp");
        }
    }

    public static void InsertOrUpdateAll<T>(List<T> items)
    {
        try
        {
            var conn = Conn();
            conn.RunInTransaction(() =>
            {
                foreach (var item in items)
                {
                    _ = conn.InsertOrReplace(item);
                }
            });
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - InsertOrUpdateAll");
        }
    }

    public static int GetPendingClockCount()
    {
        try
        {
            var conn = Conn();
            return conn.Query<ClockTime>("Select * FROM ClockTime WHERE Status = ?;", EClockStatus.Pending).Count;
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetPendingClockCount");
            return 0;
        }
    }

    public static List<Employee> GetManagers()
    {
        try
        {
            var managerCodes = GetManagerIDs();
            return Conn().Query<Employee>($"SELECT * FROM Employee WHERE Code in ({string.Join(", ", managerCodes)})");
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetManagers");
            return new List<Employee>();
        }
    }

    public static List<int> GetManagerIDs()
    {
        try
        {
            return Conn().Query<Employee>("SELECT DISTINCT ReportsToCode FROM Employee;").Select(e => e.ReportsToID).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetManagerIDs");
            return new List<int>();
        }
    }

    /// <summary>
    /// Pulls a list of Shift Entries from the database with the relevant applied clock times.
    /// </summary>
    /// <param name="start">The starting filter date.</param>
    /// <param name="end">The ending filter date.</param>
    /// <param name="managerCode">The manager's code to filter by. -1 means that there will be no manager filter.</param>
    /// <returns></returns>
    public static List<DailyEntry> GetFilteredEntries(DateTime start, DateTime end, int managerCode = -1)
    {
        var startString = start.ToString("yyyy-MM-dd");
        var endString = end.ToString("yyyy-MM-dd");
        List<DailyEntry> entries = new();
        Dictionary<int, Employee> employees = new();
        Dictionary<Guid, ClockTime> clocks = new();
        try
        {
            Conn().RunInTransaction(() =>
            {
                using var conn = Conn();
                employees = conn.Table<Employee>().ToDictionary(e => e.ID, e => e);
                clocks = conn.Query<ClockTime>("SELECT * FROM ClockTime WHERE Timestamp BETWEEN ? AND ?;",
                        startString, end.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy-MM-dd HH:mm:ss"))
                    .ToDictionary(c => c.ID, c => c);

                if (managerCode == -1) // No particular manager.
                    entries = conn.Query<DailyEntry>("SELECT ShiftEntry.* FROM ShiftEntry JOIN Employee E on ShiftEntry.EmployeeID = E.Code " +
                                                     "WHERE Date BETWEEN ? AND ?;", startString, endString);
                else
                    entries = conn.Query<DailyEntry>("SELECT ShiftEntry.* FROM ShiftEntry JOIN Employee E on ShiftEntry.EmployeeID = E.Code " +
                                                     "WHERE E.ReportsToCode = ? AND Date BETWEEN ? AND ?;", managerCode, startString, endString);
            });
            // Assign the objects according to keys and dict.
            foreach (var entry in entries)
            {
                if (employees.TryGetValue(entry.EmployeeCode, out var emp))
                    entry.Employee = emp;

                if (clocks.TryGetValue(entry.StartShiftClockID, out var useClock))
                    entry.StartShiftClock = useClock;
                if (clocks.TryGetValue(entry.StartLunchClockID, out useClock))
                    entry.StartLunchClock = useClock;
                if (clocks.TryGetValue(entry.EndLunchClockID, out useClock))
                    entry.EndLunchClock = useClock;
                if (clocks.TryGetValue(entry.EndShiftClockID, out useClock))
                    entry.EndShiftClock = useClock;
            }

            return entries;
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetManagerIDs");
            return new List<DailyEntry>();
        }
    }

    /// <summary>
    /// Given a manager, returns a list of the employees that report directly to them.
    /// </summary>
    /// <param name="managerCode"></param>
    /// <returns>A list of employees.</returns>
    public static List<Employee> GetManagedEmployees(int managerCode)
    {
        try
        {
            var conn = Conn();
            return conn.Query<Employee>("SELECT * FROM Employee WHERE ReportsToCode = ?;", managerCode).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetManagedEmployees");
            return new List<Employee>();
        }
    }

    public static List<Employee> GetManagedEmployees(Employee manager)
    {
        return GetManagedEmployees(manager.ID);
    }

    public static List<int> GetEmployeeCodes()
    {
        try
        {
            return Conn().Query<Employee>("SELECT DISTINCT Code FROM Employee;").Select(e => e.ID).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmployeeCodes");
            return new List<int>();
        }
    }

    public static List<string> GetPayPoints()
    {
        try
        {
            return Conn().Query<Employee>("SELECT DISTINCT PayPoint FROM Employee;").Select(e => e.PayPoint).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmployeeCodes");
            return new List<string>();
        }
    }

    public static List<EEmploymentType> GetEmploymentTypes()
    {
        try
        {
            return Conn().Query<Employee>("SELECT DISTINCT EmploymentType FROM Employee;").Select(e => e.EmploymentType).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetEmploymentTypes");
            return new List<EEmploymentType>();
        }
    }

    public static List<string> GetJobClassifications()
    {
        try
        {
            return Conn().Query<Employee>("SELECT DISTINCT JobClassification FROM Employee;").Select(e => e.RoleName).ToList();
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - GetJobClassifications");
            return new List<string>();
        }
    }

    public static void DeleteMultiple<T>(List<T> itemList)
    {
        try
        {
            var conn = Conn();
            conn.RunInTransaction(() =>
            {
                foreach (var item in itemList)
                {
                    conn.Delete(item);
                }
            });
        }
        catch
        {
            _ = MessageBox.Show("Unexpected Error. DBUtil - DeleteMultiple");
        }
    }
}