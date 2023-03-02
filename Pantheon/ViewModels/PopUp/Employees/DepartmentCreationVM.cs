using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Interface;
using Pantheon.Views;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Controls.Employees;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class DepartmentCreationVM : INotifyPropertyChanged, IPayPoints
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public EmployeeVM ParentVM { get; set; }

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

    private ObservableCollection<EmployeeVM> employees;
    public ObservableCollection<EmployeeVM> Employees
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

    public SelectPayPointCommand SelectPayPointCommand { get; }
    public ConfirmDepartmentCreationCommand ConfirmDepartmentCreationCommand { get; set; }

    #endregion

    public DepartmentCreationVM(EmployeeVM employeePageVM)
    {
        ParentVM = employeePageVM;
        Helios = ParentVM.Helios;
        Charon = ParentVM.Charon;

        employees = new ObservableCollection<EmployeeVM>(Helios.StaffReader.GetReportsByRole(Charon.Employee?.Role ?? new Role()).Select(e => new EmployeeVM(e, Charon, Helios)));
        departments = new ObservableCollection<string>(employees.Select(e => e.DepartmentName).Distinct().OrderBy(n => n));
        payPoints = new ObservableCollection<string>(Helios.StaffReader.PayPoints());
        DepartmentNames = Helios.StaffReader.Departments().Select(d => d.Name).ToList();

        Department = new Department();
        SelectPayPointCommand = new SelectPayPointCommand(this);
        ConfirmDepartmentCreationCommand = new ConfirmDepartmentCreationCommand(this);
    }
    

    public void SelectPayPoint()
    {
        var input = new InputWindow("Enter new Pay Point:", "New PayPoint");
        if (input.ShowDialog() != true) return;

        PayPoints.Add(input.VM.Input);
    }

    public bool ConfirmDepartmentCreation()
    {
        if (Department.Name is null or "") return false;

        if (AssignDepartmentHead && Department.Head is not null) Department.HeadID = Department.Head.ID;

        return Helios.StaffCreator.Department(Department);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}