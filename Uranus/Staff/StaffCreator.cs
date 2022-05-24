using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Uranus.Staff.Model;

namespace Uranus.Staff;

public enum EImageType
{
    EmployeeIcon,
    EmployeeAvatar,
    ProjectIcon,
    LicenceImage
}

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

    public EmployeeIcon? CreateEmployeeIconFromSourceFile(string sourceFile) => (EmployeeIcon?)CreateImageFromSourceFile(sourceFile, EImageType.EmployeeIcon);
    public EmployeeAvatar? CreateEmployeeAvatarFromSourceFile(string sourceFile) => (EmployeeAvatar?)CreateImageFromSourceFile(sourceFile, EImageType.EmployeeAvatar);
    public ProjectIcon? CreateProjectIconFromSourceFile(string sourceFile) => (ProjectIcon?)CreateImageFromSourceFile(sourceFile, EImageType.ProjectIcon);
    public LicenceImage? CreateLicenceImageFromSourceFile(string sourceFile) => (LicenceImage?)CreateImageFromSourceFile(sourceFile, EImageType.LicenceImage);

    public Image? CreateImageFromSourceFile(string sourceFile, EImageType type)
    {
        if (!File.Exists(sourceFile)) return null;

        var fileName = Path.GetFileName(sourceFile);
        var name = Path.GetFileNameWithoutExtension(fileName);

        var newDirectory = type switch
        {
            EImageType.EmployeeAvatar => Chariot.EmployeeAvatarDirectory,
            EImageType.ProjectIcon => Chariot.ProjectIconDirectory,
            EImageType.EmployeeIcon => Chariot.EmployeeIconDirectory,
            EImageType.LicenceImage => Chariot.LicenceImageDirectory,
            _ => Path.GetDirectoryName(sourceFile) ?? ""
        };

        var existingIcons = Chariot.PullObjectList<EmployeeIcon>().ToDictionary(i => i.Name, i => i);

        var newName = name;
        var cnt = 0;
        while (existingIcons.ContainsKey(newName))
        {
            newName = $"{name}_{cnt}";
            ++cnt;
        }
        name = newName;
        fileName = $"{name}{Path.GetExtension(fileName)}";

        var image = new Image(newDirectory, name, fileName);

        var newFilePath = Path.Combine(newDirectory, fileName);

        if (!File.Exists(newFilePath)) File.Copy(sourceFile, newFilePath);

        image = type switch
        {
            EImageType.EmployeeAvatar => new EmployeeAvatar(image),
            EImageType.ProjectIcon => new ProjectIcon(image),
            EImageType.LicenceImage => new LicenceImage(image),
            EImageType.EmployeeIcon => new EmployeeIcon(image),
            _ => image
        };

        image.SetDirectory(newDirectory);

        Chariot.Create(image);

        return image;
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
            catch (IOException ex)
            {
                // If the file can't be copied across, continue anyway.
                Log.Error(ex, "Failed to transfer icon file. ({File Name}).", fileName);
            }
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

    public void Shift(Shift shift)
    {
        Chariot.Database?.RunInTransaction(() =>
        {
            Chariot.Create(shift);
            foreach (var shiftBreak in shift.Breaks)
            {
                Chariot.Create(shiftBreak);
            }
        });

    }

    public void Break(Break @break) => Chariot.Create(@break);

    public void DepartmentRoster(DepartmentRoster roster) => Chariot.Create(roster);

    public void ShiftRuleSingle(ShiftRuleSingle shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public void ShiftRuleRecurring(ShiftRuleRecurring shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public void ShiftRuleRoster(ShiftRuleRoster shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public int ShiftRuleRosters(IEnumerable<ShiftRuleRoster> rosterRules) => Chariot.InsertIntoTable(rosterRules);
}