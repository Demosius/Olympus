using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Aion.Model;
using Microsoft.Win32;
using SQLite;
using Uranus.Staff.Model;

namespace Aion.ViewModel.Utility;

/// <summary>
/// Utility class for handling and converting old data structures, and accessing old database systems.
/// </summary>
public static class OldDataUtil
{
    /// <summary>
    /// Allows the user to locate and identify an Aion.sqlite database.
    /// </summary>
    /// <returns>The string directory path of the database, which will be empty if invalid cancelled, or not found.</returns>
    public static string GetAionDB(string initialDirectory = "")
    {
        if (initialDirectory is null or "") initialDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = initialDirectory,
            Filter = "Aion Database |aion.sqlite",
            RestoreDirectory = true,
            Title = "Locate Aion Database"
        };
        return openFileDialog.ShowDialog() == true ? Path.GetFullPath(openFileDialog.FileName) : "";
    }

    public static bool InitializeDatabase(string dbPath)
    {
        var tables = new[]
        {
            typeof(BrokeEmployee), typeof(ClockTime), typeof(DailyEntry)
        };

        try
        {
            SQLiteConnection connection = new(dbPath);

            _ = connection.CreateTables(CreateFlags.None, tables);
            return true;

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unexpected Error: Initializing Archive Database:\n\n{ex.Message}", "Unexpected Exception");
            return false;
        }
    }

    public static Employee ConvertEmployee(BrokeEmployee employee)
    {
        Enum.TryParse(typeof(EEmploymentType), (employee.EmploymentType ?? "CA")[..2], true, out var result);
        var eType = (EEmploymentType)(result ?? EEmploymentType.CA.ToString());
        return new Employee
        {
            ID = employee.Code,
            FirstName = employee.FirstName,
            LastName = employee.Surname,
            DisplayName = employee.FirstName,
            Location = employee.Location,
            ReportsToID = employee.ReportsToCode,
            PayPoint = employee.PayPoint,
            EmploymentType = eType,
            RoleName = employee.JobClasification is null or "" ? "Unknown" : employee.JobClasification
        };
    }

    public static ShiftEntry ConvertShiftEntry(DailyEntry entry)
    {
        return new ShiftEntry
        {
            ID = entry.ID,
            EmployeeID = entry.EmployeeCode,
            Date = entry.Date,
            Day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), entry.Day),
            ShiftType = (EShiftType)entry.ShiftTypeAlpha,
            Location = entry.Location,
            TimeTotal = entry.TimeTotal,
            HoursWorked = entry.HoursWorked,
            Comments = entry.Comments,
            ShiftStartTime = entry.StartShiftClock?.Time,
            ShiftEndTime = entry.EndShiftClock?.Time,
            LunchStartTime = entry.StartLunchClock?.Time,
            LunchEndTime = entry.EndLunchClock?.Time
        };
    }

    public static ClockEvent ConvertClockEvent(ClockTime clock)
    {
        return new ClockEvent
        {
            ID = clock.ID,
            EmployeeID = clock.EmployeeCode,
            Timestamp = clock.Timestamp,
            Date = clock.Date,
            Time = clock.Time,
            Status = (Uranus.Staff.Model.EClockStatus)clock.Status
        };
    }

    public static AionDataSet GetArchivedData()
    {
        var dbPath = GetAionDB();
        if (dbPath is null or "") return new AionDataSet();

        if (!InitializeDatabase(dbPath)) return new AionDataSet();

        AionDataSet newSet = null;

        new SQLiteConnection(dbPath).RunInTransaction(() =>
        {
            using SQLiteConnection conn = new(dbPath);
            var employees = conn.Table<BrokeEmployee>();
            var clocks = conn.Table<ClockTime>();
            var entries = conn.Table<DailyEntry>().ToList();
            // Make sure the appropriate clock times are applied to the relevant daily entries.
            Dictionary<(int, string), Dictionary<Guid, ClockTime>> clockDict = clocks
                .GroupBy(c => (c.EmployeeCode, c.Date))
                .ToDictionary(g => g.Key, g => g.ToDictionary(c => c.ID, c => c));
            foreach (var dailyEntry in entries)
            {
                if (clockDict.TryGetValue((dailyEntry.EmployeeCode, dailyEntry.Date), out var entryValueDict))
                    dailyEntry.ApplyClocks(entryValueDict);
            }
                
            newSet = new AionDataSet
            {
                Employees = employees.Select(ConvertEmployee).ToDictionary(e => e.ID, e => e),
                ClockEvents = clocks.Select(ConvertClockEvent).ToDictionary(c => c.ID, c => c),
                ShiftEntries = entries.Select(ConvertShiftEntry).ToDictionary(e => e.ID, e => e)
            };
        });


        return newSet;
    }
}