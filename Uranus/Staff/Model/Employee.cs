﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

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

public class Employee
{
    [PrimaryKey] public int ID { get; set; } // Employee number (e.g. 60853)
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [NotNull] public string DisplayName { get; set; }
    public decimal? PayRate { get; set; }
    public string RF_ID { get; set; }
    public string PC_ID { get; set; }
    public string Location { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Role))] public string RoleName { get; set; }    // Also known as Job Classification.
    [ForeignKey(typeof(Employee))] public int ReportsToID { get; set; }    // Specific Employee this employee reports to, bypassing Role and RoleReports.
    [ForeignKey(typeof(Clan))] public string ClanName { get; set; }
    public string PayPoint { get; set; }
    public EEmploymentType EmploymentType { get; set; }
    [ForeignKey(typeof(Locker))] public string LockerID { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    [ForeignKey(typeof(EmployeeIcon))] public string IconName { get; set; }
    [ForeignKey(typeof(EmployeeAvatar))] public string AvatarName { get; set; }
    [ForeignKey(typeof(Licence))] public int LicenceNumber { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }
    [ManyToOne(nameof(RoleName), nameof(Model.Role.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Role? Role { get; set; }
    [ManyToOne(nameof(ReportsToID), nameof(Reports), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? ReportsTo { get; set; }
    [ManyToOne(nameof(AvatarName), nameof(EmployeeAvatar.Employees), CascadeOperations = CascadeOperation.CascadeRead)]
    public EmployeeAvatar? Avatar { get; set; }
    [ManyToOne(nameof(ClanName), nameof(Model.Clan.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Clan? Clan { get; set; }
    [ManyToOne(nameof(IconName), nameof(EmployeeIcon.Employees), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public EmployeeIcon? Icon { get; set; }

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

    [OneToMany(nameof(EmployeeInductionReference.EmployeeID), nameof(EmployeeInductionReference.Employee), CascadeOperations = CascadeOperation.All)]
    public List<EmployeeInductionReference> InductionReferences { get; set; }
    [OneToMany(nameof(ShiftRule.EmployeeID), nameof(ShiftRule.Employee), CascadeOperations = CascadeOperation.All)]
    public List<ShiftRule> Rules { get; set; }
    [OneToMany(nameof(ReportsToID), nameof(ReportsTo), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Reports { get; set; }
    [OneToMany(nameof(ShiftEntry.EmployeeID), nameof(ShiftEntry.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<ShiftEntry> ShiftEntries { get; set; }
    [OneToMany(nameof(Roster.EmployeeID), nameof(Roster.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(ClockEvent.EmployeeID), nameof(ClockEvent.Employee), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<ClockEvent> ClockEvents { get; set; }
    [OneToMany(nameof(Model.TagUse.TempTagRFID), nameof(Model.TagUse.TempTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<TagUse> TagUse { get; set; }

    [Ignore] public string FullName => $"{FirstName} {LastName}";
    [Ignore] public string ReportsToName => ReportsTo?.FullName ?? "";

    public Employee()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        DisplayName = string.Empty;
        RF_ID = string.Empty;
        PC_ID = string.Empty;
        Location = string.Empty;
        DepartmentName = string.Empty;
        RoleName = string.Empty;
        ClanName = string.Empty;
        PayPoint = string.Empty;
        LockerID = string.Empty;
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
        Rules = new List<ShiftRule>();
        Reports = new List<Employee>();
        ShiftEntries = new List<ShiftEntry>();
        Rosters = new List<Roster>();
        ClockEvents = new List<ClockEvent>();
        TagUse = new List<TagUse>();
    }

    public Employee(int id) : this()
    {
        ID = id;
    }

    public Employee(int id, string firstName, string lastName, string displayName, decimal? payRate,
        string rfID, string pcID, string location, string departmentName, string roleName,
        int reportsToID, string clanName, string payPoint, EEmploymentType employmentType,
        string lockerID, string phoneNumber, string email, string address, string iconName,
        string avatarName, int licenceNumber, Department? department, Role? role, Employee? reportsTo,
        EmployeeAvatar? avatar, Clan? clan, EmployeeIcon? icon, Locker? locker, Licence? licence,
        List<Vehicle> vehicles, List<Shift> shifts, List<Department> departmentsCanWorkIn,
        List<Project> projects, List<EmployeeInductionReference> inductionReferences, List<ShiftRule> rules,
        List<Employee> reports, List<ShiftEntry> shiftEntries, List<Roster> rosters, List<ClockEvent> clockEvents,
        List<TagUse> tagUse)
    {
        ID = id;
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        PayRate = payRate;
        RF_ID = rfID;
        PC_ID = pcID;
        Location = location;
        DepartmentName = departmentName;
        RoleName = roleName;
        ReportsToID = reportsToID;
        ClanName = clanName;
        PayPoint = payPoint;
        EmploymentType = employmentType;
        LockerID = lockerID;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        IconName = iconName;
        AvatarName = avatarName;
        LicenceNumber = licenceNumber;
        Department = department;
        Role = role;
        ReportsTo = reportsTo;
        Avatar = avatar;
        Clan = clan;
        Icon = icon;
        Locker = locker;
        Licence = licence;
        Vehicles = vehicles;
        Shifts = shifts;
        DepartmentsCanWorkIn = departmentsCanWorkIn;
        Projects = projects;
        InductionReferences = inductionReferences;
        Rules = rules;
        Reports = reports;
        ShiftEntries = shiftEntries;
        Rosters = rosters;
        ClockEvents = clockEvents;
        TagUse = tagUse;
    }

    public void AssignRole(Role role)
    {
        RoleName = role.Name;
        Role = role;
        role.Employees.Add(this);
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

}