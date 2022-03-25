using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uranus.Staff.Model;

namespace Uranus.Staff;

public class StaffCreator
{
    private StaffChariot Chariot { get; }

    public StaffCreator(ref StaffChariot chariot)
    {
        Chariot = chariot;
    }

    public bool Employee(Employee employee, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(employee, pushType);

    public bool ClockEvent(ClockEvent clockEvent) => Chariot.Create(clockEvent);

    public bool SetShiftEntry(DateTime date, Employee employee)
    {
        var returnVal = false;
        Chariot.Database?.RunInTransaction(() =>
        {
            // Get suitable clock events.
            var clocks = Chariot.Database.Query<ClockEvent>(
                "SELECT * FROM ClockEvent WHERE Date = ? AND EmployeeID = ? AND Status <> ?;",
                date.ToString("yyyy-MM-dd"), employee.ID, EClockStatus.Deleted);

            if (!clocks.Any()) return;

            // Get existing entry - if it exists.
            var entryList = Chariot.Database.Query<ShiftEntry>(
                "SELECT * FROM ShiftEntry WHERE Date = ? AND EmployeeID = ?;",
                date.ToString("yyyy-MM-dd"), employee.ID);

            var entry = entryList.Any() ? entryList.First() : new ShiftEntry(employee, date);

            entry.ApplyClockTimes(clocks);

            foreach (var clockEvent in clocks)
            {
                Chariot.InsertOrUpdate(clockEvent);
            }

            returnVal = Chariot.InsertOrUpdate(entry) > 0;
        });

        return returnVal;
    }

    public bool Department(Department department, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(department, pushType);

    public bool Role(Role role, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(role, pushType);

    public void EstablishInitialProjects(List<Project> newProjects)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            foreach (var project in newProjects)
            {
                Chariot.Database.InsertOrReplace(project);
                Chariot.Database.InsertOrReplace(project.Icon);
            }
        });
    }

    public void CopyProjectIconsFromSource(string sourceDirectory)
    {
        foreach (var filePath in Directory.GetFiles(sourceDirectory))
        {
            var fileName = Path.GetFileName(filePath);
            try
            {
                if (Path.GetExtension(filePath) == ".ico")
                    File.Copy(filePath, Path.Combine(Chariot.ProjectIconDirectory, fileName), true);
            }
            catch (IOException) { } // If the file can't be copied across, continue anyway.
        }
    }

    public int Employees(IEnumerable<Employee> employees) => Chariot.InsertIntoTable(employees);

    public int ShiftEntries(IEnumerable<ShiftEntry> entries) => Chariot.InsertIntoTable(entries);

    public int ClockEvents(IEnumerable<ClockEvent> clocks) => Chariot.InsertIntoTable(clocks);

    public int AionDataSet(AionDataSet dataSet)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            if (dataSet.HasEmployees()) lines += Employees(dataSet.Employees.Select(entry => entry.Value));
            if (dataSet.HasClockEvents()) lines += ClockEvents(dataSet.ClockEvents.Select(entry => entry.Value));
            if (dataSet.HasShiftEntries()) lines += ShiftEntries(dataSet.ShiftEntries.Select(entry => entry.Value));
        });
        return lines;
    }


    public bool Clan(Clan clan, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(clan, pushType);
}