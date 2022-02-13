using System;
using Uranus.Staff.Model;

namespace Aion.Model
{
    public class ExportEntry
    {
        public int AssociateNumber { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string In { get; set; }
        public string OutToLunch { get; set; }
        public string InFromLunch { get; set; }
        public string Out { get; set; }
        public string ShiftType { get; set; }
        public string Total { get; set; }
        public double TimeWorked { get; set; }
        public string Comments { get; set; }

        public readonly ShiftEntry ShiftEntry;

        /// <summary>
        /// ExportEntry is based on data from ShiftEntry, so must be initialized with one.
        /// </summary>
        /// <param name="entry"></param>
        public ExportEntry(ShiftEntry entry)
        {
            ShiftEntry = entry;
            SetData();
        }

        /// <summary>
        /// Sets the data based on the entry - overriding existing data and potential changes.
        /// Will not work if there is no set ShiftEntry.
        /// </summary>
        public void SetData()
        {
            AssociateNumber = ShiftEntry.EmployeeID;
            Name = ShiftEntry.Employee?.ToString();
            Location = ShiftEntry.Location;
            Date = ShiftEntry.Date;
            Day = ShiftEntry.Day.ToString();
            In = ShiftEntry.ShiftStartTime;
            OutToLunch = ShiftEntry.LunchStartTime;
            InFromLunch = ShiftEntry.LunchEndTime;
            Out = ShiftEntry.ShiftEndTime;
            ShiftType = ShiftEntry.ShiftType.ToString();
            Total = ShiftEntry.TimeTotal;
            TimeWorked = Math.Round(ShiftEntry.HoursWorked, 2);
            Comments = ShiftEntry.Comments;
        }

        /// <summary>
        /// Takes the data here in the Export Entry data and converts potential changes to the original ShiftEntry.
        /// </summary>
        public void ConvertEntry()
        {
            ShiftEntry.Location = Location;
            Enum.TryParse(typeof(EShiftTypeAlpha), ShiftType, true, out var st);
            ShiftEntry.ShiftType = (EShiftType)(st ?? EShiftType.D);
            ShiftEntry.Comments = Comments;

            var timesChanged = CheckForChange();

            ConvertTimes();

            if (ShiftEntry.TimeTotal == Total && Math.Abs(ShiftEntry.HoursWorked - TimeWorked) < 0.0001 && timesChanged)
                ShiftEntry.SummarizeShift();
            else
            {
                ShiftEntry.TimeTotal = Total;
                ShiftEntry.HoursWorked = TimeWorked;
            }
        }

        /// <summary>
        /// Checks to see if there is a difference between the export times and the entry times.
        /// Note - Invalid time entries (that will convert to null times) will count as an adjustment and therefore return true either way.
        /// </summary>
        /// <returns>True if there is any difference between the export time values and the original entry.</returns>
        private bool CheckForChange()
        {
            return In != ShiftEntry.ShiftStartTime ||
                   OutToLunch != ShiftEntry.LunchStartTime ||
                   InFromLunch != ShiftEntry.LunchEndTime ||
                   Out != ShiftEntry.ShiftEndTime;
        }

        /// <summary>
        /// Converts the times within the Export to the ShiftEntry, checking closest matching times, and creating new clock events when necessary.
        /// </summary>
        private void ConvertTimes()
        {
            ShiftEntry.ShiftStartTime = In;
            ShiftEntry.ShiftEndTime = Out;
            ShiftEntry.LunchStartTime = OutToLunch;
            ShiftEntry.LunchEndTime = InFromLunch;
        }
    }
}
