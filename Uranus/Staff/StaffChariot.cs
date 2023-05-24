using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Staff.Models;

namespace Uranus.Staff;

public class StaffChariot : MasterChariot
{
    public override string DatabaseName => "Staff.sqlite";

    public string EmployeeIconDirectory { get; set; }
    public string EmployeeAvatarDirectory { get; set; }
    public string ProjectIconDirectory { get; set; }
    public string LicenceImageDirectory { get; set; }

    public override Type[] Tables { get; } =
    {
        typeof(Clan),                       typeof(ClockEvent),         typeof(Department),                 typeof(DepartmentProject),
        typeof(Employee),                   typeof(EmployeeAvatar),     typeof(EmployeeDepartmentLoaning),  typeof(EmployeeIcon),
        typeof(EmployeeInductionReference), typeof(EmployeeProject),    typeof(EmployeeShift),              typeof(EmployeeVehicle),
        typeof(Induction),                  typeof(Licence),            typeof(LicenceImage),               typeof(Locker),
        typeof(Shift),                      typeof(ShiftEntry),         typeof(TagUse),
        typeof(ShiftRuleSingle),            typeof(ShiftRuleRecurring), typeof(ShiftRuleRoster),
        typeof(TempTag),                    typeof(Vehicle),            typeof(Break),                      typeof(Project),
        typeof(ProjectIcon),                typeof(Role),
        typeof(Roster),                     typeof(DepartmentRoster),   typeof(EmployeeRoster),             typeof(DailyRoster),
        typeof(DailyShiftCounter),          typeof(WeeklyShiftCounter),
        typeof(PickEvent),                  typeof(PickSession),        typeof(PickDailyStats),        typeof(Mispick)
    };

    /*************************** Constructors ****************************/

    public StaffChariot(string solLocation)
    {
        EmployeeIconDirectory = string.Empty;
        EmployeeAvatarDirectory = string.Empty;
        ProjectIconDirectory = string.Empty;
        LicenceImageDirectory = string.Empty;
        // Try first to use the given directory, if not then use local file.
        BaseDataDirectory = Path.Combine(solLocation, "Staff");

        InitializeDatabaseConnection();
    }

    /// <summary>
    /// Resets the connection using the given location string.
    /// </summary>
    /// <param name="solLocation">A directory location, in which the Staff database does/should reside.</param>
    public override void ResetConnection(string solLocation)
    {
        // First thing is to nullify the current database (connection).
        Database = null;

        BaseDataDirectory = Path.Combine(solLocation, "Staff");
        InitializeDatabaseConnection();
    }

    public void  CreateIconDirectories()
    {
        EmployeeIconDirectory = Path.Combine(BaseDataDirectory, "EmployeeIcons");
        EmployeeAvatarDirectory = Path.Combine(BaseDataDirectory, "EmployeeAvatars");
        ProjectIconDirectory = Path.Combine(BaseDataDirectory, "ProjectIcons");
        LicenceImageDirectory = Path.Combine(BaseDataDirectory, "LicenceImages");
        if (!Directory.Exists(BaseDataDirectory)) _ = Directory.CreateDirectory(BaseDataDirectory);
        if (!Directory.Exists(EmployeeIconDirectory)) _ = Directory.CreateDirectory(EmployeeIconDirectory);
        if (!Directory.Exists(EmployeeAvatarDirectory)) _ = Directory.CreateDirectory(EmployeeAvatarDirectory);
        if (!Directory.Exists(ProjectIconDirectory)) _ = Directory.CreateDirectory(ProjectIconDirectory);
        if (!Directory.Exists(LicenceImageDirectory)) _ = Directory.CreateDirectory(LicenceImageDirectory);
    }

    protected sealed override void InitializeDatabaseConnection()
    {
        base.InitializeDatabaseConnection();
        CreateIconDirectories();
    }

    /***************************** CREATE Data ****************************/

    /****************************** READ Data *****************************/

    public async Task<TagUse?> GetValidUsageAsync(int employeeID, string tempTagRFID, DateTime date) =>
        (await PullObjectListAsync<TagUse>(u =>
            u.EmployeeID == employeeID && u.TempTagRF_ID == tempTagRFID && u.StartDate <= date &&
            (u.EndDate == null || u.EndDate >= date))).MinBy(u => u.StartDate);

    public TagUse? GetValidUsage(int employeeID, string tempTagRFID, DateTime date) =>
        PullObjectList<TagUse>(u =>
            u.EmployeeID == employeeID && u.TempTagRF_ID == tempTagRFID && u.StartDate <= date &&
            (u.EndDate == null || u.EndDate >= date)).MinBy(u => u.StartDate);

    public async Task<TagUse?> GetValidUsageAsync(Employee employee, TempTag tempTag, DateTime date) =>
        await GetValidUsageAsync(employee.ID, tempTag.RF_ID, date);

    public TagUse? GetValidUsage(Employee employee, TempTag tempTag, DateTime date) =>
        GetValidUsage(employee.ID, tempTag.RF_ID, date);

    /***************************** UPDATE Data ****************************/

    /***************************** DELETE Data ****************************/

}