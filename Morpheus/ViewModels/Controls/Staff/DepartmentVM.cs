using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Morpheus.ViewModels.Controls.Staff;

public class DepartmentVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public Department Department { get; set; }

    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    #region Department Access

    public string Name
    {
        get => Department.Name;
        set => _ = UpdateNameAsync(value);
    }

    public string HeadID
    {
        get => Department.HeadID.ToString();
        set
        {
            if (CanUpdate && int.TryParse(value, out var val))
            {
                Department.HeadID = val;
                _ = SetHeadAsync();
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(Head));
            _ = SaveAsync();
        }
    }

    public string Head => Department.Head?.FullName ?? "";

    public string OverDepartmentName
    {
        get => Department.OverDepartmentName;
        set
        {
            if (CanUpdate)
                Department.OverDepartmentName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string PayPoint
    {
        get => Department.PayPoint;
        set
        {
            if (CanUpdate) 
                Department.PayPoint = value;
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

    public DepartmentVM(Department department, Helios helios, Charon charon)
    {
        Department = department;
        Helios = helios;
        Charon = charon;

        CanCreate = Charon.CanCreateDepartment();
        CanUpdate = Charon.CanUpdateDepartment();
        CanDelete = Charon.CanDeleteDepartment();

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task UpdateNameAsync(string newName)
    {
        if (CanUpdate && Name != newName)
            await Helios.StaffUpdater.DepartmentNameChangeAsync(Department, newName);
        OnPropertyChanged(nameof(Name));
    }

    public async Task SetHeadAsync()
    {
        Department.Head = HeadID == string.Empty ? null : await Helios.StaffReader.EmployeeAsync(Department.HeadID);
        OnPropertyChanged(nameof(Head));
    }

    public async Task RefreshDataAsync()
    {
        Department = await Helios.StaffReader.DepartmentAsync(Department.Name) ?? Department;
    }

    public async Task SaveAsync()
    {
        if (CanUpdate) await Helios.StaffUpdater.DepartmentAsync(Department);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}