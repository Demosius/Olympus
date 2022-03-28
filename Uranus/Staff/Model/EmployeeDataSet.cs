using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Model;

/// <summary>
/// Class for holding all potential and relevant data relating directly to Employees.
/// </summary>
public class EmployeeDataSet
{
    public Dictionary<int, Employee> Employees { get; set; }
    public Dictionary<string, Department> Departments { get; set; }
    public Dictionary<string, Role> Roles { get; set; }
    public Dictionary<string, Clan> Clans { get; set; }
    public Dictionary<string, EmployeeIcon> EmployeeIcons { get; set; }
    public Dictionary<string, EmployeeAvatar> EmployeeAvatars { get; set; }
    public IEnumerable<string> Locations { get; set; }
    public IEnumerable<string> PayPoints { get; set; }
    public IEnumerable<Employee> Managers { get; set; }

    public EmployeeDataSet(IEnumerable<Employee> employees, IEnumerable<Department> departments, IEnumerable<Clan> clans,
        IEnumerable<Role> roles, IEnumerable<EmployeeIcon> icons, IEnumerable<EmployeeAvatar> avatars)
    {
        var empList = employees.ToList();
        Employees = empList.ToDictionary(e => e.ID, e => e);
        Departments = departments.ToDictionary(d => d.Name, d => d);
        Clans = clans.ToDictionary(c => c.Name, c => c);
        Roles = roles.ToDictionary(r => r.Name, r => r);
        EmployeeIcons = icons.ToDictionary(i => i.Name, i => i);
        EmployeeAvatars = avatars.ToDictionary(a => a.Name, a => a);
        Locations = empList.Select(e => e.Location).Distinct();
        PayPoints = empList.Select(e => e.PayPoint).Distinct();
        Managers = empList.Where(e =>
            empList.Select(emp => emp.ReportsToID).Distinct().Contains(e.ID));
        SetRelationships();
    }

    public EmployeeDataSet(Dictionary<int, Employee> employees, Dictionary<string, Department> departments, Dictionary<string,
        Clan> clans, Dictionary<string, Role> roles, Dictionary<string, EmployeeIcon> employeeIcons, Dictionary<string, EmployeeAvatar> employeeAvatars)
    {
        Employees = employees;
        Departments = departments;
        Clans = clans;
        Roles = roles;
        EmployeeIcons = employeeIcons;
        EmployeeAvatars = employeeAvatars;
        Locations = employees.Select(e => e.Value.Location).Distinct();
        PayPoints = employees.Select(e => e.Value.PayPoint).Distinct();
        Managers = Employees.Values.Where(e =>
            employees.Select(emp => emp.Value.ReportsToID).Distinct().Contains(e.ID));
        SetRelationships();
    }

    /// <summary>
    /// Using the dictionaries, assign relevant data as references to each other.
    /// </summary>
    public void SetRelationships()
    {
        SetFromEmployees();
        SetFromRoles();
        SetFromClans();
        SetFromDepartments();
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

        Employees.Add(newEmployee.ID, newEmployee);
    }
}