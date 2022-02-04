using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    /// <summary>
    /// A class for holding (converted to current) aion-relevant data.
    /// </summary>
    public class AionDataSet
    {
        public Dictionary<int, Employee> Employees { get; set; }
        public Dictionary<Guid, ClockEvent> ClockEvents { get; set; }
        public Dictionary<Guid, ShiftEntry> ShiftEntries { get; set; }

        public bool HasData()
        {
            return HasEmployees() || HasClockEvents() || HasShiftEntries();
        }

        public bool HasEmployees()
        {
            return Employees is not null && Employees.Any();
        }

        public bool HasClockEvents()
        {
            return ClockEvents is not null && ClockEvents.Any();
        }

        public bool HasShiftEntries()
        {
            return ShiftEntries is not null && ShiftEntries.Any();
        }
    }
}
