using System.Collections.Generic;
using Pantheon.Annotations;
using Pantheon.ViewModel.Commands;
using Pantheon.ViewModel.Pages;
using Styx;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel;

internal class ClanCreationVM : INotifyPropertyChanged
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }
    public EmployeePageVM? ParentVM { get; set; }

    public Clan Clan { get; set; }

    public List<string> ClanNames { get; set; }

    #region Notifiable Properties
    
    private ObservableCollection<Department> departments;
    public ObservableCollection<Department> Departments
    {
        get => departments;
        set
        {
            departments = value;
            OnPropertyChanged(nameof(Departments));
        }
    }

    private ObservableCollection<Employee> employees;
    public ObservableCollection<Employee> Employees
    {
        get => employees;
        set
        {
            employees = value;
            OnPropertyChanged(nameof(Employees));
        }
    }

    #endregion

    public ConfirmClanCreationCommand ConfirmClanCreationCommand { get; set; }

    public ClanCreationVM()
    {
        Clan = new Clan();
        departments = new ObservableCollection<Department>();
        employees = new ObservableCollection<Employee>();
        ClanNames = new List<string>();

        ConfirmClanCreationCommand = new ConfirmClanCreationCommand(this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetDataSources(EmployeePageVM employeePage)
    {
        ParentVM = employeePage;
        Helios = ParentVM.Helios;
        Charon = ParentVM.Charon;

        foreach (var parentVMDepartment in ParentVM.Departments)
        {
            if (parentVMDepartment is not null) Departments.Add(parentVMDepartment);
        }

        Employees = new ObservableCollection<Employee>(ParentVM.ReportingEmployees);

        ClanNames = ParentVM.Clans.Select(c => c.Name).ToList();
    }

    public bool ConfirmClanCreation()
    {
        if (Clan.Name is null or "" || Helios is null || 
            Clan.Department is null || Clan.Leader is null) return false;

        Clan.DepartmentName = Clan.Department.Name;
        Clan.LeaderID = Clan.Leader.ID;

        Clan.Leader.ClanName = Clan.Name;

        Helios.StaffUpdater.Employee(Clan.Leader);
        
        return Helios.StaffCreator.Clan(Clan);
    }
}