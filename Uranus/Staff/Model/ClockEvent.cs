using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.ComponentModel;

namespace Uranus.Staff.Model
{

    public enum EClockStatus
    {
        Pending,
        Approved,
        Rejected,
        Deleted
    }

    /// <summary>
    /// Represents an employee clock event, in which an employee has physically clocked in or out.
    /// Can also be created manually.
    /// </summary>
    public class ClockEvent : IEquatable<ClockEvent>, INotifyPropertyChanged
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        public string Timestamp { get; set; }

        private string date;
        public string Date
        {
            get => date ??= DateTime.Parse(Timestamp).ToString("yyyy-MM-dd");
            set => date = value;
        }

        private string time;
        public string Time
        {
            get => time ??= DateTime.Parse(Timestamp).ToString("HH:mm:ss");
            set => time = value;
        }

        public EClockStatus Status { get; set; }

        [ManyToOne(inverseProperty: "ClockTimes")]
        public Employee Employee { get; set; }

        [Ignore]
        public DateTime DtDate => DateTime.Parse(Date).Date;
        [Ignore]
        public TimeSpan DtTime => DateTime.Parse(Time).TimeOfDay;

        public ClockEvent()
        {
            ID = Guid.NewGuid();
        }

        public void StampTime()
        {
            StampTime(DateTime.Now);
        }

        public void StampTime(DateTime dateTime)
        {
            Timestamp = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            Date = dateTime.ToString("yyyy-MM-dd");
            Time = dateTime.ToString("HH:mm:ss");
        }

        public void StampTime(string timestamp)
        {
            var dateTime = DateTime.Parse(timestamp);
            StampTime(dateTime);
        }

        public void Approve()
        {
            Status = EClockStatus.Approved;
            OnPropertyChanged(nameof(Status));
        }

        public void Reject()
        {
            Status = EClockStatus.Rejected;
            OnPropertyChanged(nameof(Status));
        }

        public override string ToString()
        {
            return Time[..5];
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

            return string.CompareOrdinal(Timestamp, other.Timestamp) == 0;
        }

        public bool Equals(ClockEvent other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Date == other.Date && Time == other.Time;
        }

        public static bool operator ==(ClockEvent lh, ClockEvent rh)
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

        public static bool operator !=(ClockEvent lh, ClockEvent rh)
        {
            return !(lh == rh);
        }

        public static bool operator >(ClockEvent lh, ClockEvent rh)
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

        public static bool operator <(ClockEvent lh, ClockEvent rh)
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

        public static bool operator <=(ClockEvent lh, ClockEvent rh) => lh == rh || lh < rh;

        public static bool operator >=(ClockEvent lh, ClockEvent rh) => lh == rh || lh > rh;

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => ID.GetHashCode();
        
        // Property changed event handling.
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

    }

}
