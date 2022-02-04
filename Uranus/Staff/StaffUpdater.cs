using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Staff.Model;

namespace Uranus.Staff
{
    public class StaffUpdater
    {
        private StaffChariot Chariot { get; }

        public StaffUpdater(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

        public int ClockEvent(ClockEvent clock) => Chariot.Update(clock);

        public void Employee(Employee employee) => Chariot.Update(employee);

        public void ClockEvents(IEnumerable<ClockEvent> clocks) 
        {
            Chariot.Database.RunInTransaction(() =>
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
            Chariot.Database.RunInTransaction(() =>
            {
                foreach (var entry in shiftEntries)
                {
                    Chariot.InsertOrUpdate(entry);
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
            Chariot.Database.Execute("UPDATE ClockEvent SET Status = ? WHERE EmployeeID = ? AND Date = ?;",
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
            Chariot.Database.RunInTransaction(() =>
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
                        entry = new() { Date = valueTuple.Date, EmployeeID = valueTuple.EmployeeID };

                    entry.ApplyClockTimes(clocks);

                    updatedEntries.Add(entry);
                    updatedClockEvents.AddRange(clocks);
                }

                // Apply changed object updates to the database.
                ShiftEntries(updatedEntries);
                ClockEvents(updatedClockEvents);
            });
        }
    }
}
