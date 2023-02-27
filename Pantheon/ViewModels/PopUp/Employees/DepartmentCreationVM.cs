using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Interface;
using Pantheon.ViewModels.Pages;
using Pantheon.Views;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

internal class DepartmentCreationVM : INotifyPropertyChanged, IPayPoints
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }
    public EmployeePageVM? ParentVM { get; set; }

    public Department Department { get; set; }

    public List<string> DepartmentNames { get; set; }

    #region Notifiable Properties

    private ObservableCollection<string> departments;
    public ObservableCollection<string> Departments
    {
        get => departments;
        set
        {
            departments = value;
            OnPropertyChanged(nameof(Departments));
        }
    }

    private ObservableCollection<string> payPoints;
    public ObservableCollection<string> PayPoints
    {
        get => payPoints;
        set
        {
            payPoints = value;
            OnPropertyChanged(nameof(PayPoints));
        }
    }

    private bool assignDepartmentHead;
    public bool AssignDepartmentHead
    {
        get => assignDepartmentHead;
        set
        {
            assignDepartmentHead = value;
            OnPropertyChanged(nameof(AssignDepartmentHead));
        }
    }

    private Employee? deptHead;
    public Employee? DeptHead
    {
        get => deptHead;
        set
        {
            deptHead = value;
            OnPropertyChanged(nameof(DeptHead));
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

    #region Commands

    public AddPayPointCommand AddPayPointCommand { get; }
    public ConfirmDepartmentCreationCommand ConfirmDepartmentCreationCommand { get; set; }

    #endregion

    public DepartmentCreationVM()
    {
        Department = new Department();
        employees = new ObservableCollection<Employee>();
        departments = new ObservableCollection<string>();
        payPoints = new ObservableCollection<string>();
        AddPayPointCommand = new AddPayPointCommand(this);
        ConfirmDepartmentCreationCommand = new ConfirmDepartmentCreationCommand(this);
        DepartmentNames = new List<string>();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetDataSources(EmployeePageVM employeePageVM)
    {
        ParentVM = employeePageVM;
        Helios = ParentVM.Helios;
        Charon = ParentVM.Charon;

        Departments = new ObservableCollection<string>(ParentVM.Departments.Select(d => d is null ? "" : d.Name).OrderBy(n => n));
        Employees = new ObservableCollection<Employee>(ParentVM.ReportingEmployees);
        PayPoints = new ObservableCollection<string>(ParentVM.PayPoints);
        DepartmentNames = ParentVM.FullDepartments.Select(d => d.Name).ToList();
    }

    public void AddPayPoint()
    {
        var input = new InputWindow("Enter new Pay Point:", "New PayPoint");
        if (input.ShowDialog() != true) return;

        PayPoints.Add(input.VM.Input);
    }

    public bool ConfirmDepartmentCreation()
    {
        if (Department.Name is null or "" || Helios is null) return false;

        if (AssignDepartmentHead && Department.Head is not null) Department.HeadID = Department.Head.ID;

        return Helios.StaffCreator.Department(Department);
    }
}