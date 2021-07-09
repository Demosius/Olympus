using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Olympus.Helios.Staff.Model
{
    public class Shift
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string BreakString { get; set; }

        [ManyToOne]
        public Department Department { get; set; }
        [Ignore]
        public List<Break> Breaks { get; set; }
        [ManyToMany(typeof(EmployeeShift))]
        public List<Employee> Employees { get; set; }

    }

    public class Break
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public int Length { get; set; } // in minutes

        public Break() { }

        public Break(string name, DateTime startTime, int length)
        {
            Name = name;
            StartTime = startTime;
            Length = length;
        }
    }
}
