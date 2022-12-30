using System;
using System.IO;
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
        typeof(Clan), typeof(ClockEvent), typeof(Department), typeof(DepartmentProject), typeof(Employee),
        typeof(EmployeeAvatar), typeof(EmployeeDepartmentLoaning), typeof(EmployeeIcon),
        typeof(EmployeeInductionReference), typeof(EmployeeProject), typeof(EmployeeShift), typeof(EmployeeVehicle),
        typeof(Induction), typeof(Licence), typeof(LicenceImage), typeof(Locker), typeof(Shift), typeof(ShiftEntry),
        typeof(TagUse), typeof(ShiftRuleSingle), typeof(ShiftRuleRecurring), typeof(ShiftRuleRoster), typeof(TempTag),
        typeof(Vehicle), typeof(Break), typeof(Project), typeof(ProjectIcon), typeof(Role), typeof(Roster),
        typeof(DepartmentRoster), typeof(EmployeeRoster), typeof(DailyRoster), typeof(DailyShiftCounter),
        typeof(WeeklyShiftCounter)
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

    public void CreateIconDirectories()
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

    /***************************** UPDATE Data ****************************/

    /***************************** DELETE Data ****************************/

}