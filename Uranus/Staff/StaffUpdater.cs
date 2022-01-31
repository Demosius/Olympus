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
    }
}
