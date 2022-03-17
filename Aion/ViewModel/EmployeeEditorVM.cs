using Aion.ViewModel.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Aion.View;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Model;

namespace Aion.ViewModel;

public class EmployeeEditorVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    private Employee employee;

    public bool IsNew;

    private List<Employee> employees;
    private List<int> managerIDs;
        
    private bool useManagers;
    public bool UseManagers
    {
        get => useManagers;
        set
        {
            useManagers = value;
            OnPropertyChanged(nameof(UseManagers));
            SetReportList();
        }
    }

    private ObservableCollection<string> locations;
    public ObservableCollection<string> Locations
    {
        get => locations;
        set
        {
            locations = value;
            OnPropertyChanged(nameof(Locations));
        }
    }

    private ObservableCollection<Employee> reports;
    public ObservableCollection<Employee> Reports
    {
        get => reports;
        set
        {
            reports = value;
            OnPropertyChanged(nameof(Reports));
        }
    }

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

    private ObservableCollection<EEmploymentType> employmentTypes;
    public ObservableCollection<EEmploymentType> EmploymentTypes
    {
        get => employmentTypes;
        set
        {
            employmentTypes = value;
            OnPropertyChanged(nameof(EmploymentTypes));
        }
    }

    private ObservableCollection<Role> jobClassifications;
    public ObservableCollection<Role> JobClassifications
    {
        get => jobClassifications;
        set
        {
            jobClassifications = value;
            OnPropertyChanged(nameof(JobClassifications));
        }
    }

    private int code;
    public int Code
    {
        get => code;
        set
        {
            code = value;
            OnPropertyChanged(nameof(Code));
        }
    }

    private string firstName;
    public string FirstName
    {
        get => firstName;
        set
        {
            firstName = value;
            OnPropertyChanged(nameof(FirstName));
        }
    }

    private string surname;
    public string Surname
    {
        get => surname;
        set
        {
            surname = value;
            OnPropertyChanged(nameof(Surname));
        }
    }

    private string location;
    public string Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged(nameof(location));
        }
    }

    private Employee reportsTo;
    public Employee ReportsTo
    {
        get => reportsTo;
        set
        {
            reportsTo = value;
            OnPropertyChanged(nameof(ReportsTo));
        }
    }

    private Department department;
    public Department Department
    {
        get => department;
        set
        {
            department = value;
            PayPoint = department?.PayPoint ?? "";
            OnPropertyChanged(nameof(Department));
        }
    }

    private string payPoint;
    public string PayPoint
    {
        get => payPoint;
        set
        {
            payPoint = value;
            OnPropertyChanged(nameof(PayPoint));
        }
    }

    private EEmploymentType employmentType;
    public EEmploymentType EmploymentType
    {
        get => employmentType;
        set
        {
            employmentType = value;
            OnPropertyChanged(nameof(EmploymentType));
        }
    }

    private Role jobClassification;
    public Role JobClassification
    {
        get => jobClassification;
        set
        {
            jobClassification = value;
            OnPropertyChanged(nameof(JobClassification));
        }
    }

    public ConfirmEmployeeEditCommand ConfirmEmployeeEditCommand { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public AddLocationCommand AddLocationCommand { get; set; }
    public AddPayPointCommand AddPayPointCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    public EmployeeEditorVM()
    {
        ConfirmEmployeeEditCommand = new ConfirmEmployeeEditCommand(this);
        RefreshDataCommand = new RefreshDataCommand(this);
        AddLocationCommand = new AddLocationCommand(this);
        AddPayPointCommand = new AddPayPointCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        UseManagers = true;
    }

    public EmployeeEditorVM(Employee employee)
    {
        SetEmployee(employee);
    }

    public EmployeeEditorVM(Employee employee, bool isNew) : this(employee)
    {
        IsNew = isNew;
    }

    public void SetDataSource(Helios helios)
    {
        Helios = helios;
        RefreshData();
    }

    public void SetEmployee(Employee newEmployee)
    {
        employee = newEmployee;

        Code = newEmployee.ID;
        FirstName = newEmployee.FirstName;
        Surname = newEmployee.LastName;
        Location = newEmployee.Location;
        Department = Departments.FirstOrDefault(d => d.Name == newEmployee.DepartmentName);
        ReportsTo = employees.FirstOrDefault(e => e.ID == newEmployee.ReportsToID);
        PayPoint = newEmployee.PayPoint;
        EmploymentType = newEmployee.EmploymentType;
        JobClassification = JobClassifications.FirstOrDefault(r => r.Name == newEmployee.RoleName);
    }

    public void SetData(Helios helios, Employee newEmployee, bool isNew)
    {
        SetDataSource(helios);
        IsNew = isNew;
        SetEmployee(newEmployee);
    }

    /// <summary>
    /// Sets the ReportTo list based on whether UseManagers is limiting the list to those who already have reports.
    /// </summary>
    public void SetReportList()
    {
        if (employees is null) { return; }

        if (useManagers)
            Reports = new ObservableCollection<Employee>(employees.Where(e => managerIDs.Contains(e.ID)).OrderBy(e => e.FullName));
        else
            Reports = new ObservableCollection<Employee>(employees.OrderBy(e => e.FullName));
    }

    public void RefreshData()
    {
        Helios.StaffReader.AionEmployeeRefresh(out managerIDs, out employees, 
            out var locationsList, out var payPointList, 
            out var employmentTypeList, out var roleList,
            out var departmentList);
            
        Locations = new ObservableCollection<string>(locationsList.OrderBy(s => s));
        PayPoints = new ObservableCollection<string>(payPointList.OrderBy(s => s));
        EmploymentTypes = new ObservableCollection<EEmploymentType>(employmentTypeList);
        JobClassifications = new ObservableCollection<Role>(roleList.OrderBy(r => r.Name));
        Departments = new ObservableCollection<Department>(departmentList.OrderBy(d => d.Name));
            
        SetReportList();
    }

    public void RepairData()
    {
        throw new System.NotImplementedException();
    }

    public void ConfirmEdit()
    {
        employee.FirstName = FirstName;
        employee.LastName = Surname;
        employee.Location = Location;
        employee.ReportsToID = ReportsTo.ID;
        employee.ReportsTo = ReportsTo;
        employee.PayPoint = PayPoint;
        employee.EmploymentType = EmploymentType;
        employee.RoleName = JobClassification.Name;
        employee.Role = JobClassification;

        Helios.StaffUpdater.Employee(employee);
    }

    public void AddLocation()
    {
        InputWindow input = new();
        if (input.ShowDialog() != true) return;

        Locations.Add(input.Input.Text);
        Locations = new ObservableCollection<string>(Locations.OrderBy(s => s));
        Location = input.Input.Text;
    }

    public void AddPayPoint()
    {
        InputWindow input = new();
        if (input.ShowDialog() != true) return;

        PayPoints.Add(input.Input.Text);
        PayPoints = new ObservableCollection<string>(PayPoints.OrderBy(s => s));
        PayPoint = input.Input.Text;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}