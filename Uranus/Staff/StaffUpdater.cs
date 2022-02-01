using System;
using System.Collections.Generic;
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

        public bool ClockEvent(ClockEvent clock) => Chariot.Update(clock);

        public void Employee(Employee employee) => Chariot.Update(employee);

        public void ClockEvents(IEnumerable<ClockEvent> clocks) 
        {
            Chariot.Database.RunInTransaction(() =>
            {
                foreach (var clockEvent in clocks)
                {
                    Chariot.Update(clockEvent);
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
                    Chariot.Update(entry);
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
    }
}
