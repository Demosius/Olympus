using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Uranus.Staff.Model;

namespace Aion.Model;

public enum EShiftTypeAlpha
{
    D,  // Normal day shift
    M,  // Morning shift starting before 7am
    A   // Afternoon shift finishing after 6pm
}

public class DailyEntry : INotifyPropertyChanged
{
    // Fields for event properties.
    private string location;
    private ClockTime startShiftClock;
    private ClockTime startLunchClock;
    private ClockTime endLunchClock;
    private ClockTime endShiftClock;
    private EShiftTypeAlpha shiftTypeAlpha;
    private string timeTotal;
    private double hoursWorked;
    private string comments;

    [PrimaryKey]
    public Guid ID { get; set; }
    [ForeignKey(typeof(Employee))]
    public int EmployeeCode { get; set; }
    public string Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged(nameof(Location));
        }
    }
    public string Date { get; set; }
    public string Day { get; set; }
    [ForeignKey(typeof(ClockTime))]
    public Guid StartShiftClockID { get; set; }
    [ForeignKey(typeof(ClockTime))]
    public Guid StartLunchClockID { get; set; }
    [ForeignKey(typeof(ClockTime))]
    public Guid EndLunchClockID { get; set; }
    [ForeignKey(typeof(ClockTime))]
    public Guid EndShiftClockID { get; set; }
    public EShiftTypeAlpha ShiftTypeAlpha
    {
        get => shiftTypeAlpha;
        set
        {
            shiftTypeAlpha = value;
            OnPropertyChanged(nameof(ShiftTypeAlpha));
        }
    }
    public string TimeTotal
    {
        get => timeTotal;
        set
        {
            timeTotal = value;
            OnPropertyChanged(nameof(TimeTotal));
        }
    }
    public double HoursWorked
    {
        get => hoursWorked;
        set
        {
            hoursWorked = value;
            OnPropertyChanged(nameof(HoursWorked));
        }
    }
    public string Comments
    {
        get => comments;
        set
        {
            comments = value;
            OnPropertyChanged(nameof(Comments));
        }
    }

    [ManyToOne]
    public Employee Employee { get; set; }
    [OneToOne(foreignKey: nameof(StartShiftClockID))]
    public ClockTime StartShiftClock
    {
        get => startShiftClock;
        set
        {
            StartShiftClockID = SetClock(ref startShiftClock, value);
            OnPropertyChanged(nameof(StartShiftClock));
        }
    }
    [OneToOne(foreignKey: nameof(StartLunchClockID))]
    public ClockTime StartLunchClock
    {
        get => startLunchClock;
        set
        {
            StartLunchClockID = SetClock(ref startLunchClock, value);
            OnPropertyChanged(nameof(StartLunchClock));
        }
    }
    [OneToOne(foreignKey: nameof(EndLunchClockID))]
    public ClockTime EndLunchClock
    {
        get => endLunchClock;
        set
        {
            EndLunchClockID = SetClock(ref endLunchClock, value);
            OnPropertyChanged(nameof(EndLunchClock));
        }
    }
    [OneToOne(foreignKey: nameof(EndShiftClockID))]
    public ClockTime EndShiftClock
    {
        get => endShiftClock;
        set
        {
            EndShiftClockID = SetClock(ref endShiftClock, value);
            OnPropertyChanged(nameof(EndShiftClock));
        }
    }

    [Ignore]
    public List<ClockTime> AdditionalClocks { get; set; }

    public DailyEntry() { }

    public DailyEntry(Employee employee, List<ClockTime> clockTimes)
    {
        ID = Guid.NewGuid();
        EmployeeCode = employee.ID;
        Location = employee.LocationCode;
        var d = clockTimes[0].DtDate;
        Date = d.ToString("yyyy-MM-dd");
        Day = d.ToString("dddd");

        AssignClockTimes(clockTimes);
        SummarizeShift();
    }

    public DailyEntry(Employee employee, DateTime date)
    {
        ID = Guid.NewGuid();
        EmployeeCode = employee.ID;
        Location = employee.LocationCode;
        Date = date.ToString("yyyy-MM-dd");
        Day = date.ToString("dddd");
    }

    // ReSharper disable once RedundantAssignment
    private static Guid SetClock(ref ClockTime clock, ClockTime newClockValue)
    {
        clock = newClockValue;
        if (clock is null)
            return Guid.NewGuid();
        clock.Status = EClockStatus.Approved;
        return clock.ID;
    }

    /// <summary>
    /// Pulls a list of all the clocks, including rejected(additional) clocks and the approved clocks.
    /// </summary>
    /// <returns></returns>
    public List<ClockTime> GetClocks()
    {
        var returnVal = AdditionalClocks ?? new List<ClockTime>();

        if (StartShiftClock is not null) { returnVal.Add(StartShiftClock); }
        if (StartLunchClock is not null) { returnVal.Add(StartLunchClock); }
        if (EndLunchClock is not null) { returnVal.Add(EndLunchClock); }
        if (EndShiftClock is not null) { returnVal.Add(EndShiftClock); }

        return returnVal;
    }

    /// <summary>
    /// Auto assign clock times to specific events.
    /// (Start/End Shift/Lunch)
    /// </summary>
    /// <param name="clockTimes"></param>
    private void AssignClockTimes(List<ClockTime> clockTimes)
    {
        // Make sure we grab the most relevant clocks first, if there is not a full count.
        clockTimes = clockTimes.OrderBy(c => c.Timestamp).ToList();
        if (clockTimes.Count > 0)
        {
            StartShiftClock = clockTimes[0];
            StartShiftClockID = StartShiftClock.ID;
            StartShiftClock.Status = EClockStatus.Approved;
        }
        if (clockTimes.Count > 1)
        {
            EndShiftClock = clockTimes.Last();
            EndShiftClockID = EndShiftClock.ID;
            EndShiftClock.Status = EClockStatus.Approved;
        }
        if (clockTimes.Count > 2)
        {
            StartLunchClock = clockTimes[1];
            StartLunchClockID = StartLunchClock.ID;
            StartLunchClock.Status = EClockStatus.Approved;
        }
        if (clockTimes.Count > 3)
        {
            EndLunchClock = clockTimes[2];
            EndLunchClockID = EndLunchClock.ID;
            EndLunchClock.Status = EClockStatus.Approved;
        }

        // Reject additional times - which should be all except the last one, and the first 3.
        if (clockTimes.Count > 4)
        {
            AdditionalClocks = clockTimes.Skip(3).Take(clockTimes.Count - 4).ToList();
            foreach (var clock in AdditionalClocks)
            {
                clock.Status = EClockStatus.Rejected;
            }
        }
    }

    /// <summary>
    /// Clear the specific clocks from properties, and returns them as a loose list.
    /// </summary>
    /// <returns>List of Clocks that were previously assigned.</returns>
    private IEnumerable<ClockTime> ClearClocks()
    {
        List<ClockTime> clocks = new();
        if (StartShiftClock is not null)
        {
            clocks.Add(StartShiftClock);
            StartShiftClock.Status = EClockStatus.Pending;
            StartShiftClock = null;
        }
        if (StartLunchClock is not null)
        {
            clocks.Add(StartLunchClock);
            StartLunchClock.Status = EClockStatus.Pending;
            StartLunchClock = null;
        }
        if (EndLunchClock is not null)
        {
            clocks.Add(EndLunchClock);
            EndLunchClock.Status = EClockStatus.Pending;
            EndLunchClock = null;
        }
        if (EndShiftClock is not null)
        {
            clocks.Add(EndShiftClock);
            EndShiftClock.Status = EClockStatus.Pending;
            EndShiftClock = null;
        }
        StartShiftClockID = Guid.Empty;
        StartLunchClockID = Guid.Empty;
        EndLunchClockID = Guid.Empty;
        EndShiftClockID = Guid.Empty;

        return clocks;
    }

    /// <summary>
    /// Assuming the entry has no clock objects assigned
    /// </summary>
    /// <param name="clocks"></param>
    public void ApplyClocks(Dictionary<Guid, ClockTime> clocks)
    {
        if (clocks.TryGetValue(StartShiftClockID, out var clock))
        {
            clock.Status = EClockStatus.Approved;
            StartShiftClock = clock;
            clocks.Remove(StartShiftClockID);
        }

        if (clocks.TryGetValue(StartLunchClockID, out clock))
        {
            clock.Status = EClockStatus.Approved;
            StartLunchClock = clock;
            clocks.Remove(StartLunchClockID);
        }

        if (clocks.TryGetValue(EndLunchClockID, out clock))
        {
            clock.Status = EClockStatus.Approved;
            EndLunchClock = clock;
            clocks.Remove(EndLunchClockID);
        }

        if (clocks.TryGetValue(EndShiftClockID, out clock))
        {
            clock.Status = EClockStatus.Approved;
            EndShiftClock = clock;
            clocks.Remove(EndShiftClockID);
        }

        AdditionalClocks = new List<ClockTime>();
        foreach (var c in clocks.Values)
        {
            c.Status = EClockStatus.Rejected;
            AdditionalClocks.Add(c);
        }
    }

    public void AddClockTimes(List<ClockTime> newClocks)
    {
        if (newClocks.Count == 0) return;
        var clocks = ClearClocks().Concat(newClocks).ToList();
        AssignClockTimes(clocks);
        SummarizeShift();
    }

    /// <summary>
    /// Given the shift clock times, summarizes the total shift and break times, and shift type.
    /// </summary>
    public void SummarizeShift()
    {
        // Can't be summarized if there is not at least 2 clocks.
        if (StartShiftClock is null || EndShiftClock is null) return;
        if (StartShiftClock.DtTime < new TimeSpan(6, 50, 0))  // Use 650 as those set to start at 700 will clock in up to 10 minutes before their shift.
            ShiftTypeAlpha = EShiftTypeAlpha.M;
        else if (EndShiftClock.DtTime > new TimeSpan(18, 0, 0))
            ShiftTypeAlpha = EShiftTypeAlpha.A;
        else
            ShiftTypeAlpha = EShiftTypeAlpha.D;

        // Shift lunch break is set to 30 min for afternoon shift, otherwise is 40 minutes. 
        // Regardless of actual clocks - but only apply if start lunch is not null.
        var workSpan = EndShiftClock.DtTime.Subtract(StartShiftClock.DtTime);

        // Only subtract lunch break if shift is over 3 hours, and there is at least a initial lunch clock.
        if (StartLunchClock is not null && workSpan > new TimeSpan(3, 0, 0))
            workSpan = workSpan.Subtract(new TimeSpan(0, ShiftTypeAlpha == EShiftTypeAlpha.A ? 30 : 40, 0));

        TimeTotal = new DateTime(workSpan.Ticks).ToString("HH:mm");
        HoursWorked = workSpan.TotalHours;
    }

    public void SetStartShiftClock(ClockTime newStart)
    {
        StartShiftClock = newStart;
        StartShiftClockID = newStart.ID;
    }

    public void SetStartLunchClock(ClockTime newStart)
    {
        StartLunchClock = newStart;
        StartLunchClockID = newStart.ID;
    }

    public void SetEndLunchClock(ClockTime newEnd)
    {
        EndLunchClock = newEnd;
        EndLunchClockID = newEnd.ID;
    }

    public void SetEndShiftClock(ClockTime newEnd)
    {
        EndShiftClock = newEnd;
        EndShiftClockID = newEnd.ID;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"{EmployeeCode} - {Employee}: {Day} {Date}";
    }


    /* Equality overloading. */

    public override bool Equals(object obj)
    {
        if (obj is not ClockTime other)
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return ID == other.ID;
    }

    public bool Equals(DailyEntry other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ID == other.ID;
    }

    public static bool operator ==(DailyEntry lh, DailyEntry rh)
    {
        if (ReferenceEquals(lh, rh))
        {
            return true;
        }
        if (lh is null)
        {
            return false;
        }
        if (rh is null)
        {
            return false;
        }

        return lh.Equals(rh);
    }

    public static bool operator !=(DailyEntry lh, DailyEntry rh)
    {
        return !(lh == rh);
    }

    public static bool operator >(DailyEntry lh, DailyEntry rh)
    {
        if (ReferenceEquals(lh, rh))
        {
            return false;
        }
        if (lh is null)
        {
            return false;
        }
        if (rh is null)
        {
            return false;
        }

        return string.CompareOrdinal(lh.Date, rh.Date) > 0 || lh.Date == rh.Date && string.CompareOrdinal(lh.Date, rh.Date) > 0;
    }

    public static bool operator <(DailyEntry lh, DailyEntry rh)
    {
        if (ReferenceEquals(lh, rh))
        {
            return false;
        }
        if (lh is null)
        {
            return false;
        }
        if (rh is null)
        {
            return false;
        }

        return string.CompareOrdinal(lh.Date, rh.Date) < 0 || lh.Date == rh.Date && string.CompareOrdinal(lh.Date, rh.Date) < 0;
    }

    public static bool operator <=(DailyEntry lh, DailyEntry rh) => lh == rh || lh < rh;

    public static bool operator >=(DailyEntry lh, DailyEntry rh) => lh == rh || lh > rh;

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => ID.GetHashCode();
}