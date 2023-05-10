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

    public async Task<bool> SetShiftEntryAsync(DateTime date, Employee employee)
    {
        var returnVal = false;

        async void Action()
        {
            // Get suitable clock events.
            var clockTask = Chariot.PullObjectListAsync<ClockEvent>(c => c.Date == date.ToString("yyyy-MM-dd") && c.EmployeeID == employee.ID && c.Status != EClockStatus.Deleted);

            // Get existing entry - if it exists.
            var entryTask = Chariot.PullObjectListAsync<ShiftEntry>(e => e.Date == date.ToString("yyyy-MM-dd") && e.EmployeeID == employee.ID);

            var clocks = await clockTask;
            if (!clocks.Any()) return;

            var entryList = await entryTask;

            var entry = entryList.Any() ? entryList.First() : new ShiftEntry(employee, date);

            entry.ApplyClockTimes(clocks);

            var clockUpdateTask = Chariot.UpdateTableAsync(clocks);
            var entryUpdateTask = Chariot.InsertOrUpdateAsync(entry);

            var lines = await Task.WhenAll(clockUpdateTask, entryUpdateTask);

            returnVal = lines.Sum() > 0;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

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

    public async Task<EmployeeIcon?> CreateEmployeeIconFromSourceFileAsync(string sourceFile) => (EmployeeIcon?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.EmployeeIcon);
    public async Task<EmployeeAvatar?> CreateEmployeeAvatarFromSourceFileAsync(string sourceFile) => (EmployeeAvatar?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.EmployeeAvatar);
    public async Task<ProjectIcon?> CreateProjectIconFromSourceFileAsync(string sourceFile) => (ProjectIcon?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.ProjectIcon);
    public async Task<LicenceImage?> CreateLicenceImageFromSourceFileAsync(string sourceFile) => (LicenceImage?)await CreateImageFromSourceFileAsync(sourceFile, EImageType.LicenceImage);

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

        var existingIcons = (await Chariot.PullObjectListAsync<EmployeeIcon>()).ToDictionary(i => i.Name, i => i);

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

    public async Task<int> EmployeesAsync(IEnumerable<Employee> employees) => await Chariot.InsertIntoTableAsync(employees);

    public async Task<int> ShiftEntriesAsync(IEnumerable<ShiftEntry> entries) => await Chariot.InsertIntoTableAsync(entries);

    public async Task<int> ClockEventsAsync(IEnumerable<ClockEvent> clocks) => await Chariot.InsertIntoTableAsync(clocks);

    public async Task<int> AionDataSetAsync(AionDataSet dataSet)
    {
        var lines = 0;

        async void Action()
        {
            var tasks = new List<Task<int>>();
            if (dataSet.HasClockEvents()) tasks.Add(ClockEventsAsync(dataSet.ClockEvents.Select(entry => entry.Value)));
            if (dataSet.HasShiftEntries()) tasks.Add(ShiftEntriesAsync(dataSet.ShiftEntries.Select(entry => entry.Value)));
            if (dataSet.HasEmployees()) tasks.Add(EmployeesAsync(dataSet.Employees.Select(entry => entry.Value)));

            lines = (await Task.WhenAll(tasks)).Sum();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

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

    public async Task DepartmentRosterAsync(DepartmentRoster roster)
    {
        async void Action()
        {
            var tasks = new List<Task>
            {
                Chariot.InsertIntoTableAsync(roster.EmployeeRosters),
                Chariot.InsertIntoTableAsync(roster.Rosters),
                Chariot.InsertIntoTableAsync(roster.DailyRosters()),
                Task.Run(() => Chariot.Create(roster))
            };
            await Task.WhenAll(tasks);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
    }

    public void ShiftRuleSingle(ShiftRuleSingle shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public void ShiftRuleRecurring(ShiftRuleRecurring shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public void ShiftRuleRoster(ShiftRuleRoster shiftRule, EPushType pushType = EPushType.ObjectOnly) =>
        Chariot.Create(shiftRule, pushType);

    public async Task<int> ShiftRuleRostersAsync(IEnumerable<ShiftRuleRoster> rosterRules) => await Chariot.InsertIntoTableAsync(rosterRules);

    public bool TempTag(TempTag newTag, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(newTag, pushType);

    /* Miss Pick Data */
    public async Task<int> UploadMissPickDataAsync(string rawData) =>
        await UploadMissPickDataAsync(DataConversion.RawStringToMissPicks(rawData));

    public async Task<int> UploadMissPickDataAsync(List<MissPick> missPicks)
    {
        if (!missPicks.Any()) return 0;
        var lines = 0;

        async void Action()
        {
            // Figure out what data, if any, already exists within the data base.
            // What is the date range?
            var dates = missPicks.Select(mp => mp.ShipmentDate).ToList();
            var startDate = dates.Min();
            var endDate = dates.Max();
            var existingData =
                (await Chariot.PullObjectListAsync<MissPick>(mp =>
                    mp.ShipmentDate >= startDate && mp.ShipmentDate <= endDate))
                .ToDictionary(mp => mp.ID, mp => mp);

            var newLines = missPicks.Where(mp => !existingData.ContainsKey(mp.ID));

            lines += await Chariot.InsertIntoTableAsync(newLines);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return lines;
    }

    public int PickEvents(List<PickEvent> events) => Chariot.Database?.InsertAll(events) ?? 0;

    public int PickSessions(List<PickSession> sessions) => Chariot.Database?.InsertAll(sessions) ?? 0;

    public int PickStats(List<PickDailyStats> pickStats) => Chariot.Database?.InsertAll(pickStats) ?? 0;
}