using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Staff.Models;

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

    public async Task<int> SetShiftEntryAsync(DateTime date, Employee employee)
    {
        var lines = 0;

        void Action()
        {
            // Get suitable clock events.
            var clocks = Chariot.PullObjectList<ClockEvent>(c =>
                c.Date == date.ToString("yyyy-MM-dd") && c.EmployeeID == employee.ID &&
                c.Status != EClockStatus.Deleted);

            // Get existing entry - if it exists.
            var entryList = Chariot.PullObjectList<ShiftEntry>(e =>
                e.Date == date.ToString("yyyy-MM-dd") && e.EmployeeID == employee.ID);

            if (!clocks.Any()) return;

            var entry = entryList.Any() ? entryList.First() : new ShiftEntry(employee, date);

            entry.ApplyClockTimes(clocks);

            lines += Chariot.UpdateTable(clocks);
            lines += Chariot.InsertOrReplace(entry);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }

    public bool Department(Department department, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(department, pushType);

    public bool Role(Role role, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(role, pushType);

    public void EstablishInitialProjects(List<Project> newProjects)
    {
        Chariot.RunInTransaction(() =>
        {
            foreach (var project in newProjects)
            {
                Chariot.InsertOrReplace((object?) project);
                Chariot.InsertOrReplace((object?) project.Icon);
            }
        });
    }

    public async Task<EmployeeIcon?> CreateEmployeeIconFromSourceFileAsync(string sourceFile) => (EmployeeIcon?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.EmployeeIcon).ConfigureAwait(false);
    public async Task<EmployeeAvatar?> CreateEmployeeAvatarFromSourceFileAsync(string sourceFile) => (EmployeeAvatar?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.EmployeeAvatar).ConfigureAwait(false);
    public async Task<ProjectIcon?> CreateProjectIconFromSourceFileAsync(string sourceFile) => (ProjectIcon?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.ProjectIcon).ConfigureAwait(false);
    public async Task<LicenceImage?> CreateLicenceImageFromSourceFileAsync(string sourceFile) => (LicenceImage?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.LicenceImage).ConfigureAwait(false);

    public async Task<Image?> CreateImageFromSourceFileAsync(string sourceFile, EImageType type)
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

        var image = new Image();
        void Action()
        {
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

            image = new Image(newDirectory, name, fileName);

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
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return image;
    }

    public void CopyProjectIconsFromSource(string sourceDirectory)
    {
        foreach (var filePath in Directory.GetFiles(sourceDirectory))
        {
            var fileName = Path.GetFileName(filePath);
            try
            {
                if (Path.GetExtension(filePath) is ".ico" or ".svg")
                    File.Copy(filePath, Path.Combine(Chariot.ProjectIconDirectory, fileName), true);
            }
            catch (IOException ex)
            {
                // If the file can't be copied across, continue anyway.
                Log.Error(ex, "Failed to transfer icon file. ({File Name}).", fileName);
            }
        }
    }

    public async Task<int> EmployeesAsync(IEnumerable<Employee> employees) => await Chariot.InsertIntoTableAsync(employees).ConfigureAwait(false);

    public int Employees(IEnumerable<Employee> employees) => Chariot.InsertIntoTable(employees);

    public async Task<int> ShiftEntriesAsync(IEnumerable<ShiftEntry> entries) => await Chariot.InsertIntoTableAsync(entries).ConfigureAwait(false);

    public int ShiftEntries(IEnumerable<ShiftEntry> entries) => Chariot.InsertIntoTable(entries);

    public async Task<int> ClockEventsAsync(IEnumerable<ClockEvent> clocks) => await Chariot.InsertIntoTableAsync(clocks).ConfigureAwait(false);

    public int ClockEvents(IEnumerable<ClockEvent> clocks) => Chariot.InsertIntoTable(clocks);

    public async Task<int> AionDataSetAsync(AionDataSet dataSet)
    {
        var lines = 0;

        void Action()
        {
            lines += ClockEvents(dataSet.ClockEvents.Select(entry => entry.Value));
            lines += ShiftEntries(dataSet.ShiftEntries.Select(entry => entry.Value));
            lines += Employees(dataSet.Employees.Select(entry => entry.Value));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }


    public bool Clan(Clan clan, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(clan, pushType);

    public void Shift(Shift shift)
    {
        Chariot.RunInTransaction(() =>
        {
            Chariot.Create(shift);
            foreach (var shiftBreak in shift.Breaks)
            {
                Chariot.Create(shiftBreak);
            }
        });

    }

    public void Break(Break @break) => Chariot.Create(@break);

    public async Task DepartmentRosterAsync(DepartmentRoster roster)
    {
        void Action()
        {
            Chariot.InsertIntoTable(roster.EmployeeRosters);
            Chariot.InsertIntoTable(roster.Rosters);
            Chariot.InsertIntoTable(roster.DailyRosters());
            Chariot.Create(roster);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
    }

    public void ShiftRuleSingle(ShiftRuleSingle shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public void ShiftRuleRecurring(ShiftRuleRecurring shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public void ShiftRuleRoster(ShiftRuleRoster shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public async Task<int> ShiftRuleRostersAsync(IEnumerable<ShiftRuleRoster> rosterRules) => await Chariot.InsertIntoTableAsync(rosterRules).ConfigureAwait(false);

    public bool TempTag(TempTag newTag, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(newTag, pushType);

    /* Mispick Data */
    public async Task<int> UploadMispickDataAsync(string rawData) =>
        await UploadMispickDataAsync(DataConversion.RawStringToMispicks(rawData)).ConfigureAwait(false);

    public async Task<int> UploadMispickDataAsync(List<Mispick> mispicks)
    {
        if (!mispicks.Any()) return 0;
        var lines = 0;

        // Figure out what data, if any, already exists within the data base.
        // What is the date range?
        var dates = mispicks.Select(mp => mp.ShipmentDate).ToList();
        var startDate = dates.Min();
        var endDate = dates.Max();
        void Action()
        {
            var existingData = Chariot
                .PullObjectList<Mispick>(mp => mp.ShipmentDate >= startDate && mp.ShipmentDate <= endDate)
                .ToDictionary(mp => mp.ID, mp => mp);

            var newLines = mispicks.Where(mp => !existingData.ContainsKey(mp.ID)).ToList();

            Mispick.HandleDuplicateValues(ref newLines);

            lines += Chariot.InsertIntoTable(newLines);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public int PickEvents(List<PickEvent> events) => Chariot.Database?.InsertAll(events) ?? 0;

    public int PickSessions(List<PickSession> sessions) => Chariot.Database?.InsertAll(sessions) ?? 0;

    public int PickStats(List<PickDailyStats> pickStats) => Chariot.Database?.InsertAll(pickStats) ?? 0;

    /* QA Stats */
    /// <summary>
    /// Upload new QA Carton Data.
    /// Aligns to matching QA Lines and adjusts the dates of those to match with this data.
    /// </summary>
    /// <param name="qaCartons"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<int> QACartonsAsync(List<QACarton> qaCartons)
    {
        var lines = 0;

        void Action()
        {
            var cartonIDs = qaCartons.Select(c => c.ID);
            var qaLineDict = Chariot.PullObjectList<QALine>(l => cartonIDs.Contains(l.CartonID))
                .GroupBy(l => l.CartonID)
                .ToDictionary(g => g.Key, g => g.ToList());
            var qaLines = new List<QALine>();

            // Update QALine dates.
            foreach (var qaCarton in qaCartons)
            {
                if (!qaLineDict.TryGetValue(qaCarton.ID, out var qaLineGroup)) continue;
                foreach (var qaLine in qaLineGroup)
                {
                    qaLine.Date = qaCarton.Date;
                    qaLines.Add(qaLine);
                }
            }

            lines += Chariot.UpdateTable(qaCartons);
            lines += Chariot.UpdateTable(qaLines);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }

    /// <summary>
    /// Upload new QA Line Data.
    /// Align to matching QA Cartons to set the day.
    /// Creates mispick data where appropriate.
    /// </summary>
    /// <param name="qaLines"></param>
    /// <returns></returns>
    public async Task<int> QALinesAsync(List<QALine> qaLines)
    {
        var lines = 0;

        void Action()
        {
            var qaLineDict = qaLines.GroupBy(l => l.CartonID).ToDictionary(g => g.Key, g => g.ToList());
            var cartonIDs = qaLineDict.Keys.ToList();
            var qaCartons = Chariot.PullObjectList<QACarton>(c => cartonIDs.Contains(c.ID));

            // Update QALine dates.
            foreach (var qaCarton in qaCartons)
            {
                if (!qaLineDict.TryGetValue(qaCarton.ID, out var qaLineGroup)) continue;
                foreach (var qaLine in qaLineGroup)
                {
                    qaLine.Date = qaCarton.Date;
                    qaLines.Add(qaLine);
                }
            }

            // Get mispick data.
            var mispicks = qaLines.Select(qaLine => qaLine.GenerateMispick()).Where(mispick => mispick is not null).ToList();

            lines += Chariot.UpdateTable(qaLines);
            lines += Chariot.UpdateTable(mispicks);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }
}