using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Interface;
using Pantheon.Views;
using Pantheon.Views.PopUp.Employees;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Employees;

public class EmployeeVM : INotifyPropertyChanged, IPayPoints
{
    public Employee Employee { get; set; }
    public Charon Charon { get; set; }
    public Helios Helios { get; set; }

    #region INotifyPropertyChanged Members

    public int ID => Employee.ID;
    public string RoleName => Employee.RoleName;
    public string ReportToName => ReportsTo is null ? "" : $"{ReportsTo.FullName} - {ReportsTo.RoleName}";

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
        set
        {
            Employee.ReportsTo = value; OnPropertyChanged();
            ReportsToID = Employee.ReportsTo?.ID ?? 0;
        }
    }

    public int ReportsToID
    {
        get => Employee.ReportsToID;
        set { Employee.ReportsToID = value; OnPropertyChanged(); }
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

    #region Commands

    public SaveEmployeeCommand SaveEmployeeCommand { get; set; }
    public LaunchIconiferCommand LaunchIconiferCommand { get; set; }
    public SelectClanCommand SelectClanCommand { get; set; }
    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public SelectLocationCommand SelectLocationCommand { get; set; }
    public SelectPayPointCommand SelectPayPointCommand { get; set; }
    public SelectRoleCommand SelectRoleCommand { get; set; }

    #endregion

    public EmployeeVM(Employee employee, Charon charon, Helios helios)
    {
        Employee = employee;

        Charon = charon;
        Helios = helios;

        SaveEmployeeCommand = new SaveEmployeeCommand(this);
        LaunchIconiferCommand = new LaunchIconiferCommand(this);
        SelectClanCommand = new SelectClanCommand(this);
        SelectDepartmentCommand = new SelectDepartmentCommand(this);
        SelectLocationCommand = new SelectLocationCommand(this);
        SelectPayPointCommand = new SelectPayPointCommand(this);
        SelectRoleCommand = new SelectRoleCommand(this);
    }

    public void SetDataFromObjects() => Employee.SetDataFromObjects();

    public void Delete() => Employee.Delete();

    public void SaveEmployee()
    {
        // When changing the role, only update the RoleName so that they can be compared.
        // Update the Role Object once this is confirmed.
        if ((Role is null || Role.Name != RoleName) && !ConfirmUnEditableChange()) return;

        SetDataFromObjects();

        if (Helios.StaffUpdater.Employee(Employee) > 0)
            MessageBox.Show($"Successfully saved changes to {FullName}.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void LaunchIconifer()
    {
        var iconifer = new IconSelectionWindow(this);
        if (iconifer.ShowDialog() != true) return;
        Icon = iconifer.VM.SelectedIcon;
    }

    public void SelectLocation()
    {
        // TODO: Update to Location Selection Window
        var input = new InputWindow("Enter new location:", "New Location");
        if (input.ShowDialog() != true) return;

        Location = input.VM.Input;
    }

    public void SelectDepartment()
    {
        var departmentCreator = new DepartmentCreationWindow(this);
        if (departmentCreator.ShowDialog() != true) return;

        Department = departmentCreator.VM.Department;
    }

    public void SelectRole()
    {
        var roleCreator = new RoleCreationWindow(Helios, Charon);
        if (roleCreator.ShowDialog() != true) return;

        Role = roleCreator.VM.Role;
    }

    public void SelectClan()
    {
        var clanCreator = new ClanCreationWindow(Helios, Charon);
        if (clanCreator.ShowDialog() != true) return;

        Clan = clanCreator.VM.Clan;
    }

    public void SelectPayPoint()
    {
        var payPointSelector = new PayPointSelectionWindow(Helios, Charon);
        if (payPointSelector.ShowDialog() != true) return;

        var pp = payPointSelector.VM.SelectedPayPoint;
        if (pp is null) return;

        PayPoint = pp.Name;
    }

    public void SelectManager()
    {
        var managerSelectionWindow = new ManagerSelectionWindow(this);
        if (managerSelectionWindow.ShowDialog() != true) return;
        
        var manager = managerSelectionWindow.VM.SelectedManager?.Employee;
        if (manager is null) return;

        ReportsTo = manager;
    }

    /// <summary>
    /// Assuming the user is about to adjust the employee in such a way that removes that employee from the user's permissions to edit further, make sure confirmation is attained.
    /// </summary>
    /// <returns></returns>
    private bool ConfirmUnEditableChange()
    {
        var newRole = Helios.StaffReader.Role(RoleName);

        if (newRole is null) return false;

        if (Charon.CanUpdateEmployee(newRole)) return true;

        if (MessageBox.Show(
                "Changing this employee's Role will mean you will not be able to edit them in the future.\n\nDo you want to continue?",
                "Confirm New Role", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) !=
            MessageBoxResult.Yes) return false;

        Role = newRole;
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}