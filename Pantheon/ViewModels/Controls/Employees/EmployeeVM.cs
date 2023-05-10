using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Commands.TempTags;
using Pantheon.ViewModels.Interfaces;
using Pantheon.Views.PopUp.Employees;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Employees;

public class EmployeeVM : INotifyPropertyChanged, ILocations, IDepartments, IRoles, IManagers, IClans, IPayPoints, ITempTags
{
    public Employee Employee { get; set; }
    public Charon Charon { get; set; }
    public Helios Helios { get; set; }

    public bool SensitiveVisibility { get; }
    public bool VerySensitiveVisibility { get; }

    public bool CanUnassign => Charon.CanCreateEmployee() && TempTag is not null;
    public bool CanAssign => Charon.CanCreateEmployee() && TempTag is null;

    #region INotifyPropertyChanged Members

    public int ID => Employee.ID;
    public string RoleName => Employee.RoleName;
    public string ReportsToName => ReportsTo is null ? "" : $"{ReportsTo.FullName} - {ReportsTo.RoleName}";

    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            OnPropertyChanged();
        }
    }

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
        set { Employee.Location = value; OnPropertyChanged(); }
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
            OnPropertyChanged(nameof(ReportsToName));
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
        set { Employee.Department = value; OnPropertyChanged(); }
    }

    public Role? Role
    {
        get => Employee.Role;
        set { Employee.Role = value; OnPropertyChanged(); }
    }

    public string PhoneNumber
    {
        get => Employee.PhoneNumber;
        set { Employee.PhoneNumber = value; OnPropertyChanged(); }
    }

    public string Email
    {
        get => Employee.Email;
        set { Employee.Email = value; OnPropertyChanged(); }
    }

    public string Address
    {
        get => Employee.Address;
        set { Employee.Address = value; OnPropertyChanged(); }
    }

    public string PayRate
    {
        get => Employee.PayRate?.ToString("C") ?? "";
        set
        {
            Employee.PayRate = decimal.TryParse(value, out var d) ? d : null;
            OnPropertyChanged();
        }
    }

    public EmployeeAvatar? Avatar
    {
        get => Employee.Avatar;
        set { Employee.Avatar = value; OnPropertyChanged(); }
    }

    public Clan? Clan
    {
        get => Employee.Clan;
        set { Employee.Clan = value; OnPropertyChanged(); }
    }

    public EmployeeIcon? Icon
    {
        get => Employee.Icon;
        set
        {
            Employee.Icon = value;
            OnPropertyChanged();
            IconUri = new Uri(value?.FullPath ?? "", UriKind.RelativeOrAbsolute);
        }
    }

    private Uri? iconUri;
    public Uri? IconUri
    {
        get => iconUri;
        set
        {
            iconUri = value;
            OnPropertyChanged();
        }
    }

    public bool IsUser
    {
        get => Employee.IsUser;
        set { Employee.IsUser = value; OnPropertyChanged(); }
    }

    public TempTag? TempTag
    {
        get => Employee.TempTag;
        set { Employee.TempTag = value; OnPropertyChanged(); }
    }

    public string TempTagRF_ID
    {
        get => Employee.TempTagRF_ID;
        set { Employee.TempTagRF_ID = value; OnPropertyChanged(); }
    }

    public bool HasTempTag => TempTag is not null;
    public bool HasNoTempTag => !HasTempTag;

    #endregion

    #region Commands

    public SaveEmployeeCommand SaveEmployeeCommand { get; set; }
    public LaunchIconiferCommand LaunchIconiferCommand { get; set; }
    public LaunchAvatarSelectorCommand LaunchAvatarSelectorCommand { get; set; }
    public SelectClanCommand SelectClanCommand { get; set; }
    public ClearClanCommand ClearClanCommand { get; set; }
    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public ClearDepartmentCommand ClearDepartmentCommand { get; set; }
    public SelectLocationCommand SelectLocationCommand { get; set; }
    public ClearLocationCommand ClearLocationCommand { get; set; }
    public SelectPayPointCommand SelectPayPointCommand { get; set; }
    public ClearPayPointCommand ClearPayPointCommand { get; set; }
    public SelectRoleCommand SelectRoleCommand { get; set; }
    public ClearRoleCommand ClearRoleCommand { get; set; }
    public SelectManagerCommand SelectManagerCommand { get; set; }
    public ClearManagerCommand ClearManagerCommand { get; set; }
    public LaunchEmployeeShiftWindowCommand LaunchEmployeeShiftWindowCommand { get; set; }
    public SelectTempTagCommand SelectTempTagCommand { get; set; }
    public UnassignTempTagCommand UnassignTempTagCommand { get; set; }
    public AssignTempTagCommand AssignTempTagCommand { get; set; }

    #endregion

    public EmployeeVM(Employee employee, Charon charon, Helios helios)
    {
        Employee = employee;

        Charon = charon;
        Helios = helios;

        SensitiveVisibility = Charon.CanReadEmployeeSensitive(Employee);
        VerySensitiveVisibility = Charon.CanReadEmployeeVerySensitive(Employee);

        IconUri = Icon is null ? null : new Uri(Icon.FullPath, UriKind.RelativeOrAbsolute);

        SaveEmployeeCommand = new SaveEmployeeCommand(this);
        LaunchIconiferCommand = new LaunchIconiferCommand(this);
        LaunchAvatarSelectorCommand = new LaunchAvatarSelectorCommand(this);
        SelectClanCommand = new SelectClanCommand(this);
        SelectDepartmentCommand = new SelectDepartmentCommand(this);
        SelectLocationCommand = new SelectLocationCommand(this);
        SelectPayPointCommand = new SelectPayPointCommand(this);
        SelectRoleCommand = new SelectRoleCommand(this);
        SelectManagerCommand = new SelectManagerCommand(this);
        LaunchEmployeeShiftWindowCommand = new LaunchEmployeeShiftWindowCommand(this);
        ClearLocationCommand = new ClearLocationCommand(this);
        ClearDepartmentCommand = new ClearDepartmentCommand(this);
        ClearRoleCommand = new ClearRoleCommand(this);
        ClearManagerCommand = new ClearManagerCommand(this);
        ClearClanCommand = new ClearClanCommand(this);
        ClearPayPointCommand = new ClearPayPointCommand(this);
        SelectTempTagCommand = new SelectTempTagCommand(this);
        UnassignTempTagCommand = new UnassignTempTagCommand(this);
        AssignTempTagCommand = new AssignTempTagCommand(this);
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

    public void LaunchAvatarSelector()
    {
        var avatarSelector = new AvatarSelectionWindow(this);
        if (avatarSelector.ShowDialog() != true) return;
        Avatar = avatarSelector.VM.SelectedAvatar;
    }

    public void SelectLocation()
    {
        var locationSelector = new LocationSelectionWindow(Helios, Charon);
        if (locationSelector.ShowDialog() != true) return;

        var loc = locationSelector.VM.SelectedLocation;
        if (loc is null) return;

        Location = loc.Name;
    }

    public void ClearLocation()
    {
        Location = string.Empty;
    }

    public void SelectDepartment()
    {
        var departmentSelector = new DepartmentSelectionWindow(Helios, Charon);
        if (departmentSelector.ShowDialog() != true) return;

        Department = departmentSelector.VM.SelectedDepartment;
    }

    public void ClearDepartment()
    {
        Department = null;
    }

    public void SelectRole()
    {
        var roleSelector = new RoleSelectionWindow(Helios, Charon, DepartmentName);
        if (roleSelector.ShowDialog() != true) return;

        Role = roleSelector.VM.SelectedRole;
    }

    public void ClearRole()
    {
        Role = null;
    }

    public void SelectClan()
    {
        var clanSelector = new ClanSelectionWindow(Helios, Charon);
        if (clanSelector.ShowDialog() != true) return;

        Clan = clanSelector.VM.SelectedClan;
    }

    public void ClearClan()
    {
        Clan = null;
    }

    public void SelectPayPoint()
    {
        var payPointSelector = new PayPointSelectionWindow(Helios, Charon);
        if (payPointSelector.ShowDialog() != true) return;

        var pp = payPointSelector.VM.SelectedPayPoint;
        if (pp is null) return;

        PayPoint = pp.Name;
    }

    public void ClearPayPoint()
    {
        PayPoint = string.Empty;
    }

    public void SelectManager()
    {
        var fullEmployeeList = AsyncHelper.RunSync(() => Helios.StaffReader.EmployeesAsync())
            .OrderBy(e => e.FullName)
            .Select(e => new EmployeeVM(e, Charon, Helios))
            .ToList();

        var managerSelector = new EmployeeSelectionWindow(fullEmployeeList, true, DepartmentName);
        if (managerSelector.ShowDialog() != true) return;

        var manager = managerSelector.SelectedEmployee?.Employee;
        if (manager is null) return;

        ReportsTo = manager;
    }

    public void ClearManager()
    {
        ReportsTo = null;
    }

    public async Task SelectTempTagAsync()
    {
        var tagSelector = new TempTagSelectionWindow(Helios, Charon);
        if (tagSelector.ShowDialog() != true) return;

        var tag = tagSelector.TempTag;
        if (tag is null) return;

        await Helios.StaffUpdater.AssignTempTagAsync(tag, Employee);
        OnPropertyChanged(nameof(TempTag));
        OnPropertyChanged(nameof(TempTagRF_ID));
        OnPropertyChanged(nameof(HasNoTempTag));
        OnPropertyChanged(nameof(HasTempTag));

        // TODO: Adjust temptags for all in dataset ??

    }

    public void UnassignTempTag()
    {
        if (TempTag is null) return;
        Helios.StaffUpdater.UnassignTempTag(TempTag);
        OnPropertyChanged(nameof(HasTempTag));
        OnPropertyChanged(nameof(HasNoTempTag));
        OnPropertyChanged(nameof(TempTag));
        OnPropertyChanged(nameof(TempTagRF_ID));
    }

    public async Task AssignTempTagAsync() => await SelectTempTagAsync();

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

    public void LaunchEmployeeShiftWindow()
    {
        var shiftWindow = new EmployeeShiftWindow(this);

        shiftWindow.ShowDialog();
    }

    /// <summary>
    /// Execute OnPropertyChanged on Members to signify a change on TempTag status.
    /// </summary>
    public void RefreshTempTag()
    {
        OnPropertyChanged(nameof(TempTag));
        OnPropertyChanged(nameof(TempTagRF_ID));
        OnPropertyChanged(nameof(CanAssign));
        OnPropertyChanged(nameof(CanUnassign));
        OnPropertyChanged(nameof(HasTempTag));
        OnPropertyChanged(nameof(HasNoTempTag));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return Employee.ToString();
    }
}