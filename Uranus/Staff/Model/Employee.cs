using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Uranus.Staff.Model;

public enum EEmploymentType
{
    [Description("Salary")]
    SA,
    [Description("Casual")]
    CA,
    [Description("Full-Time Permanent")]
    FP,
    [Description("Part-Time Permanent")]
    PP
}

public static class EmploymentTypeExtension
{
    public static string GetDescription(this EEmploymentType employmentType)
    {
        return employmentType switch
        {
            EEmploymentType.SA => "Salary",
            EEmploymentType.CA => "Casual",
            EEmploymentType.FP => "Full-Time Permanent",
            EEmploymentType.PP => "Part-Time Permanent",
            _ => ""
        };
    }
}

public class Employee : INotifyPropertyChanged
{
    #region Fields

    private string location;
    private string payPoint;
    private string firstName;
    private string lastName;
    private EEmploymentType employmentType;
    private Department? department;
    private Role? role;
    private Clan? clan;
    private EmployeeIcon? icon;
    private EmployeeAvatar? avatar;
    private bool isUser;

    #endregion

    [PrimaryKey] public int ID { get; set; } // Employee number (e.g. 60853)
    public string FirstName
    {
        get => firstName;
        set
        {
            firstName = value;
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(FullName));
        }
    }
    public string LastName
    {
        get => lastName;
        set
        {
            lastName = value;
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(FullName));
        }
    }
    [SQLite.NotNull] public string DisplayName { get; set; }
    public decimal? PayRate { get; set; }
    public string RF_ID { get; set; }
    public string PC_ID { get; set; }
    public string Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged(nameof(Location));
        }
    }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Role))] public string RoleName { get; set; }    // Also known as Job Classification.
    [ForeignKey(typeof(Employee))] public int ReportsToID { get; set; }    // Specific Employee this employee reports to, bypassing Role and RoleReports.
    [ForeignKey(typeof(Clan))] public string ClanName { get; set; }
    [ForeignKey(typeof(Shift))] public string DefaultShiftID { get; set; }
    public string PayPoint
    {
        get => payPoint;
        set
        {
            payPoint = value;
            OnPropertyChanged(nameof(PayPoint));
        }
    }
    public EEmploymentType EmploymentType
    {
        get => employmentType;
        set
        {
            employmentType = value;
            OnPropertyChanged(nameof(EmploymentType));
        }
    }
    [ForeignKey(typeof(Locker))] public string LockerID { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    [ForeignKey(typeof(EmployeeIcon))] public string IconName { get; set; }
    [ForeignKey(typeof(EmployeeAvatar))] public string AvatarName { get; set; }
    [ForeignKey(typeof(Licence))] public string LicenceNumber { get; set; }
    [DefaultValue(false)]
    public bool IsUser
    {
        get => isUser;
        set
        {
            isUser = value;
            OnPropertyChanged();
        }
    }
    [DefaultValue(true)] public bool IsActive { get; set; }     // Employees are de-activated instead of deleted.

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Employees),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department
    {
        get => department;
        set
        {
            department = value;
            OnPropertyChanged(nameof(Department));
        }
    }
    [ManyToOne(nameof(RoleName), nameof(Model.Role.Employees),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Role? Role
    {
        get => role;
        set
        {
            role = value;
            OnPropertyChanged(nameof(Role));
        }
    }
    [ManyToOne(nameof(ReportsToID), nameof(Reports), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? ReportsTo { get; set; }
    [ManyToOne(nameof(AvatarName), nameof(EmployeeAvatar.Employees), CascadeOperations = CascadeOperation.CascadeRead)]
    public EmployeeAvatar? Avatar
    {
        get => avatar;
        set
        {
            avatar = value;
            OnPropertyChanged(nameof(Avatar));
        }
    }
    [ManyToOne(nameof(ClanName), nameof(Model.Clan.Employees),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Clan? Clan
    {
        get => clan;
        set
        {
            clan = value;
            OnPropertyChanged(nameof(Clan));
        }
    }
    [ManyToOne(nameof(IconName), nameof(EmployeeIcon.Employees),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public EmployeeIcon? Icon
    {
        get => icon;
        set
        {
            icon = value;
            OnPropertyChanged(nameof(Icon));
        }
    }
    [ManyToOne(nameof(DefaultShiftID), nameof(Shift.DefaultEmployees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Shift? DefaultShift { get; set; }

    [OneToOne(nameof(LockerID), nameof(Model.Locker.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Locker? Locker { get; set; }
    [OneToOne(nameof(LicenceNumber), nameof(Model.Licence.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Licence? Licence { get; set; }

    [ManyToMany(typeof(EmployeeVehicle), nameof(EmployeeVehicle.EmployeeID), nameof(Vehicle.Owners), CascadeOperations = CascadeOperation.All)]
    public List<Vehicle> Vehicles { get; set; }
    [ManyToMany(typeof(EmployeeShift), nameof(EmployeeShift.EmployeeID), nameof(Shift.Employees), CascadeOperations = CascadeOperation.All)]
    public List<Shift> Shifts { get; set; }
    [ManyToMany(typeof(EmployeeDepartmentLoaning), nameof(EmployeeDepartmentLoaning.EmployeeID), nameof(Model.Department.EmployeesCanLoan), CascadeOperations = CascadeOperation.All)]
    public List<Department> DepartmentsCanWorkIn { get; set; }
    [ManyToMany(typeof(EmployeeProject), nameof(EmployeeProject.EmployeeID), nameof(Project.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Project> Projects { get; set; }

    [OneToMany(nameof(ShiftRule.EmployeeID), nameof(ShiftRuleSingle.Employee), CascadeOperations = CascadeOperation.All)]
    public List<ShiftRuleSingle> SingleRules { get; set; }
    [OneToMany(nameof(ShiftRule.EmployeeID), nameof(ShiftRuleRecurring.Employee), CascadeOperations = CascadeOperation.All)]
    public List<ShiftRuleRecurring> RecurringRules { get; set; }
    [OneToMany(nameof(ShiftRule.EmployeeID), nameof(ShiftRuleRoster.Employee), CascadeOperations = CascadeOperation.All)]
    public List<ShiftRuleRoster> RosterRules { get; set; }

    [OneToMany(nameof(EmployeeInductionReference.EmployeeID), nameof(EmployeeInductionReference.Employee), CascadeOperations = CascadeOperation.All)]
    public List<EmployeeInductionReference> InductionReferences { get; set; }
    [OneToMany(nameof(ReportsToID), nameof(ReportsTo), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Reports { get; set; }
    [OneToMany(nameof(ShiftEntry.EmployeeID), nameof(ShiftEntry.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<ShiftEntry> ShiftEntries { get; set; }
    [OneToMany(nameof(Roster.EmployeeID), nameof(Roster.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(EmployeeRoster.EmployeeID), nameof(EmployeeRoster.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<EmployeeRoster> EmployeeRosters { get; set; }
    [OneToMany(nameof(ClockEvent.EmployeeID), nameof(ClockEvent.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<ClockEvent> ClockEvents { get; set; }
    [OneToMany(nameof(Model.TagUse.TempTagRFID), nameof(Model.TagUse.TempTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<TagUse> TagUse { get; set; }

    [Ignore] public string FullName => $"{FirstName} {LastName}";
    [Ignore] public string ReportsToName => ReportsTo?.FullName ?? "";

    public Employee()
    {
        firstName = string.Empty;
        lastName = string.Empty;
        DisplayName = string.Empty;
        RF_ID = string.Empty;
        PC_ID = string.Empty;
        location = string.Empty;
        DepartmentName = string.Empty;
        RoleName = string.Empty;
        ClanName = string.Empty;
        DefaultShiftID = string.Empty;
        payPoint = string.Empty;
        LockerID = string.Empty;
        LicenceNumber = string.Empty;
        PhoneNumber = string.Empty;
        Email = string.Empty;
        Address = string.Empty;
        IconName = string.Empty;
        AvatarName = string.Empty;
        Vehicles = new List<Vehicle>();
        Shifts = new List<Shift>();
        DepartmentsCanWorkIn = new List<Department>();
        Projects = new List<Project>();
        InductionReferences = new List<EmployeeInductionReference>();
        SingleRules = new List<ShiftRuleSingle>();
        RecurringRules = new List<ShiftRuleRecurring>();
        RosterRules = new List<ShiftRuleRoster>();
        Reports = new List<Employee>();
        ShiftEntries = new List<ShiftEntry>();
        Rosters = new List<Roster>();
        EmployeeRosters = new List<EmployeeRoster>();
        ClockEvents = new List<ClockEvent>();
        TagUse = new List<TagUse>();
    }

    public Employee(int id) : this()
    {
        ID = id;
    }

    public void AssignRole(Role newRole)
    {
        RoleName = newRole.Name;
        Role = newRole;
        newRole.Employees.Add(this);
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }

    public override bool Equals(object? obj) => Equals(obj as Employee);

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => ID;

    public bool Equals(Employee? employee)
    {
        if (employee is null) return false;

        if (ReferenceEquals(this, employee)) return true;

        if (GetType() != employee.GetType()) return false;

        return ID == employee.ID;
    }

    public static bool operator ==(Employee? lhs, Employee? rhs) => lhs?.Equals(rhs) ?? rhs is null;

    public static bool operator !=(Employee? lhs, Employee? rhs) => !(lhs == rhs);

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the employee object up for deletion by removing it as a reference from other related objects.
    /// </summary>
    public void Delete()
    {
        Role?.Employees.Remove(this);
        Department?.Employees.Remove(this);
        ReportsTo?.Reports.Remove(this);
        Clan?.Employees.Remove(this);
        Avatar?.Employees.Remove(this);
        Icon?.Employees.Remove(this);
        Locker = null;
        Licence = null;
        foreach (var vehicle in Vehicles)
            vehicle.Owners.Remove(this);
        foreach (var shift in Shifts)
            shift.Employees.Remove(this);
        foreach (var dept in DepartmentsCanWorkIn)
            dept.EmployeesCanLoan.Remove(this);
        foreach (var project in Projects)
            project.Employees.Remove(this);
        foreach (var employee in Reports)
            employee.ReportsTo = null;
        foreach (var shiftEntry in ShiftEntries)
            shiftEntry.Employee = null;
        foreach (var roster in Rosters)
            roster.Employee = null;
        foreach (var clockEvent in ClockEvents)
            clockEvent.Employee = null;
        foreach (var tagUse in TagUse)
            tagUse.Employee = null;
    }


    /// <summary>
    /// Sets the appropriate data keys according to the current data objects.
    /// e.g. DepartmentName = Department.Name
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void SetDataFromObjects()
    {
        DepartmentName = Department?.Name ?? DepartmentName;
        ReportsToID = ReportsTo?.ID ?? ReportsToID;
        RoleName = Role?.Name ?? RoleName;
        ClanName = Clan?.Name ?? ClanName;
        LockerID = Locker?.ID ?? LockerID;
        LicenceNumber = Licence?.Number ?? LicenceNumber;
        IconName = Icon?.Name ?? IconName;
        AvatarName = Avatar?.Name ?? AvatarName;
    }
}