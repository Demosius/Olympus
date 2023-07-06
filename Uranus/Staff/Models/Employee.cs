using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Uranus.Staff.Models;

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

public class Employee
{
    [PrimaryKey] public int ID { get; set; } // Employee number (e.g. 60853)

    [SQLite.NotNull] public string DisplayName { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal? PayRate { get; set; }
    [Indexed] public string RF_ID { get; set; }
    [Indexed] public string PC_ID { get; set; }
    [Indexed, StringLength(4)] public string DematicID { get; set; }
    public string Location { get; set; }
    public string PayPoint { get; set; }
    public EEmploymentType EmploymentType { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Role))] public string RoleName { get; set; }    // Also known as Job Classification.
    [ForeignKey(typeof(Employee))] public int ReportsToID { get; set; }    // Specific Employee this employee reports to, bypassing Role and RoleReports.
    [ForeignKey(typeof(Clan))] public string ClanName { get; set; }
    [ForeignKey(typeof(Shift))] public string DefaultShiftID { get; set; }
    [ForeignKey(typeof(Locker))] public string LockerID { get; set; }
    [ForeignKey(typeof(EmployeeIcon))] public string IconName { get; set; }
    [ForeignKey(typeof(EmployeeAvatar))] public string AvatarName { get; set; }
    [ForeignKey(typeof(Licence))] public string LicenceNumber { get; set; }
    [ForeignKey(typeof(TempTag))] public string TempTagRF_ID { get; set; }

    [DefaultValue(false)] public bool IsUser { get; set; }
    [DefaultValue(true)] public bool IsActive { get; set; }     // Employees are de-activated instead of deleted.

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }
    [ManyToOne(nameof(RoleName), nameof(Models.Role.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Role? Role { get; set; }
    [ManyToOne(nameof(ReportsToID), nameof(Reports), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? ReportsTo { get; set; }
    [ManyToOne(nameof(AvatarName), nameof(EmployeeAvatar.Employees), CascadeOperations = CascadeOperation.CascadeRead)]
    public EmployeeAvatar? Avatar { get; set; }
    [ManyToOne(nameof(ClanName), nameof(Models.Clan.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Clan? Clan { get; set; }
    [ManyToOne(nameof(IconName), nameof(EmployeeIcon.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public EmployeeIcon? Icon { get; set; }
    [ManyToOne(nameof(DefaultShiftID), nameof(Shift.DefaultEmployees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Shift? DefaultShift { get; set; }

    [OneToOne(nameof(LockerID), nameof(Models.Locker.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Locker? Locker { get; set; }
    [OneToOne(nameof(LicenceNumber), nameof(Models.Licence.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Licence? Licence { get; set; }
    [OneToOne(nameof(TempTagRF_ID), nameof(Models.TempTag.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public TempTag? TempTag { get; set; }

    [ManyToMany(typeof(EmployeeVehicle), nameof(EmployeeVehicle.EmployeeID), nameof(Vehicle.Owners), CascadeOperations = CascadeOperation.All)]
    public List<Vehicle> Vehicles { get; set; }
    [ManyToMany(typeof(EmployeeShift), nameof(EmployeeShift.EmployeeID), nameof(Shift.Employees), CascadeOperations = CascadeOperation.All)]
    public List<Shift> Shifts { get; set; }
    [ManyToMany(typeof(EmployeeDepartmentLoaning), nameof(EmployeeDepartmentLoaning.EmployeeID), nameof(Models.Department.EmployeesCanLoan), CascadeOperations = CascadeOperation.All)]
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
    [OneToMany(nameof(Models.TagUse.TempTagRF_ID), nameof(Models.TagUse.TempTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<TagUse> TagUse { get; set; }
    [OneToMany(nameof(PickEvent.OperatorID), nameof(PickEvent.Operator), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickEvent> PickEvents { get; set; }
    [OneToMany(nameof(PickSession.OperatorID), nameof(PickSession.Operator), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickSession> PickSessions { get; set; }
    [OneToMany(nameof(PickDailyStats.OperatorID), nameof(PickDailyStats.Operator), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickDailyStats> PickStatistics { get; set; }

    [Ignore] public List<QACarton> QACartons { get; set; }
    [Ignore] public string FullName => $"{FirstName} {LastName}";
    [Ignore] public string ReportsToName => ReportsTo?.FullName ?? "";

    [Ignore]
    public List<ShiftRule> ShiftRules =>
        new List<ShiftRule>().Concat(SingleRules).Concat(RecurringRules).Concat(RosterRules).ToList();

    public Employee()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        DisplayName = string.Empty;
        RF_ID = string.Empty;
        PC_ID = string.Empty;
        DematicID = string.Empty;
        Location = string.Empty;
        DepartmentName = string.Empty;
        RoleName = string.Empty;
        ClanName = string.Empty;
        DefaultShiftID = string.Empty;
        PayPoint = string.Empty;
        LockerID = string.Empty;
        LicenceNumber = string.Empty;
        PhoneNumber = string.Empty;
        Email = string.Empty;
        Address = string.Empty;
        IconName = string.Empty;
        AvatarName = string.Empty;
        TempTagRF_ID = string.Empty;
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
        PickEvents = new List<PickEvent>();
        PickSessions = new List<PickSession>();
        PickStatistics = new List<PickDailyStats>();
        QACartons = new List<QACarton>();
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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}