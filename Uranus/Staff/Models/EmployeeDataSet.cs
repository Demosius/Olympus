using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

/// <summary>
/// Class for holding all potential and relevant data relating directly to Employees.
/// </summary>
public class EmployeeDataSet
{
    public Dictionary<int, Employee> Employees { get; set; }
    public Dictionary<string, Department> Departments { get; set; }
    public Dictionary<string, List<DepartmentRoster>> DepartmentRosters { get; set; }
    public Dictionary<string, Role> Roles { get; set; }
    public Dictionary<string, Clan> Clans { get; set; }
    public Dictionary<string, EmployeeIcon> EmployeeIcons { get; set; }
    public Dictionary<string, EmployeeAvatar> EmployeeAvatars { get; set; }
    public Dictionary<string, Shift> Shifts { get; set; }
    public Dictionary<string, List<Break>> BreakDict { get; set; }
    public IEnumerable<string> Locations { get; set; }
    public IEnumerable<string> PayPoints { get; set; }
    public IEnumerable<Employee> Managers { get; set; }
    public IEnumerable<TagUse> TagUses { get; set; }
    public Dictionary<int, List<ShiftRuleSingle>> SingleRuleDict { get; set; }
    public Dictionary<int, List<ShiftRuleRecurring>> RecurringRuleDict { get; set; }
    public Dictionary<int, List<ShiftRuleRoster>> RosterRuleDict { get; set; }
    public Dictionary<string, TempTag> TagDict { get; set; }

    public EmployeeDataSet()
    {
        Employees = new Dictionary<int, Employee>();
        Departments = new Dictionary<string, Department>();
        Roles = new Dictionary<string, Role>();
        DepartmentRosters = new Dictionary<string, List<DepartmentRoster>>();
        Clans = new Dictionary<string, Clan>();
        EmployeeIcons = new Dictionary<string, EmployeeIcon>();
        EmployeeAvatars = new Dictionary<string, EmployeeAvatar>();
        Shifts = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        Locations = new List<string>();
        PayPoints = new List<string>();
        Managers = new List<Employee>();
        SingleRuleDict = new Dictionary<int, List<ShiftRuleSingle>>();
        RecurringRuleDict = new Dictionary<int, List<ShiftRuleRecurring>>();
        RosterRuleDict = new Dictionary<int, List<ShiftRuleRoster>>();
        TagDict = new Dictionary<string, TempTag>();
        TagUses = new List<TagUse>();
    }

    public EmployeeDataSet(IEnumerable<Employee> employees, IEnumerable<Department> departments,
        IEnumerable<DepartmentRoster> departmentRosters, IEnumerable<Clan> clans, IEnumerable<Role> roles,
        IEnumerable<EmployeeIcon> icons, IEnumerable<EmployeeAvatar> avatars, IEnumerable<Shift> shifts,
        IEnumerable<Break> breaks, IEnumerable<ShiftRuleSingle> singleRules,
        IEnumerable<ShiftRuleRecurring> recurringRules, IEnumerable<ShiftRuleRoster> rosterRules, 
        IEnumerable<TempTag> tags, IEnumerable<TagUse> tagUses)
    {
        var empList = employees.ToList();
        Employees = empList.ToDictionary(e => e.ID, e => e);
        Departments = departments.ToDictionary(d => d.Name, d => d);
        DepartmentRosters = departmentRosters
            .GroupBy(dr => dr.DepartmentName)
            .ToDictionary(g => g.Key, g => g.ToList());
        Clans = clans.ToDictionary(c => c.Name, c => c);
        Roles = roles.ToDictionary(r => r.Name, r => r);
        EmployeeIcons = icons.ToDictionary(i => i.Name, i => i);
        EmployeeAvatars = avatars.ToDictionary(a => a.Name, a => a);
        Locations = empList.Select(e => e.Location).Distinct();
        PayPoints = empList.Select(e => e.PayPoint).Distinct();
        Managers = empList.Where(e =>
            empList.Select(emp => emp.ReportsToID).Distinct().Contains(e.ID));
        Shifts = shifts.ToDictionary(s => s.ID, s => s);
        BreakDict = breaks
            .GroupBy(b => b.ShiftID)
            .ToDictionary(g => g.Key, b => b.ToList());

        TagUses = tagUses;
        TagDict = tags.ToDictionary(t => t.RF_ID);

        SingleRuleDict = singleRules.GroupBy(r => r.EmployeeID)
            .ToDictionary(g => g.Key, g => g.ToList());
        RecurringRuleDict = recurringRules.GroupBy(r => r.EmployeeID)
            .ToDictionary(g => g.Key, g => g.ToList());
        RosterRuleDict = rosterRules.GroupBy(r => r.EmployeeID)
            .ToDictionary(g => g.Key, g => g.ToList());

        SetRelationships();
    }

    /// <summary>
    /// Using the dictionaries, assign relevant data as references to each other.
    /// </summary>
    private void SetRelationships()
    {
        SetFromTags();
        SetFromShifts();
        SetFromEmployees();
        SetFromRoles();
        SetFromClans();
        SetFromDepartments();
    }

    private void SetFromTags()
    {
        foreach (var tempTag in TagDict.Values)
            if (tempTag.EmployeeID != 0 && Employees.TryGetValue(tempTag.EmployeeID, out var employee))
                tempTag.SetEmployee(employee);
        
        foreach (var tagUse in TagUses)
        {
            if (TagDict.TryGetValue(tagUse.TempTagRF_ID, out var tag))
            {
                tag.TagUse.Add(tagUse);
                tagUse.TempTag = tag;
            }

            if (!Employees.TryGetValue(tagUse.EmployeeID, out var employee)) continue;

            employee.TagUse.Add(tagUse);
            tagUse.Employee = employee;
        }
    }

    private void SetFromShifts()
    {
        foreach (var (_, shift) in Shifts)
        {
            if (BreakDict.TryGetValue(shift.ID, out var breaks))
            {
                shift.Breaks = breaks;
                foreach (var @break in breaks)
                    @break.Shift = shift;
            }

            if (!Departments.TryGetValue(shift.DepartmentName, out var department)) continue;

            department.Shifts.Add(shift);
            shift.Department = department;
        }
    }

    private void SetFromDepartments()
    {
        foreach (var (_, department) in Departments)
        {
            if (Departments.TryGetValue(department.OverDepartmentName, out var overDepartment))
            {
                department.OverDepartment = overDepartment;
                overDepartment.SubDepartments.Add(department);
            }

            if (DepartmentRosters.TryGetValue(department.Name, out var rosters))
            {
                department.DepartmentRosters = rosters;
                foreach (var departmentRoster in rosters)
                {
                    departmentRoster.Department = department;
                }
            }

            if (Employees.TryGetValue(department.HeadID, out var employee))
                department.Head = employee;
        }
    }

    private void SetFromEmployees()
    {
        foreach (var (_, employee) in Employees)
        {
            if (Departments.TryGetValue(employee.DepartmentName, out var department))
            {
                employee.Department = department;
                department.Employees.Add(employee);
            }

            if (Roles.TryGetValue(employee.RoleName, out var role))
            {
                employee.Role = role;
                role.Employees.Add(employee);
            }

            if (Employees.TryGetValue(employee.ReportsToID, out var manager))
            {
                employee.ReportsTo = manager;
                manager.Reports.Add(employee);
            }

            if (Clans.TryGetValue(employee.ClanName, out var clan))
            {
                employee.Clan = clan;
                clan.Employees.Add(employee);
            }

            if (EmployeeIcons.TryGetValue(employee.IconName, out var icon))
            {
                employee.Icon = icon;
                icon.Employees.Add(employee);
            }

            if (EmployeeAvatars.TryGetValue(employee.AvatarName, out var avatar))
            {
                employee.Avatar = avatar;
                avatar.Employees.Add(employee);
            }

            if (Shifts.TryGetValue(employee.DefaultShiftID, out var shift))
            {
                employee.DefaultShift = shift;
                shift.DefaultEmployees.Add(employee);
            }

            if (SingleRuleDict.TryGetValue(employee.ID, out var singleRules))
            {
                employee.SingleRules = singleRules;
                foreach (var rule in singleRules) rule.Employee = employee;
            }

            if (RecurringRuleDict.TryGetValue(employee.ID, out var recurringRules))
            {
                employee.RecurringRules = recurringRules;
                foreach (var rule in recurringRules) rule.Employee = employee;
            }

            if (RosterRuleDict.TryGetValue(employee.ID, out var rosterRules))
            {
                employee.RosterRules = rosterRules;
                foreach (var rule in rosterRules) rule.Employee = employee;
            }
        }
    }

    private void SetFromRoles()
    {
        foreach (var (_, role) in Roles)
        {
            if (Roles.TryGetValue(role.ReportsToRoleName, out var overRole))
            {
                role.ReportsToRole = overRole;
                overRole.Reports.Add(role);
            }

            if (!Departments.TryGetValue(role.DepartmentName, out var department)) continue;

            role.Department = department;
            department.Roles.Add(role);
        }
    }

    private void SetFromClans()
    {
        foreach (var (_, clan) in Clans)
        {
            if (Departments.TryGetValue(clan.DepartmentName, out var department))
            {
                clan.Department = department;
                department.Clans.Add(clan);
            }

            if (Employees.TryGetValue(clan.LeaderID, out var leader))
                clan.Leader = leader;
        }
    }

    /// <summary>
    /// Given an employee, returns all employees that report to them, recursively.
    /// </summary>
    /// <param name="employeeID">The ID of the leading Employee.</param>
    /// <returns></returns>
    public IEnumerable<Employee> GetReportsByRole(int employeeID)
    {
        if (!Employees.TryGetValue(employeeID, out var employee)) return new List<Employee>();

        if (!Roles.TryGetValue(employee.RoleName, out var headRole)) return new List<Employee>();

        Dictionary<string, Role> roleDict = new();

        GetRoleReports(headRole, ref roleDict);

        return roleDict.Values.SelectMany(r => r.Employees);
    }

    /// <summary>
    /// Recursively get reporting roles based on the given 'head' role.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="returnDict"></param>
    private static void GetRoleReports(Role role, ref Dictionary<string, Role> returnDict)
    {
        foreach (var report in role.Reports)
        {
            if (returnDict.ContainsKey(report.Name)) continue;
            returnDict.Add(report.Name, report);
            GetRoleReports(report, ref returnDict);
        }
    }

    public void AddDepartment(ref Department newDepartment)
    {
        if (Departments.ContainsKey(newDepartment.Name))
            throw new Exception("Attempt to add department to data set that already exists in name.");

        foreach (var (_, department) in Departments)
        {
            if (newDepartment.OverDepartmentName == department.Name)
            {
                newDepartment.OverDepartment = department;
                department.SubDepartments.Add(newDepartment);
            }

            if (department.OverDepartmentName != newDepartment.Name) continue;

            department.OverDepartment = newDepartment;
            newDepartment.SubDepartments.Add(department);
        }

        foreach (var (_, clan) in Clans)
        {
            if (clan.DepartmentName != newDepartment.Name) continue;

            clan.Department = newDepartment;
            newDepartment.Clans.Add(clan);
        }

        foreach (var (_, role) in Roles)
        {
            if (role.DepartmentName != newDepartment.Name) continue;

            role.Department = newDepartment;
            newDepartment.Roles.Add(role);
        }

        foreach (var (_, employee) in Employees)
        {
            if (employee.ID == newDepartment.HeadID) newDepartment.Head = employee;

            if (employee.DepartmentName != newDepartment.Name) continue;

            employee.Department = newDepartment;
            newDepartment.Employees.Add(employee);
        }

        Departments.Add(newDepartment.Name, newDepartment);
    }

    public void AddRole(ref Role newRole)
    {
        if (Roles.ContainsKey(newRole.Name))
            throw new Exception("Attempt to add role to data set that already exists in name.");

        if (Departments.TryGetValue(newRole.DepartmentName, out var department))
        {
            newRole.Department = department;
            department.Roles.Add(newRole);
        }

        foreach (var (_, employee) in Employees)
        {
            if (employee.RoleName != newRole.Name) continue;

            employee.Role = newRole;
            newRole.Employees.Add(employee);
        }

        foreach (var (_, role) in Roles)
        {
            if (role.ReportsToRoleName == newRole.Name)
            {
                role.ReportsToRole = newRole;
                newRole.Reports.Add(role);
            }

            if (newRole.ReportsToRoleName != role.Name) continue;

            newRole.ReportsToRole = role;
            role.Reports.Add(newRole);
        }

        Roles.Add(newRole.Name, newRole);
    }

    public void AddClan(ref Clan newClan)
    {
        if (Clans.ContainsKey(newClan.Name))
            throw new Exception("Attempt to add clan to data set that already exists in name.");

        if (Departments.TryGetValue(newClan.DepartmentName, out var department))
        {
            newClan.Department = department;
            department.Clans.Add(newClan);
        }

        foreach (var (_, employee) in Employees)
        {
            if (newClan.LeaderID == employee.ID) newClan.Leader = employee;

            if (employee.ClanName != newClan.Name) continue;

            employee.Clan = newClan;
            newClan.Employees.Add(employee);
        }

        Clans.Add(newClan.Name, newClan);
    }

    public void AddEmployee(Employee newEmployee)
    {
        if (Employees.ContainsKey(newEmployee.ID))
            throw new Exception("Attempt to add employee to data set that already has existing employee ID.");

        if (Departments.TryGetValue(newEmployee.DepartmentName, out var department))
        {
            newEmployee.Department = department;
            department.Employees.Add(newEmployee);
        }

        if (Clans.TryGetValue(newEmployee.ClanName, out var clan))
        {
            newEmployee.Clan = clan;
            clan.Employees.Add(newEmployee);
        }

        if (Roles.TryGetValue(newEmployee.RoleName, out var role))
        {
            newEmployee.Role = role;
            role.Employees.Add(newEmployee);
        }

        if (Employees.TryGetValue(newEmployee.ReportsToID, out var manager))
        {
            newEmployee.ReportsTo = manager;
            manager.Reports.Add(newEmployee);
        }

        foreach (var employee in Employees.Values.Where(employee => employee.ReportsToID == newEmployee.ID))
        {
            employee.ReportsTo = newEmployee;
            newEmployee.Reports.Add(employee);
        }

        foreach (var tagUse in TagUses.Where(u => u.EmployeeID == newEmployee.ID))
        {
            newEmployee.TagUse.Add(tagUse);
            tagUse.Employee = newEmployee;
        }

        if (TagDict.TryGetValue(newEmployee.TempTagRF_ID, out var tempTag))
        {
            newEmployee.TempTag = tempTag;
            tempTag.Employee = newEmployee;
        }

        Employees.Add(newEmployee.ID, newEmployee);
    }

    /// <summary>
    /// Pulls all relevant sub departments according to the given department name.
    /// Will also fill relevant department data, such as shifts.
    /// </summary>
    /// <param name="departmentName"></param>
    /// <returns></returns>
    public IEnumerable<Department> SubDepartments(string departmentName)
    {
        // Get the department matching the given name.
        if (!Departments.TryGetValue(departmentName, out var department)) return new List<Department>();

        var deptDict = new Dictionary<string, Department>();

        GetSubDepartments(department, ref deptDict);

        deptDict.Add(departmentName, department);

        return deptDict.Values;
    }

    /// <summary>
    /// Gets a dict of departments that are all  recursively under the given (potential) head department.
    /// </summary>
    /// <param name="department"></param>
    /// <param name="returnDict"></param>
    private static void GetSubDepartments(Department department, ref Dictionary<string, Department> returnDict)
    {
        foreach (var sub in department.SubDepartments)
        {
            if (returnDict.ContainsKey(sub.Name)) continue;
            returnDict.Add(sub.Name, sub);
            GetSubDepartments(sub, ref returnDict);
        }
    }
}