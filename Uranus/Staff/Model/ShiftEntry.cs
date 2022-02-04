using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        [PrimaryKey] public Guid ID { get; set; }
        [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
        [ForeignKey(typeof(Shift))] public string ShiftName { get; set; }

        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string LunchStartTime { get; set; }
        public string LunchEndTime { get; set; }

        private string location;
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

        private EShiftType shiftType;
        public EShiftType ShiftType
        {
            get => shiftType;
            set
            {
                shiftType = value;
                OnPropertyChanged(nameof(ShiftType));
            }
        }

        private string timeTotal;
        public string TimeTotal
        {
            get => timeTotal;
            set
            {
                timeTotal = value;
                OnPropertyChanged(nameof(TimeTotal));
            }
        }

        private double hoursWorked;
        public double HoursWorked
        {
            get => hoursWorked;
            set
            {
                hoursWorked = value;
                OnPropertyChanged(nameof(HoursWorked));
            }
        }

        private string comments;
        public string Comments
        {
            get => comments;
            set
            {
                comments = value;
                OnPropertyChanged(nameof(Comments));
            }
        }

        [ManyToOne("EmployeeID","ShiftEntries", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }

        [Ignore]
        public List<ClockEvent> ClockEvents { get; set; }

        public ShiftEntry()
        {
            ID = Guid.NewGuid();
        }

        public ShiftEntry(Employee employee, List<ClockEvent> clockTimes)
        {
            ID = Guid.NewGuid();
            EmployeeID = employee.ID;
            Location = employee.Location;
            var d = clockTimes[0].DtDate;
            Date = d.ToString("yyyy-MM-dd");
            Day = d.ToString("dddd");

            ApplyClockTimes(clockTimes);
            SummarizeShift();
        }

        public ShiftEntry(Employee employee, DateTime date)
        {
            ID = Guid.NewGuid();
            EmployeeID = employee.ID;
            Location = employee.Location;
            Date = date.ToString("yyyy-MM-dd");
            Day = date.ToString("dddd");
        }

        /// <summary>
        /// Sets the times based on the current clock events.
        /// (Start/End Shift/Lunch)
        /// </summary>
        public void ApplyClockTimes()
        {
            ApplyClockTimes(ClockEvents);
        }

        /// <summary>
        /// Sets the times based on the given clock events.
        /// (Start/End Shift/Lunch)
        /// </summary>
        /// <param name="clockTimes"></param>
        public void ApplyClockTimes(List<ClockEvent> clockTimes)
        {
            // Make sure we grab the most relevant clocks first, if there is not a full count.
            clockTimes = clockTimes.OrderBy(c => c.Timestamp).ToList();
            if (clockTimes.Count > 0)
            {
                ShiftStartTime = clockTimes[0].Time;
                clockTimes[0].Status = EClockStatus.Approved;
            }
            if (clockTimes.Count > 1)
            {
                ShiftEndTime = clockTimes.Last().Time;
                clockTimes.Last().Status = EClockStatus.Approved;
            }
            if (clockTimes.Count > 2)
            {
                LunchStartTime = clockTimes[1].Time;
                clockTimes[1].Status = EClockStatus.Approved;
            }
            if (clockTimes.Count > 3)
            {
                LunchEndTime = clockTimes[2].Time;
                clockTimes[2].Status = EClockStatus.Approved;
            }

            // Reject additional times - which should be all except the last one, and the first 3.
            if (clockTimes.Count <= 4) return;
            
            ClockEvents = clockTimes.Skip(3).Take(clockTimes.Count - 4).ToList();
            foreach (var clock in ClockEvents)
            {
                clock.Status = EClockStatus.Rejected;
            }

            SummarizeShift();
        }
        
        /// <summary>
        /// Given the shift times, summarizes the total shift and break times, and shift type.
        /// </summary>
        public void SummarizeShift()
        {
            // Can't be summarized if there is not at least 2 applied times.
            if (ShiftStartTime is null or "" || ShiftEndTime is null or "") return;
            
            if (DateTime.Parse(ShiftStartTime).TimeOfDay < new TimeSpan(6, 50, 0))  // Use 650 as those set to start at 700 will clock in up to 10 minutes before their shift.
                ShiftType = EShiftType.M;
            else if (DateTime.Parse(ShiftEndTime).TimeOfDay > new TimeSpan(18, 0, 0))
                ShiftType = EShiftType.A;
            else
                ShiftType = EShiftType.D;

            // Shift lunch break is set to 30 min for afternoon shift, otherwise is 40 minutes. 
            // Regardless of actual clocks - but only apply if start lunch is not null.
            var workSpan = DateTime.Parse(ShiftEndTime).TimeOfDay.Subtract(DateTime.Parse(ShiftStartTime).TimeOfDay);

            // Only subtract lunch break if shift is over 3 hours, and there is at least a initial lunch clock.
            if (LunchStartTime is not null and not "" && workSpan > new TimeSpan(3, 0, 0))
                workSpan = workSpan.Subtract(new(0, ShiftType == EShiftType.A ? 30 : 40, 0));

            TimeTotal = new DateTime(workSpan.Ticks).ToString("HH:mm");
            HoursWorked = workSpan.TotalHours;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public override string ToString()
        {
            return $"{EmployeeID} - {Employee?.FirstName} {Employee?.LastName}: {Day} {Date}";
        }


        /* Equality overloading. */

        public override bool Equals(object obj)
        {
            if (obj is not ClockEvent other)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
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
            return rh is not null && lh.Equals(rh);
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

            return string.CompareOrdinal(lh.Date, rh.Date) > 0 || lh.Date == rh.Date && string.CompareOrdinal(lh.Date, rh.Date) > 0;
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

            return string.CompareOrdinal(lh.Date, rh.Date) < 0 || lh.Date == rh.Date && string.CompareOrdinal(lh.Date, rh.Date) < 0;
        }

        public static bool operator <=(ShiftEntry lh, ShiftEntry rh) => lh == rh || lh < rh;

        public static bool operator >=(ShiftEntry lh, ShiftEntry rh) => lh == rh || lh > rh;

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => ID.GetHashCode();
    }

}
