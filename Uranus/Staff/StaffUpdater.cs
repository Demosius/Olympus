using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Staff.Models;

namespace Uranus.Staff;

public class StaffUpdater
{
    private StaffChariot Chariot { get; }
    private StaffReader Reader { get; }

    public StaffUpdater(ref StaffChariot chariot, StaffReader reader)
    {
        Chariot = chariot;
        Reader = reader;
    }

    public int ClockEvent(ClockEvent clock) => Chariot.Update(clock);

    public int Employee(Employee employee) => Chariot.Update(employee);

    public void ClockEvents(IEnumerable<ClockEvent> clocks)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var clockEvent in clocks)
            {
                Chariot.InsertOrUpdate(clockEvent);
            }
        });
    }

    public void ShiftEntry(ShiftEntry shiftEntry) => Chariot.InsertOrUpdate(shiftEntry);

    public void ShiftEntries(IEnumerable<ShiftEntry> shiftEntries)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var entry in shiftEntries)
            {
                Chariot.InsertOrUpdate(entry);
            }
        });
    }

    public void EntriesAndClocks(IEnumerable<ShiftEntry> shiftEntries, IEnumerable<ClockEvent> clockEvents)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var entry in shiftEntries)
            {
                Chariot.InsertOrUpdate(entry);
            }

            foreach (var clockEvent in clockEvents)
            {
                Chariot.InsertOrUpdate(clockEvent);
            }
        });
    }

    /// <summary>
    /// Denotes a deleted shift entry, and sets the associated Clock Events to pending.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="employee"></param>
    public void SetPending(DateTime date, Employee employee)
    {
        SetPending(date.ToString("yyyy-MM-dd"), employee.ID);
    }
    /// <summary>
    /// Denotes a deleted shift entry, and sets the associated Clock Events to pending.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="employeeID"></param>
    public void SetPending(string date, int employeeID)
    {
        Chariot.Database?.Execute("UPDATE ClockEvent SET Status = ? WHERE EmployeeID = ? AND Date = ?;",
            EClockStatus.Pending, employeeID, date);
    }
    /// <summary>
    /// Denotes a deleted shift entry, and sets the associated Clock Events to pending.
    /// </summary>
    /// <param name="shiftEntry"></param>
    public void SetPending(ShiftEntry shiftEntry)
    {
        SetPending(shiftEntry.Date, shiftEntry.EmployeeID);
    }
    /// <summary>
    /// Denotes a deleted shift entry, and sets the associated Clock Events to pending.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="employeeID"></param>
    public void SetPending(DateTime date, int employeeID)
    {
        SetPending(date.ToString("yyyy-MM-dd"), employeeID);
    }
    /// <summary>
    /// Denotes a deleted shift entry, and sets the associated Clock Events to pending.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="employee"></param>
    public void SetPending(string date, Employee employee)
    {
        SetPending(date, employee.ID);
    }

    /// <summary>
    /// Gets the pending clock events and applies them to existing and new Shift Entries.
    /// </summary>
    public void ApplyPendingClockEvents()
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            // Get full clocks and shifts.
            var clockDict = Chariot.Database.Query<ClockEvent>("SELECT * FROM ClockEvent WHERE Status <> ?;", EClockStatus.Deleted)
                .GroupBy(c => (c.EmployeeID, c.Date))
                .ToDictionary(g => g.Key, g => g.ToList());
            var entryDict = Chariot.Database.Table<ShiftEntry>()
                .ToDictionary(e => (e.EmployeeID, e.Date), e => e);

            // Get list of dates and employee IDs for pending clock events.
            var checkList = clockDict.SelectMany(d => d.Value)
                .Where(c => c.Status == EClockStatus.Pending)
                .Select(c => (c.EmployeeID, c.Date));

            // Set up lists for holding updated values.
            List<ClockEvent> updatedClockEvents = new();
            List<ShiftEntry> updatedEntries = new();

            // Iterate through the check list to identify what clock groups and entries we need to modify.
            foreach (var valueTuple in checkList)
            {
                if (!clockDict.TryGetValue(valueTuple, out var clocks)) continue; // This should not occur, as the date and employee ID tuples are based on existing clocks, but let us be sure.

                if (!entryDict.TryGetValue(valueTuple, out var entry))
                    entry = new ShiftEntry { Date = valueTuple.Date, EmployeeID = valueTuple.EmployeeID };

                entry.ApplyClockTimes(clocks);

                updatedEntries.Add(entry);
                updatedClockEvents.AddRange(clocks);
            }

            // Apply changed object updates to the database.
            ShiftEntries(updatedEntries);
            ClockEvents(updatedClockEvents);
        });
    }

    /// <summary>
    /// Repair Aion Related data, fixing known potential issues such
    /// as duplicate data, or missing fields from tables.
    /// </summary>
    /// <returns>The number of rows of data affected by the repairs.</returns>
    public int RepairAionData()
    {
        var returnValue = 0;

        Chariot.Database?.RunInTransaction(() =>
        {
            // Remove duplicate values.
            returnValue += RemoveDuplicateShiftEntries();

            //Remove any entries without an employee or date value
            returnValue +=
                Chariot.Database.Execute("DELETE FROM ShiftEntry WHERE Location is NULL or EmployeeID is NULL");

            // Make sure location is not null - use employeeID
            returnValue += Chariot.Database.Execute("UPDATE ShiftEntry " +
                                                    "SET Location = (SELECT Employee.Location FROM Employee WHERE ShiftEntry.EmployeeID = Employee.ID) " +
                                                    "WHERE EXISTS " +
                                                    "(SELECT * FROM Employee " +
                                                    "WHERE ShiftEntry.EmployeeID = Employee.ID) " +
                                                    "AND ShiftEntry.Location is NULL;");

            // Get the shift entries to check and modify.
            IEnumerable<ShiftEntry> shiftEntries = Chariot.Database.Table<ShiftEntry>();
            // Change the Day to match the date, as appropriate, and insert the data back into the database.
            foreach (var shiftEntry in shiftEntries)
            {
                var day = DateTime.Parse(shiftEntry.Date).DayOfWeek;
                if (shiftEntry.Day == day) continue;
                shiftEntry.Day = day;
                returnValue += Chariot.Database.InsertOrReplace(shiftEntry);
            }
        });

        return returnValue;
    }

    /// <summary>
    /// Remove duplicate (employee - date) shift entries.
    /// </summary>
    /// <returns>The number of rows removed.</returns>
    public int RemoveDuplicateShiftEntries()
    {
        return Chariot.Database?.Execute("DELETE FROM ShiftEntry WHERE ID NOT IN " +
                                        "(SELECT MIN(ID) FROM ShiftEntry GROUP BY EmployeeID, Date)") ?? 0;
    }

    /// <summary>
    /// Update the given list of employees in the database.
    /// </summary>
    /// <param name="employees"></param>
    /// <returns>Number of DB rows affected</returns>
    public int Employees(IEnumerable<Employee> employees)
    {
        var returnVal = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            returnVal = employees.Sum(employee => Chariot.Database.InsertOrReplace(employee));
        });
        return returnVal;
    }

    public int EmployeeIcon(EmployeeIcon icon) => Chariot.Update(icon);
    public int EmployeeAvatar(EmployeeAvatar avatar) => Chariot.Update(avatar);
    public int ProjectIcon(ProjectIcon icon) => Chariot.Update(icon);
    public int LicenceImage(LicenceImage image) => Chariot.Update(image);

    /// <summary>
    /// Renames an Employee Icon by removing it and creating a new one.
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="newName"></param>
    public void RenameEmployeeIcon(ref EmployeeIcon icon, string newName)
    {
        var oldName = icon.Name;
        Chariot.Delete(icon);
        icon.Name = newName;
        Chariot.Create(icon);
        Chariot.Database?.Execute("UPDATE Employee SET IconName = ? WHERE IconName = ?;", newName, oldName);

    }
    /// <summary>
    /// Renames an Employee Avatar by removing it and creating a new one.
    /// </summary>
    /// <param name="avatar"></param>
    /// <param name="newName"></param>
    public void RenameEmployeeAvatar(ref EmployeeAvatar avatar, string newName)
    {
        var oldName = avatar.Name;
        Chariot.Delete(avatar);
        avatar.Name = newName;
        Chariot.Create(avatar);
        Chariot.Database?.Execute("UPDATE Employee SET AvatarName = ? WHERE AvatarName = ?;", newName, oldName);

    }
    public int Shift(Shift shift)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            lines += shift.Breaks.Sum(shiftBreak => Chariot.Update(shiftBreak));
            lines += Chariot.Update(shift);

            if (shift.Default)
            {
                Chariot.Database.Execute("UPDATE Shift SET \"Default\" = FALSE WHERE DepartmentName = ? AND Name != ?;",
                    shift.DepartmentName, shift.Name);
            }
        });
        return lines;
    }

    /// <summary>
    /// Updates EmployeeShift objects.
    /// Checks original vs active status to know what to create or delete.
    /// </summary>
    /// <param name="empShifts"></param>
    /// <returns></returns>
    public int EmployeeToShiftConnections(IEnumerable<EmployeeShift> empShifts)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var employeeShift in empShifts)
            {
                switch (employeeShift.Active)
                {
                    case true when !employeeShift.Original:
                        lines += Chariot.Database.Insert(employeeShift);
                        break;
                    case false when employeeShift.Original:
                        lines += Chariot.Database.Execute("DELETE FROM EmployeeShift WHERE EmployeeID = ? AND ShiftID = ?;", employeeShift.EmployeeID, employeeShift.ShiftID);
                        break;
                }
            }
        });
        return lines;
    }

    /// <summary>
    /// Saves relevant data from the given department roster, assuming it is appropriately filled with
    /// DailyRosters, EmployeeRosters, and Roster objects.
    ///
    /// Remove all previous data tied to this department roster so that all appropriate changes are saved, including roster deletions.
    /// </summary>
    /// <param name="departmentRoster">Filled and initialized department roster with daily/employee/roster references.</param>
    public int DepartmentRoster(DepartmentRoster departmentRoster)
    {
        var id = departmentRoster.ID;
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            lines += Chariot.Database!.Execute("DELETE FROM Roster WHERE DepartmentRosterID = ?;", id);
            lines += Chariot.InsertIntoTable(departmentRoster.Rosters);
            // Get Daily Roster IDs, to use to remove Daily Shift Counters, before deleting them.
            var dailyIDs = Chariot.PullObjectList<DailyRoster>(r => r.DepartmentRosterID == id).Select(roster => roster.ID);
            lines += Chariot.Database.Execute("DELETE FROM DailyRoster WHERE DepartmentRosterID = ?;", id);
            lines += Chariot.InsertIntoTable(departmentRoster.DailyRosters());
            lines += Chariot.Database.Execute("DELETE FROM EmployeeRoster WHERE DepartmentRosterID = ?;", id);
            lines += Chariot.InsertIntoTable(departmentRoster.EmployeeRosters);
            lines += Chariot.Database.Execute("DELETE FROM WeeklyShiftCounter WHERE RosterID = ?;", id);
            lines += Chariot.InsertIntoTable(departmentRoster.ShiftCounters);
            lines += Chariot.Database.Execute($"DELETE FROM DailyShiftCounter WHERE RosterID in ('{string.Join("', '", dailyIDs)}');");
            lines += Chariot.InsertIntoTable(departmentRoster.DailyShiftCounters());
            lines += Chariot.Database.Update(departmentRoster);
        });
        return lines;
    }

    /// <summary>
    /// Takes the given employee ID and sets the employee's IsUser value to false.
    /// </summary>
    /// <param name="employeeID"></param>
    /// <returns>True if successful.</returns>
    public bool DeactivateUser(int employeeID) => Chariot.Database?.Execute("UPDATE Employee SET IsUser = false WHERE ID = ?;", employeeID) > 0;

    public int ShiftRuleSingle(ShiftRuleSingle shiftRule) => Chariot.Update(shiftRule);

    public int ShiftRuleRecurring(ShiftRuleRecurring shiftRule) => Chariot.Update(shiftRule);

    public int ShiftRuleRoster(ShiftRuleRoster shiftRule) => Chariot.Update(shiftRule);

    /* Temp Tags */
    public int AssignTempTag(TempTag tag, Employee employee)
    {
        var lines = 0;
        lines += UnassignTempTag(tag);

        Chariot.Database?.RunInTransaction(() =>
        {
            var usage = Reader.GetValidUsage(employee, tag, DateTime.Today);
            if (usage is null)
            {
                usage = tag.SetEmployee(employee, true);
                lines += usage is null ? 0 : Chariot.Create(usage) ? 1 : 0;
            }
            else
            {
                usage.EndDate = null;
                tag.SetEmployee(employee);
                tag.TagUse.Add(usage);
                employee.TagUse.Add(usage);
                lines += Chariot.InsertOrUpdate(usage);
            }
            lines += Chariot.InsertOrUpdate(tag);
            lines += Chariot.InsertOrUpdate(employee);
        });
        
        return lines;
    }
    
    public int UnassignTempTag(TempTag tempTag)
    {
        var employeeID = tempTag.EmployeeID;
        if (employeeID == 0) return 0;

        tempTag.Unassign();
        
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            lines += Chariot.Database.Execute(
                "UPDATE TagUse SET EndDate = ? WHERE EmployeeID = ? AND TempTagRF_ID = ? AND EndDate is null;",
                DateTime.Today, employeeID, tempTag.RF_ID);

            lines += Chariot.Database.Execute("UPDATE Employee SET TempTagRF_ID = ? WHERE ID = ?;", null, employeeID);

            lines += Chariot.Update(tempTag);
        });

        return lines;
    }

    /// <summary>
    /// Update the usage of the given tag, removing existing lines and replacing with the given.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>Number of lines affected in the database.</returns>
    public int TagUsage(TempTag tag)
    {
        var lines = 0;

        Chariot.Database?.RunInTransaction(() =>
        {
            lines += Chariot.Database.Execute("DELETE FROM TagUse WHERE TempTagRF_ID = ?;", tag.RF_ID);
            lines += Chariot.InsertIntoTable(tag.TagUse);
        });

        return lines;
    }

    public int TagUsage(TagUse use) => Chariot.InsertOrUpdate(use);
}