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
    public IEnumerable<string> Locations { get; set; }
    public IEnumerable<string> PayPoints { get; set; }
    public IEnumerable<Employee> Managers { get; set; }

    public EmployeeDataSet(IEnumerable<Employee> employees, IEnumerable<Department> departments, IEnumerable<Clan> clans, IEnumerable<Role> roles)
    {
        var empList = employees.ToList();
        Employees = empList.ToDictionary(e => e.ID, e => e);
        Departments = departments.ToDictionary(d => d.Name, d => d);
        Clans = clans.ToDictionary(c => c.Name, c => c);
        Roles = roles.ToDictionary(r => r.Name, r => r);
        Locations = empList.Select(e => e.Location).Distinct();
        PayPoints = empList.Select(e => e.PayPoint).Distinct();
        Managers = empList.Where(e =>
            empList.Select(emp => emp.ReportsToID).Distinct().Contains(e.ID));
        SetRelationships();
    }

    public EmployeeDataSet(Dictionary<int, Employee> employees, Dictionary<string, Department> departments, Dictionary<string, Clan> clans, Dictionary<string, Role> roles)
    {
        Employees = employees;
        Departments = departments;
        Clans = clans;
        Roles = roles;
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
}