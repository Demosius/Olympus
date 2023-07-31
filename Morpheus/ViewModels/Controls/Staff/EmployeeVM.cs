using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Morpheus.ViewModels.Controls.Staff;

public class EmployeeVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public Employee Employee { get; set; }

    private bool CanRead { get; }
    private bool CanReadSensitive { get; }
    private bool CanReadVerySensitive { get; }
    private bool CanUpdate { get; }
    private bool CanDelete { get; }

    #region Employee Access

    /* All Visible */
    public string FullName => Employee.FullName;

    public int ID => Employee.ID;

    public string FirstName
    {
        get => Employee.FirstName;
        set
        {
            if (CanUpdate)
                Employee.FirstName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FullName));
            _ = SaveAsync();
        }
    }

    public string LastName
    {
        get => Employee.LastName;
        set
        {
            if (CanUpdate)
                Employee.LastName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FullName));
            _ = SaveAsync();
        }
    }

    public string DisplayName
    {
        get => Employee.DisplayName;
        set
        {
            if (CanUpdate)
                Employee.DisplayName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string RoleName
    {
        get => Employee.RoleName;
        set
        {
            if (CanUpdate)
                Employee.RoleName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string DepartmentName
    {
        get => Employee.DepartmentName;
        set
        {
            if (CanUpdate)
                Employee.DepartmentName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string ReportsToID
    {
        get => Employee.ReportsToID.ToString();
        set
        {
            if (CanUpdate && int.TryParse(value, out var val))
            {
                Employee.ReportsToID = val;
                _ = SetReportAsync();
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(ReportsTo));
            _ = SaveAsync();
        }
    }

    public string ReportsTo  => Employee.ReportsTo?.FullName ?? "";

    public string PC_ID
    {
        get => Employee.PC_ID;
        set
        {
            if (CanUpdate)
                Employee.PC_ID = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string RF_ID
    {
        get => Employee.RF_ID;
        set
        {
            if (CanUpdate)
                Employee.RF_ID = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string DematicID
    {
        get => Employee.DematicID;
        set
        {
            if (CanUpdate && Regex.IsMatch(value, @"^\d{4}$"))
                Employee.DematicID = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public bool IsActive
    {
        get => Employee.IsActive;
        set
        {
            if (CanUpdate && ConfirmActiveStatusChange(value))
                Employee.IsActive = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    /* Can Read */
    public string Location
    {
        get => CanRead ? Employee.Location : "{--hidden--}";
        set
        {
            if (CanUpdate && CanRead)
                Employee.Location = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string PayPoint
    {
        get => CanRead ? Employee.PayPoint : "{--hidden--}";
        set
        {
            if (CanUpdate && CanRead)
                Employee.PayPoint = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string ClanName
    {
        get => CanRead ? Employee.ClanName : "{--hidden--}";
        set
        {
            if (CanUpdate && CanRead)
                Employee.ClanName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    /* Can Read Sensitive */
    public string PhoneNumber
    {
        get => CanReadSensitive ? Employee.PhoneNumber : "{--hidden--}";
        set
        {
            if (CanUpdate && CanReadSensitive)
                Employee.PhoneNumber = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string Email
    {
        get => CanReadSensitive ? Employee.Email : "{--hidden--}";
        set
        {
            if (CanUpdate && CanReadSensitive)
                Employee.Email = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string Address
    {
        get => CanReadSensitive ? Employee.Address : "{--hidden--}";
        set
        {
            if (CanUpdate && CanReadSensitive)
                Employee.Address = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    /* Can Read Very Sensitive */

    public string PayRate
    {
        get => CanReadVerySensitive ? $"{Employee.PayRate:C}" : "{--hidden--}";
        set
        {
            if (CanUpdate && CanReadVerySensitive && decimal.TryParse(value, out var val))
                Employee.PayRate = val;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }
    #endregion

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public EmployeeVM(Employee employee, Helios helios, Charon charon)
    {
        Employee = employee;
        Helios = helios;
        Charon = charon;

        CanRead = Charon.CanReadEmployee(Employee);
        CanReadSensitive = Charon.CanReadEmployeeSensitive(Employee);
        CanReadVerySensitive = Charon.CanReadEmployeeVerySensitive(Employee);

        CanUpdate = Charon.CanUpdateEmployee(Employee);
        CanDelete = Charon.CanDeleteEmployee(Employee);

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private bool ConfirmActiveStatusChange(bool newActiveStatus)
    {
        if (newActiveStatus == IsActive) return false;

        if (!newActiveStatus)
        {
            if (CanDelete)
                return MessageBox.Show($"Do you want to delete(/deactivate) this user: {Employee}?", "Confirm Deletion",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Information) == MessageBoxResult.Yes;

            MessageBox.Show($"You do not have permission to delete(/deactivate) {Employee}.", "Cannot Delete",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (Charon.CanCreateEmployee())
            return MessageBox.Show($"Are you sure that you want to RE-activate) this user: {Employee}?",
                "Confirm Reactivation",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Information) == MessageBoxResult.Yes;

        MessageBox.Show("You do not have permission to create new employees, or re-activate old ones.",
            "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;

    }

    public async Task RefreshDataAsync()
    {
        Employee = await Helios.StaffReader.EmployeeAsync(Employee.ID) ?? Employee;
    }

    public async Task SetReportAsync()
    {
        Employee.ReportsTo = ReportsToID == string.Empty ? null : await Helios.StaffReader.EmployeeAsync(Employee.ReportsToID);
        OnPropertyChanged(nameof(ReportsTo));
    }

    public async Task SaveAsync()
    {
        if (CanUpdate) await Helios.StaffUpdater.EmployeeAsync(Employee);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}