using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Employees;

public class EmployeeVM : INotifyPropertyChanged
{
    public Employee Employee { get; set; }

    #region INotifyPropertyChanged Members

    public int ID => Employee.ID;
    public string RoleName => Employee.RoleName;

    public string DefaultShiftID
    {
        get => Employee.DefaultShiftID;
        set => Employee.DefaultShiftID = value;
    }

    public List<ShiftRuleRoster> RosterRules
    {
        get => Employee.RosterRules;
        set => Employee.RosterRules = value;
    }

    public List<ShiftRuleSingle> SingleRules
    {
        get => Employee.SingleRules;
        set => Employee.SingleRules = value;
    }

    public List<ShiftRuleRecurring> RecurringRules
    {
        get => Employee.RecurringRules;
        set => Employee.RecurringRules = value;
    }

    public Shift? DefaultShift
    {
        get => Employee.DefaultShift;
        set => Employee.DefaultShift = value;
    }

    public string DepartmentName
    {
        get => Employee.DepartmentName;
        set => Employee.DepartmentName = value;
    }

    public string FirstName
    {
        get => Employee.FirstName;
        set
        {
            Employee.FirstName = value;
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged();
        }
    }

    public string LastName
    {
        get => Employee.LastName;
        set
        {
            Employee.LastName = value;
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged();
        }
    }

    public string DisplayName
    {
        get => Employee.DisplayName;
        set { Employee.DisplayName = value; OnPropertyChanged(); }
    }

    public string FullName => Employee.FullName;

    public string PC_ID
    {
        get => Employee.PC_ID;
        set { Employee.PC_ID = value; OnPropertyChanged(); }
    }

    public string RF_ID
    {
        get => Employee.RF_ID;
        set { Employee.RF_ID = value; OnPropertyChanged(); }
    }

    public string Location
    {
        get => Employee.Location;
        set { Employee.Location = value; OnPropertyChanged(nameof(Location)); }
    }

    public string PayPoint
    {
        get => Employee.PayPoint;
        set { Employee.PayPoint = value; OnPropertyChanged(); }
    }

    public EEmploymentType EmploymentType
    {
        get => Employee.EmploymentType;
        set { Employee.EmploymentType = value; OnPropertyChanged(); }
    }

    public Employee? ReportsTo
    {
        get => Employee.ReportsTo;
        set { Employee.ReportsTo = value; OnPropertyChanged(); }
    }

    public Department? Department
    {
        get => Employee.Department;
        set { Employee.Department = value; OnPropertyChanged(nameof(Department)); }
    }

    public Role? Role
    {
        get => Employee.Role;
        set { Employee.Role = value; OnPropertyChanged(nameof(Role)); }
    }

    public string PhoneNumber
    {
        get => Employee.PhoneNumber;
        set { Employee.PhoneNumber = value; OnPropertyChanged(nameof(PhoneNumber)); }
    }

    public string Email
    {
        get => Employee.Email;
        set { Employee.Email = value; OnPropertyChanged(nameof(Email)); }
    }

    public string Address
    {
        get => Employee.Address;
        set { Employee.Address = value; OnPropertyChanged(nameof(Address)); }
    }

    public decimal? PayRate
    {
        get => Employee.PayRate;
        set { Employee.PayRate = value; OnPropertyChanged(nameof(PayRate)); }
    }

    public EmployeeAvatar? Avatar
    {
        get => Employee.Avatar;
        set { Employee.Avatar = value; OnPropertyChanged(nameof(Avatar)); }
    }

    public Clan? Clan
    {
        get => Employee.Clan;
        set { Employee.Clan = value; OnPropertyChanged(nameof(Clan)); }
    }

    public EmployeeIcon? Icon
    {
        get => Employee.Icon; set
        { Employee.Icon = value; OnPropertyChanged(nameof(Icon)); }
    }

    public bool IsUser
    {
        get => Employee.IsUser;
        set { Employee.IsUser = value; OnPropertyChanged(); }
    }

    #endregion

    public EmployeeVM(Employee employee)
    {
        Employee = employee;
    }

    public void SetDataFromObjects() => Employee.SetDataFromObjects();

    public void Delete() => Employee.Delete();

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}