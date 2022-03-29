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

    public void Employee(Employee selectedEmployee) => Chariot.Database?.Delete(selectedEmployee);

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
}