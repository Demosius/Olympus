using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Uranus.Staff.Model;

namespace Aion.Model
{
    [Table("Employee")]
    public class BrokeEmployee : INotifyPropertyChanged
    {
        /* Fields - for notify property changed */
        private string surname;
        private string firstName;
        private string location;
        private int reportsToCode;
        private BrokeEmployee reportsTo;
        private string payPoint;
        private string employmentType;
        // ReSharper disable once IdentifierTypo
        private string jobClasification;

        /* Properties */
        [PrimaryKey]
        public int Code { get; set; }
        public string Surname
        {
            get => surname;
            set
            {
                surname = value;
                OnPropertyChanged(nameof(Surname));
            }
        }
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName= value;
                OnPropertyChanged(nameof(FirstName));
            }
        }
        public string Location
        {
            get => location;
            set
            {
                location= value;
                OnPropertyChanged(nameof(Location));
            }
        }
        [ForeignKey(typeof(BrokeEmployee))]
        public int ReportsToCode
        {
            get => reportsToCode;
            set
            {
                reportsToCode= value;
                OnPropertyChanged(nameof(ReportsToCode));
            }
        }
        public string PayPoint
        {
            get => payPoint;
            set
            {
                payPoint= value;
                OnPropertyChanged(nameof(PayPoint));
            }
        }
        public string EmploymentType
        {
            get => employmentType;
            set
            {
                employmentType= value;
                OnPropertyChanged(nameof(EmploymentType));
            }
        }
        // ReSharper disable once IdentifierTypo
        public string JobClasification
        {
            get => jobClasification;
            set
            {
                jobClasification= value;
                OnPropertyChanged(nameof(JobClasification));
            }
        }

        [OneToMany(inverseProperty: "ReportsTo", CascadeOperations = CascadeOperation.All)]
        public List<BrokeEmployee> Reports { get; set; }
        [ManyToOne(inverseProperty: "Reports")]
        public BrokeEmployee ReportsTo
        {
            get => reportsTo;
            set
            {
                reportsTo= value;
                OnPropertyChanged(nameof(ReportsTo));
            }
        }
        [OneToMany(inverseProperty: "Employee", CascadeOperations = CascadeOperation.All)]
        public List<ClockEvent> ClockTimes { get; set; }
        [OneToMany(inverseProperty: "Employee", CascadeOperations = CascadeOperation.All)]
        public List<ShiftEntry> ShiftEntries { get; set; }

        [Ignore]
        public string FullName => $"{FirstName} {Surname}";

        /// <summary>
        /// Adds a newly created timestamp (clock time event) for the employee for current time/date.
        /// </summary>
        /// <returns>The newly created clock time.</returns>
        public ClockEvent AddTimestamp()
        {
            ClockEvent clock = new() { EmployeeID = Code, Status = Uranus.Staff.Model.EClockStatus.Pending };
            clock.StampTime();
            ClockTimes?.Add(clock);
            return clock;
        }

        /// <summary>
        /// Converts the employee's ClockTimes to ShiftEntries. 
        /// </summary>
        public void ConvertClockToEntries()
        {
            if (ClockTimes is null) return;
            var pendingClocks = ClockTimes.Where(c => c.Status == Uranus.Staff.Model.EClockStatus.Pending).GroupBy(c => c.DtDate.ToString("yyyy-MM-dd")).ToDictionary(g => g.Key, g => g.ToList());
            if (pendingClocks.Count == 0) return;

            // Get existing Daily Entries in dictionary form for quick look up.
            var currentEntries = ShiftEntries.ToDictionary(d => d.Date, d => d);

            foreach (var (key, value) in pendingClocks)
            {
                if (currentEntries.ContainsKey(key))
                {
                    currentEntries[key].ApplyClockTimes(value);
                }
                else
                {
                    ShiftEntries.Add(new(new(), value));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public override string ToString()
        {
            return $"{FirstName} {Surname}";
        }
    }
}
