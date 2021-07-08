using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Olympus.Helios.Staff.Model
{
    public class Shift
    {
        public string Name { get; set; }
        public Department Department { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Break> Breaks { get; set; }

        public Shift() { }

        public Shift(string name, Department department, DateTime startTime, DateTime endTime, string breaks)
        {
            Name = name;
            Department = department;
            StartTime = startTime;
            EndTime = endTime;
            Breaks = JsonSerializer.Deserialize<List<Break>>(breaks);
        }

        public Shift(string name, Department department, DateTime startTime, DateTime endTime, List<Break> breaks)
        {
            Name = name;
            Department = department;
            StartTime = startTime;
            EndTime = endTime;
            Breaks = breaks;
        }

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
