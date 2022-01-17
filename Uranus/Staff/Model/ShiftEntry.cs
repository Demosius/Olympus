using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    public enum EShiftType
    {
        D,  // Normal day shift
        M,  // Morning shift starting before 7am
        A   // Afternoon shift finishing after 6pm
    }

    /// <summary>
    /// Daily entry of an acted shift. Whether the employee was present, or they were on leave or otherwise absent.
    /// </summary>
    public class ShiftEntry : INotifyPropertyChanged
    {
        // Fields for event properties.
        private string location;
        private ClockEvent startShiftClock;
        private ClockEvent startLunchClock;
        private ClockEvent endLunchClock;
        private ClockEvent endShiftClock;
        private EShiftType shiftType;
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
        [ForeignKey(typeof(ClockEvent))]
        public Guid StartShiftClockID { get; set; }
        [ForeignKey(typeof(ClockEvent))]
        public Guid StartLunchClockID { get; set; }
        [ForeignKey(typeof(ClockEvent))]
        public Guid EndLunchClockID { get; set; }
        [ForeignKey(typeof(ClockEvent))]
        public Guid EndShiftClockID { get; set; }
        public EShiftType ShiftType
        {
            get => shiftType;
            set
            {
                shiftType = value;
                OnPropertyChanged(nameof(ShiftType));
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
        public ClockEvent StartShiftClock
        {
            get => startShiftClock;
            set
            {
                StartShiftClockID = SetClock(ref startShiftClock, value);
                OnPropertyChanged(nameof(StartShiftClock));
            }
        }
        [OneToOne(foreignKey: nameof(StartLunchClockID))]
        public ClockEvent StartLunchClock
        {
            get => startLunchClock;
            set
            {
                StartLunchClockID = SetClock(ref startLunchClock, value);
                OnPropertyChanged(nameof(StartLunchClock));
            }
        }
        [OneToOne(foreignKey: nameof(EndLunchClockID))]
        public ClockEvent EndLunchClock
        {
            get => endLunchClock;
            set
            {
                EndLunchClockID = SetClock(ref endLunchClock, value);
                OnPropertyChanged(nameof(EndLunchClock));
            }
        }
        [OneToOne(foreignKey: nameof(EndShiftClockID))]
        public ClockEvent EndShiftClock
        {
            get => endShiftClock;
            set
            {
                EndShiftClockID = SetClock(ref endShiftClock, value);
                OnPropertyChanged(nameof(EndShiftClock));
            }
        }

        [Ignore]
        public List<ClockEvent> AdditionalClocks { get; set; }

        public ShiftEntry() { }

        public ShiftEntry(Employee employee, List<ClockEvent> clockTimes)
        {
            ID = Guid.NewGuid();
            EmployeeCode = employee.ID;
            Location = employee.Location;
            DateTime d = clockTimes[0].DTDate;
            Date = d.ToString("yyyy-MM-dd");
            Day = d.ToString("dddd");

            AssignClockEvents(clockTimes);
            SummarizeShift();
        }

        public ShiftEntry(Employee employee, DateTime date)
        {
            ID = Guid.NewGuid();
            EmployeeCode = employee.ID;
            Location = employee.Location;
            Date = date.ToString("yyyy-MM-dd");
            Day = date.ToString("dddd");
        }

        private static Guid SetClock(ref ClockEvent clock, ClockEvent newClockValue)
        {
            clock = newClockValue;
            if (clock is null)
                return Guid.NewGuid();
            else
            {
                clock.Status = EClockStatus.Approved;
                return clock.ID;
            }
        }

        /// <summary>
        /// Pulls a list of all the clocks, including rejected(additional) clocks and the approved clocks.
        /// </summary>
        /// <returns></returns>
        public List<ClockEvent> GetClocks()
        {
            List<ClockEvent> returnVal = AdditionalClocks ?? new();

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
        private void AssignClockEvents(List<ClockEvent> clockTimes)
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
                foreach (ClockEvent clock in AdditionalClocks)
                {
                    clock.Status = EClockStatus.Rejected;
                }
            }
        }

        /// <summary>
        /// Clear the specific clocks from properties, and returns them as a loose list.
        /// </summary>
        /// <returns>List of Clocks that were previously assigned.</returns>
        private List<ClockEvent> ClearClocks()
        {
            List<ClockEvent> clocks = new();
            if (StartShiftClock != null)
            {
                clocks.Add(StartShiftClock);
                StartShiftClock.Status = EClockStatus.Pending;
                StartShiftClock = null;
            }
            if (StartLunchClock != null)
            {
                clocks.Add(StartLunchClock);
                StartLunchClock.Status = EClockStatus.Pending;
                StartLunchClock = null;
            }
            if (EndLunchClock != null)
            {
                clocks.Add(EndLunchClock);
                EndLunchClock.Status = EClockStatus.Pending;
                EndLunchClock = null;
            }
            if (EndShiftClock != null)
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
        public void ApplyClocks(Dictionary<Guid, ClockEvent> clocks)
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

            AdditionalClocks = new();
            foreach (var c in clocks.Values)
            {
                c.Status = EClockStatus.Rejected;
                AdditionalClocks.Add(c);
            }
        }

        public void AddClockEvents(List<ClockEvent> newClocks)
        {
            if (newClocks.Count == 0) return;
            List<ClockEvent> clocks = ClearClocks().Concat(newClocks).ToList();
            AssignClockEvents(clocks);
            SummarizeShift();
        }

        /// <summary>
        /// Given the shift clock times, summarizes the total shift and break times, and shift type.
        /// </summary>
        public void SummarizeShift()
        {
            // Can't be summarized if there is not at least 2 clocks.
            if (StartShiftClock is null || EndShiftClock is null) return;
            if (StartShiftClock.DTTime < new TimeSpan(6, 50, 0))  // Use 650 as those set to start at 700 will clock in up to 10 minutes before their shift.
                ShiftType = EShiftType.M;
            else if (EndShiftClock.DTTime > new TimeSpan(18, 0, 0))
                ShiftType = EShiftType.A;
            else
                ShiftType = EShiftType.D;

            // Shift lunch break is set to 30 min for afternoon shift, otherwise is 40 mins. 
            // Regardless of actuall clocks - but only apply if start lunch is not null.
            TimeSpan workSpan = EndShiftClock.DTTime.Subtract(StartShiftClock.DTTime);

            // Only subtract lunch break if shift is over 3 hours, and there is at least a initial lunch clock.
            if (StartLunchClock != null && workSpan > new TimeSpan(3, 0, 0))
                workSpan = workSpan.Subtract(new TimeSpan(0, (ShiftType == EShiftType.A) ? 30 : 40, 0));

            TimeTotal = new DateTime(workSpan.Ticks).ToString("HH:mm");
            HoursWorked = workSpan.TotalHours;
        }

        public void SetStartShiftClock(ClockEvent newStart)
        {
            StartShiftClock = newStart;
            StartShiftClockID = newStart.ID;
        }

        public void SetStartLunchClock(ClockEvent newStart)
        {
            StartLunchClock = newStart;
            StartLunchClockID = newStart.ID;
        }

        public void SetEndLunchClock(ClockEvent newEnd)
        {
            EndLunchClock = newEnd;
            EndLunchClockID = newEnd.ID;
        }

        public void SetEndShiftClock(ClockEvent newEnd)
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
            return $"{EmployeeCode} - {Employee?.FirstName} {Employee?.LastName}: {Day} {Date}";
        }


        /* Equality overloading. */

        public override bool Equals(object obj)
        {
            if (obj is not ClockEvent other)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ID == other.ID;
        }

        public bool Equals(ShiftEntry other)
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

        public static bool operator ==(ShiftEntry lh, ShiftEntry rh)
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

        public static bool operator !=(ShiftEntry lh, ShiftEntry rh)
        {
            return !(lh == rh);
        }

        public static bool operator >(ShiftEntry lh, ShiftEntry rh)
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

            return string.Compare(lh.Date, rh.Date) > 0 || (lh.Date == rh.Date && string.Compare(lh.Date, rh.Date) > 0);
        }

        public static bool operator <(ShiftEntry lh, ShiftEntry rh)
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

            return string.Compare(lh.Date, rh.Date) < 0 || (lh.Date == rh.Date && string.Compare(lh.Date, rh.Date) < 0);
        }

        public static bool operator <=(ShiftEntry lh, ShiftEntry rh) => lh == rh || lh < rh;

        public static bool operator >=(ShiftEntry lh, ShiftEntry rh) => lh == rh || lh > rh;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
