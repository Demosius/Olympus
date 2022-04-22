using System.Collections.Generic;
using Uranus.Staff.Model;

namespace Uranus.Staff;

public class StaffDeleter
{
    private StaffChariot Chariot { get; }

    public StaffDeleter(ref StaffChariot chariot)
    {
        Chariot = chariot;
    }

    public void ClockEvent(ClockEvent clockEvent) => Chariot.Database?.Delete(clockEvent);

    public void ClockEvents(IEnumerable<ClockEvent> clockEvents)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var clockEvent in clockEvents)
            {
                Chariot.Delete(clockEvent);
            }
        });
    }

    public void ShiftEntry(ShiftEntry selectedEntry) => Chariot.Database?.Delete(selectedEntry);

    public void ShiftEntries(IEnumerable<ShiftEntry> deletedEntries)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var deletedEntry in deletedEntries)
            {
                ShiftEntry(deletedEntry);
            }
        });
    }

    public void EntriesAndClocks(IEnumerable<ShiftEntry> deletedEntries, IEnumerable<ClockEvent> deletedClocks)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var deletedEntry in deletedEntries)
            {
                ShiftEntry(deletedEntry);
            }

            foreach (var deletedClock in deletedClocks)
            {
                ClockEvent(deletedClock);
            }
        });
    }

    /// <summary>
    /// Sets employee IsActive status as false. Does not perform a true delete for historic reference purposes.
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public int Employee(Employee employee) => Chariot.Database?.ExecuteScalar<int>("UPDATE Employee SET IsActive = ? WHERE ID = ?;", false, employee.ID) ?? 0;

    /// <summary>
    /// True deletion of employee from database. Use sparingly.
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public int EmployeeObliteration(Employee employee) => Chariot.Delete(employee) ? 1 : 0;

    public void Shift(Shift shift)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var shiftBreak in shift.Breaks)
                Chariot.Delete(shiftBreak);
            Chariot.Delete(shift);
        });
    }

    public void Break(Break @break) => Chariot.Delete(@break);

    /// <summary>
    /// Delete the given department roster, and every employee/daily/roster that references it, from the database.
    /// </summary>
    /// <param name="roster"></param>
    /// <returns></returns>
    public int DepartmentRoster(DepartmentRoster roster)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            lines += Chariot.Database.ExecuteScalar<int>("DELETE FROM Roster WHERE DepartmentRosterID = ?;", roster.ID);
            lines += Chariot.Database.ExecuteScalar<int>("DELETE FROM DailyRoster WHERE DepartmentRosterID = ?;", roster.ID);
            lines += Chariot.Database.ExecuteScalar<int>("DELETE FROM EmployeeRoster WHERE DepartmentRosterID = ?;", roster.ID);
            lines += Chariot.Database.Delete(roster);
        });
        return lines;
    }
}