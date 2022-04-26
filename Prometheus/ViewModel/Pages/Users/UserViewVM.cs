using System.Collections.ObjectModel;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Styx.Interfaces;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Model;
using Uranus.Users.Model;

namespace Prometheus.ViewModel.Pages.Users;

internal class UserViewVM : INotifyPropertyChanged, IDataSource, IDBInteraction
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public ObservableCollection<User> Users { get; set; }
    private EmployeeDataSet? employeeDataSet;

    #region INotifyPropertyChanged Members

    private User selectedUser;
    public User SelectedUser
    {
        get => selectedUser;
        set
        {
            selectedUser = value;
            OnPropertyChanged();
        }
    }

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
        }
    }

    private ESortMethod selectedSortMethod;
    public ESortMethod SelectedSortMethod
    {
        get => selectedSortMethod;
        set
        {
            selectedSortMethod = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public UserViewVM()
    {
        Users = new ObservableCollection<User>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshData();
    }

    public void RefreshData()
    {
        if (Helios is null || Charon is null) return;

        GatherUsers();
    }

    private void GatherUsers()
    {
        if (Helios is null || Charon is null) return;

        employeeDataSet = Helios.StaffReader.EmployeeDataSet();

        var userList = Helios.UserReader.Users();

        foreach (var user in userList)
        {
            if (!employeeDataSet.Employees.TryGetValue(user.ID, out var employee)) continue;
            user.Employee = employee;
            Users.Add(user);
        }
    }

    public void RepairData()
    {
        throw new System.NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}