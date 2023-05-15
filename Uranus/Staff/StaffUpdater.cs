using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Staff.Models;

namespace Uranus.Staff;

public class StaffUpdater
{
    private StaffChariot Chariot { get; }

    public StaffUpdater(ref StaffChariot chariot)
    {
        Chariot = chariot;
    }

    public int ClockEvent(ClockEvent clock) => Chariot.Update(clock);

    public int Employee(Employee employee) => Chariot.Update(employee);

    public async Task<int> ClockEventsAsync(IEnumerable<ClockEvent> clocks) => await Chariot.UpdateTableAsync(clocks);

    public int ClockEvents(IEnumerable<ClockEvent> clocks) => Chariot.UpdateTable(clocks);

    public async Task<int> ShiftEntryAsync(ShiftEntry shiftEntry) => await Chariot.InsertOrUpdateAsync(shiftEntry);

    public async Task<int> ShiftEntriesAsync(IEnumerable<ShiftEntry> shiftEntries) => await Chariot.UpdateTableAsync(shiftEntries);

    public int ShiftEntries(IEnumerable<ShiftEntry> shiftEntries) => Chariot.UpdateTable(shiftEntries);

    public async Task<int> EntriesAndClocksAsync(IEnumerable<ShiftEntry> shiftEntries, IEnumerable<ClockEvent> clockEvents)
    {
        var lines = 0;
        void Action()
        {
            lines += ClockEvents(clockEvents);
            lines += ShiftEntries(shiftEntries);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }

    public int EntriesAndClocks(IEnumerable<ShiftEntry> shiftEntries, IEnumerable<ClockEvent> clockEvents)
    {
        var lines = 0;
        void Action()
        {
            lines += ClockEvents(clockEvents);
            lines += ShiftEntries(shiftEntries);
        }

        Chariot.RunInTransaction(Action);

        return lines;
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
        Chariot.Execute("UPDATE ClockEvent SET Status = ? WHERE EmployeeID = ? AND Date = ?;",
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
    public async Task<int> ApplyPendingClockEventsAsync()
    {
        var lines = 0;
        void Action()
        {
            // Get full clocks and shifts.
            var clockList = Chariot.PullObjectList<ClockEvent>(c => c.Status != EClockStatus.Deleted);
            var entryList = Chariot.PullObjectList<ShiftEntry>();

            var clockDict = clockList.GroupBy(c => (c.EmployeeID, c.Date))
                .ToDictionary(g => g.Key, g => g.ToList());
            var entryDict = entryList.ToDictionary(e => (e.EmployeeID, e.Date), e => e);

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
                if (!clockDict.TryGetValue(valueTuple, out var clocks))
                    continue; // This should not occur, as the date and employee ID tuples are based on existing clocks, but let us be sure.

                if (!entryDict.TryGetValue(valueTuple, out var entry))
                    entry = new ShiftEntry { Date = valueTuple.Date, EmployeeID = valueTuple.EmployeeID };

                entry.ApplyClockTimes(clocks);

                updatedEntries.Add(entry);
                updatedClockEvents.AddRange(clocks);
            }

            // Apply changed object updates to the database.
            lines += EntriesAndClocks(updatedEntries, updatedClockEvents);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }

    /// <summary>
    /// Repair Aion Related data, fixing known potential issues such
    /// as duplicate data, or missing fields from tables.
    /// </summary>
    /// <returns>The number of rows of data affected by the repairs.</returns>
    public int RepairAionData()
    {
        var returnValue = 0;

        Chariot.RunInTransaction(() =>
        {
            // Remove duplicate values.
            returnValue += RemoveDuplicateShiftEntries();

            //Remove any entries without an employee or date value
            returnValue +=
                Chariot.Execute("DELETE FROM ShiftEntry WHERE Location is NULL or EmployeeID is NULL");

            // Make sure location is not null - use employeeID
            returnValue += Chariot.Execute("UPDATE ShiftEntry " +
                                                    "SET Location = (SELECT Employee.Location FROM Employee WHERE ShiftEntry.EmployeeID = Employee.ID) " +
                                                    "WHERE EXISTS " +
                                                    "(SELECT * FROM Employee " +
                                                    "WHERE ShiftEntry.EmployeeID = Employee.ID) " +
                                                    "AND ShiftEntry.Location is NULL;");

            // Get the shift entries to check and modify.
            IEnumerable<ShiftEntry> shiftEntries = Chariot.PullObjectList<ShiftEntry>();
            // Change the Day to match the date, as appropriate, and insert the data back into the database.
            foreach (var shiftEntry in shiftEntries)
            {
                var day = DateTime.Parse(shiftEntry.Date).DayOfWeek;
                if (shiftEntry.Day == day) continue;
                shiftEntry.Day = day;
                returnValue +=Chariot.InsertOrReplace(shiftEntry);
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
        return Chariot.Execute("DELETE FROM ShiftEntry WHERE ID NOT IN " +
                                        "(SELECT MIN(ID) FROM ShiftEntry GROUP BY EmployeeID, Date)");
    }

    /// <summary>
    /// Update the given list of employees in the database.
    /// </summary>
    /// <param name="employees"></param>
    /// <returns>Number of DB rows affected</returns>
    public int Employees(IEnumerable<Employee> employees) => Chariot.UpdateTable(employees);

    public async Task<int> EmployeesAsync(IEnumerable<Employee> employees) => await Chariot.UpdateTableAsync(employees);

    public int EmployeeIcon(EmployeeIcon icon) => Chariot.Update(icon);
    public int EmployeeAvatar(EmployeeAvatar avatar) => Chariot.Update(avatar);
    public int ProjectIcon(ProjectIcon icon) => Chariot.Update(icon);
    public int LicenceImage(LicenceImage image) => Chariot.Update(image);

    /// <summary>
    /// Renames an Employee Icon by removing it and creating a new one.
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="newName"></param>
    public void RenameEmployeeIcon(EmployeeIcon icon, string newName)
    {
        Chariot.RunInTransaction(() =>
        {
            var oldName = icon.Name;
            Chariot.Delete(icon);
            icon.Name = newName;
            Chariot.Create(icon);
            Chariot.Execute("UPDATE Employee SET IconName = ? WHERE IconName = ?;", newName, oldName);
        });
    }
    /// <summary>
    /// Renames an Employee Avatar by removing it and creating a new one.
    /// </summary>
    /// <param name="avatar"></param>
    /// <param name="newName"></param>
    public void RenameEmployeeAvatar(EmployeeAvatar avatar, string newName)
    {
        Chariot.RunInTransaction(() =>
        {
            var oldName = avatar.Name;
            Chariot.Delete(avatar);
            avatar.Name = newName;
            Chariot.Create(avatar);
            Chariot.Execute("UPDATE Employee SET AvatarName = ? WHERE AvatarName = ?;", newName, oldName);
        });

    }
    public int Shift(Shift shift)
    {
        var lines = 0;
        Chariot.RunInTransaction(() =>
        {
            lines += shift.Breaks.Sum(shiftBreak => Chariot.Update(shiftBreak));
            lines += Chariot.Update(shift);

            if (shift.Default)
            {
                Chariot.Execute("UPDATE Shift SET \"Default\" = FALSE WHERE DepartmentName = ? AND Name != ?;",
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
        Chariot.RunInTransaction(() =>
        {
            foreach (var employeeShift in empShifts)
            {
                switch (employeeShift.Active)
                {
                    case true when !employeeShift.Original:
                        lines += Chariot.Insert(employeeShift);
                        break;
                    case false when employeeShift.Original:
                        lines += Chariot.Execute("DELETE FROM EmployeeShift WHERE EmployeeID = ? AND ShiftID = ?;", employeeShift.EmployeeID, employeeShift.ShiftID);
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
    public async Task<int> DepartmentRosterAsync(DepartmentRoster departmentRoster)
    {
        var id = departmentRoster.ID;
        var lines = 0;

        void Action()
        {
            // Get Daily Roster IDs, to use to remove Daily Shift Counters, before deleting them.
            var dailyIDs = Chariot.PullObjectList<DailyRoster>(r => r.DepartmentRosterID == id).Select(roster => roster.ID);

            lines += Chariot.Execute("DELETE FROM Roster WHERE DepartmentRosterID = ?;", id);
            lines += Chariot.Execute("DELETE FROM DailyRoster WHERE DepartmentRosterID = ?;", id);
            lines += Chariot.Execute("DELETE FROM EmployeeRoster WHERE DepartmentRosterID = ?;", id);
            lines += Chariot.Execute("DELETE FROM WeeklyShiftCounter WHERE RosterID = ?;", id);

            lines += Chariot.Execute($"DELETE FROM DailyShiftCounter WHERE RosterID in ('{string.Join("', '", dailyIDs)}');");

            lines += Chariot.InsertIntoTable(departmentRoster.Rosters);
            lines += Chariot.InsertIntoTable(departmentRoster.DailyRosters());
            lines += Chariot.InsertIntoTable(departmentRoster.EmployeeRosters);
            lines += Chariot.InsertIntoTable(departmentRoster.ShiftCounters);
            lines += Chariot.InsertIntoTable(departmentRoster.DailyShiftCounters());
            lines += Chariot.Update(departmentRoster);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }

    /// <summary>
    /// Takes the given employee ID and sets the employee's IsUser value to false.
    /// </summary>
    /// <param name="employeeID"></param>
    /// <returns>True if successful.</returns>
    public bool DeactivateUser(int employeeID) => Chariot.Execute("UPDATE Employee SET IsUser = false WHERE ID = ?;", employeeID) > 0;

    public int ShiftRuleSingle(ShiftRuleSingle shiftRule) => Chariot.Update(shiftRule);

    public int ShiftRuleRecurring(ShiftRuleRecurring shiftRule) => Chariot.Update(shiftRule);

    public int ShiftRuleRoster(ShiftRuleRoster shiftRule) => Chariot.Update(shiftRule);

    /* Pick Event Tracking */

    /// <summary>
    /// Takes raw data (such as what would come fresh from the clipboard) and converts it to pick events (if possible), and updates as appropriate the
    /// relevant Pick Events, Pick Sessions, and PickStatisticsByDate.
    /// </summary>
    /// <returns>Number of events successfully uploaded to the database.</returns>
    public async Task<int> UploadPickHistoryDataAsync(string rawData, TimeSpan? ptlBreak = null, TimeSpan? rftBreak = null)
    {
        var events = PickEvent.GenerateFromRawData(rawData, out var sessions, out var stats, ptlBreak, rftBreak);

        return await UploadPickHistoryDataAsync(events, sessions, stats);
    }

    /// <summary>
    /// Updates pick events/sessions/dailyStats with new given data.
    ///
    /// Assumes data matches between types (stats => sessions => events) and is complete for the day.
    /// Caution: Will remove all previous data of the same day(s).
    /// </summary>
    /// <returns>Total number of lines impacted by database transactions.</returns>
    public async Task<int> UploadPickHistoryDataAsync(List<PickEvent> pickEvents, List<PickSession> sessions, List<PickDailyStats> dailyStats)
    {
        if (!pickEvents.Any()) return 0;
        var lines = 0;

        void Action()
        {
            // get relevant dates.
            var dates = pickEvents.Select(e => e.Date).Distinct().ToList();

            // Get ID reference dictionary to match employee ID key.
            var idDict = new Dictionary<string, int>();
            foreach (var employee in Chariot.PullObjectList<Employee>())
            {
                if (employee.DematicID != string.Empty && employee.DematicID != "0000" && !idDict.ContainsKey(employee.DematicID)) idDict.Add(employee.DematicID, employee.ID);
                if (employee.RF_ID != string.Empty && !idDict.ContainsKey(employee.RF_ID)) idDict.Add(employee.RF_ID, employee.ID);
            }

            // Assign actual OperatorID.
            foreach (var pickEvent in pickEvents.Where(e => e.OperatorID == 0))
            {
                if (idDict.TryGetValue(pickEvent.OperatorDematicID, out var id))
                    pickEvent.OperatorID = id;
                else if (idDict.TryGetValue(pickEvent.OperatorRF_ID, out id)) pickEvent.OperatorID = id;
            }

            // Remove existing database lines that may cause conflicts (they have been pulled out, so we should never lose any).
            foreach (var date in dates.Select(d => d.Ticks))
            {
                lines += Chariot.Execute("DELETE FROM PickEvent WHERE Date = ?;", date);
                lines += Chariot.Execute("DELETE FROM PickSession WHERE Date = ?;", date);
                lines += Chariot.Execute("DELETE FROM PickDailyStats WHERE Date = ?;", date);
            }

            // Enter new lines as appropriate.
            lines += Chariot.InsertIntoTable(pickEvents);
            lines += Chariot.InsertIntoTable(sessions);
            lines += Chariot.InsertIntoTable(dailyStats);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }

    public async Task<int> PickEventsAsync(IEnumerable<PickEvent> pickEvents) => await Chariot.UpdateTableAsync(pickEvents);

    public async Task<int> PickSessionsAsync(IEnumerable<PickSession> sessions) => await Chariot.UpdateTableAsync(sessions);

    public async Task<int> PickDailyStatsAsync(IEnumerable<PickDailyStats> stats) => await Chariot.UpdateTableAsync(stats);

    public async Task<int> PickStatsAsync(IEnumerable<PickEvent> events, IEnumerable<PickSession> sessions,
        IEnumerable<PickDailyStats> stats)
    {
        var lines = 0;

        void Action()
        {
            lines += Chariot.UpdateTable(events);
            lines += Chariot.UpdateTable(sessions);
            lines += Chariot.UpdateTable(stats);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }

    /* Temp Tags */
    public async Task<int> AssignTempTagAsync(TempTag tag, Employee employee)
    {
        var lines = 0;
        lines += UnassignTempTag(tag);

        void Action()
        {
            var usage = Chariot.GetValidUsage(employee, tag, DateTime.Today);
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
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }

    public int UnassignTempTag(TempTag tempTag)
    {
        var employeeID = tempTag.EmployeeID;
        if (employeeID == 0) return 0;

        tempTag.Unassign();

        var lines = 0;
        Chariot.RunInTransaction(() =>
        {
            lines += Chariot.Execute(
                "UPDATE TagUse SET EndDate = ? WHERE EmployeeID = ? AND TempTagRF_ID = ? AND EndDate is null;",
                DateTime.Today, employeeID, tempTag.RF_ID);

            lines += Chariot.Execute("UPDATE Employee SET TempTagRF_ID = ? WHERE ID = ?;", null, employeeID);

            lines += Chariot.Update(tempTag);
        });

        return lines;
    }

    /// <summary>
    /// Update the usage of the given tag, removing existing lines and replacing with the given.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>Number of lines affected in the database.</returns>
    public async Task<int> TagUsageAsync(TempTag tag)
    {
        // TODO: Figure out what this is supposed to do, and make it do that.
        // Removing all tag use for the given tag is almost certainly incorrect!
        var lines = 0;

        void Action()
        {
            lines += Chariot.Execute("DELETE FROM TagUse WHERE TempTagRF_ID = ?;", tag.RF_ID);
            lines += Chariot.InsertIntoTable(tag.TagUse);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }

    public async Task<int> TagUsageAsync(TagUse use) => await Chariot.InsertOrUpdateAsync(use);

    public async Task<int> MissPickAsync(MissPick missPick) => await Chariot.InsertOrUpdateAsync(missPick);

    /// <summary>
    /// Update pick data and miss pick data.
    /// Intended for use after error assignment.
    ///
    /// WARNING: Assumes data present represents all data for these days.
    /// </summary>
    /// <param name="missPicks"></param>
    /// <param name="pickEvents"></param>
    /// <param name="pickSessions"></param>
    /// <param name="stats"></param>
    /// <returns>The number of rows modified in the database as a result of this execution.</returns>
    public async Task<int> ErrorAssignmentAsync(List<MissPick> missPicks, List<PickEvent> pickEvents, List<PickSession> pickSessions, List<PickDailyStats> stats)
    {
        var lines = 0;

        // get dates
        var dates = missPicks.Select(m => m.ShipmentDate).ToList();
        dates.AddRange(pickEvents.Select(e => e.Date));

        dates = dates.Distinct().ToList();

        void Action()
        {
            // Remove existing data.
            foreach (var date in dates)
            {
                Chariot.Execute("DELETE FROM PickEvent WHERE Date = ?;", date);
                Chariot.Execute("DELETE FROM PickSession WHERE Date = ?;", date);
                Chariot.Execute("DELETE FROM PickDailyStats WHERE Date = ?;", date);
                Chariot.Execute("DELETE FROM MissPick WHERE ShipmentDate = ?;", date);
            }

            // Insert new data.
            lines += Chariot.InsertIntoTable(missPicks);
            lines += Chariot.InsertIntoTable(pickEvents);
            lines += Chariot.InsertIntoTable(pickSessions);
            lines += Chariot.InsertIntoTable(stats);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return lines;
    }
}