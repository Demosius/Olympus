using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Staff.Models;

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
        Chariot.RunInTransaction(() =>
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
        Chariot.RunInTransaction(() =>
        {
            foreach (var deletedEntry in deletedEntries)
            {
                ShiftEntry(deletedEntry);
            }
        });
    }

    public async Task EntriesAndClocksAsync(IEnumerable<ShiftEntry> deletedEntries, IEnumerable<ClockEvent> deletedClocks)
    {
        await Task.Run(() =>
        {
            Chariot.RunInTransaction(() =>
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
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets employee IsActive status as false. Does not perform a true delete for historic reference purposes.
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public int Employee(Employee employee) => Chariot.ExecuteScalar<int>("UPDATE Employee SET IsActive = ? WHERE ID = ?;", false, employee.ID);

    /// <summary>
    /// True deletion of employee from database. Use sparingly.
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public int EmployeeObliteration(Employee employee) => Chariot.Delete(employee);

    public void Shift(Shift shift)
    {
        Chariot.RunInTransaction(() =>
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
        Chariot.RunInTransaction(() =>
        {
            lines += Chariot.ExecuteScalar<int>("DELETE FROM Roster WHERE DepartmentRosterID = ?;", roster.ID);
            lines += Chariot.ExecuteScalar<int>("DELETE FROM DailyRoster WHERE DepartmentRosterID = ?;", roster.ID);
            lines += Chariot.ExecuteScalar<int>("DELETE FROM EmployeeRoster WHERE DepartmentRosterID = ?;", roster.ID);
            lines += Chariot.Delete(roster);
        });
        return lines;
    }

    public int SingleRule(ShiftRuleSingle single) => Chariot.Delete(single);

    public int RecurringRule(ShiftRuleRecurring recurring) => Chariot.Delete(recurring);

    public int RosterRule(ShiftRuleRoster roster) => Chariot.Delete(roster);

    public int Department(Department department) => Chariot.Delete(department);

    public int Role(Role role) => Chariot.Delete(role);

    public void Clan(Clan clan)
    {
        Chariot.RunInTransaction(() =>
        {
            // Remove all associations first.
            Chariot.Execute("UPDATE Employee SET ClanName = null WHERE ClanName = ?;", clan.Name);
            Chariot.Delete(clan);
        });
    }

    /// <summary>
    /// Delete tag from database.
    /// Will fail if there is any use of it, as it will be required for historic data.
    /// </summary>
    /// <param name="tempTag"></param>
    /// <returns>The number of rows deleted.</returns>
    public int TempTag(TempTag tempTag)
    {
        if (tempTag.Employee is not null || tempTag.EmployeeID != 0 || tempTag.TagUse.Any()) return 0;

        var linesOfUse = 0;
        var deletedLines = 0;

        Chariot.RunInTransaction(() =>
        {
            linesOfUse += Chariot.ExecuteScalar<int>("SELECT COUNT(*) FROM Employee WHERE TempTagRF_ID = ?;",
                tempTag.RF_ID);
            linesOfUse += Chariot.ExecuteScalar<int>("SELECT COUNT(*) FROM TagUse WHERE TempTagRF_ID = ?;",
                tempTag.RF_ID);

            // Do not delete if still assigned and/or has historic use.
            if (linesOfUse > 0)
                return;
            deletedLines = Chariot.Delete(tempTag);
        });

        return deletedLines;
    }

    public int PickEvents(List<DateTime> dates)
    {
        var lines = 0;

        Chariot.RunInTransaction(() =>
        {
            lines += dates.Sum(date => Chariot.Execute("DELETE FROM PickEvent WHERE Date = ?;", date.Ticks));
        });

        return lines;
    }

    public int PickSessions(List<DateTime> dates)
    {
        var lines = 0;

        Chariot.RunInTransaction(() =>
        {
            lines += dates.Sum(date => Chariot.Execute("DELETE FROM PickSession WHERE Date = ?;", date.Ticks));
        });

        return lines;
    }

    public int PickStats(List<DateTime> dates)
    {
        var lines = 0;

        Chariot.RunInTransaction(() =>
        {
            lines += dates.Sum(date => Chariot.Execute("DELETE FROM PickStatisticsByDay WHERE Date = ?;", date.Ticks));
        });

        return lines;
    }

    public async Task<int> MispickAsync(Mispick mispick) => await Task.Run(() => Chariot.Delete(mispick)).ConfigureAwait(false);

    public async Task<int> RoleAsync(Role role) => await Task.Run(() => Chariot.Delete(role)).ConfigureAwait(false);

    public async Task<int> DepartmentAsync(Department department) => await Task.Run(() => Chariot.Delete(department)).ConfigureAwait(false);

    public async Task<int> ClanAsync(Clan clan) => await Task.Run(() => Chariot.Delete(clan)).ConfigureAwait(false);
    
}