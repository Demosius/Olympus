using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interfaces;
using Pantheon.Views.PopUp.Employees;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class DepartmentSelectionVM : INotifyPropertyChanged, ICreationMode, ISelector, IPayPoints, IDepartments, IManagers
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public bool UserCanCreate { get; }
    public bool CanCreate => UserCanCreate && NewDepartmentName.Length > 0 && 
                             !Departments.Select(d => d.Name).Contains(NewDepartmentName);

    public bool UserCanDelete { get; }
    public bool CanDelete => UserCanDelete && SelectedDepartment is not null && SelectedDepartment.IsDeletable;

    public bool CanConfirm => SelectedDepartment is not null;

    public bool ShowNew => UserCanCreate && InCreation;
    public bool ShowCreationOption => UserCanCreate && !InCreation;

    #region INotifyPropertChanged

    public ObservableCollection<Department> Departments { get; set; }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDelete));
            OnPropertyChanged(nameof(CanConfirm));
        }
    }

    private bool inCreation;
    public bool InCreation
    {
        get => inCreation;
        set
        {
            inCreation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowNew));
            OnPropertyChanged(nameof(ShowCreationOption));
        }
    }

    // New Department Values

    private string newDepartmentName;
    public string NewDepartmentName
    {
        get => newDepartmentName;
        set
        {
            newDepartmentName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanCreate));
        }
    }

    private string payPoint;
    public string PayPoint
    {
        get => payPoint;
        set
        {
            payPoint = value;
            OnPropertyChanged();
        }
    }

    private Department? parentDepartment;
    public Department? ParentDepartment
    {
        get => parentDepartment;
        set
        {
            parentDepartment = value;
            OnPropertyChanged();
        }
    }

    private Employee? departmentHead;
    public Employee? DepartmentHead
    {
        get => departmentHead;
        set
        {
            departmentHead = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ActivateCreationCommand ActivateCreationCommand { get; set; }
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }
    public SelectPayPointCommand SelectPayPointCommand { get; }
    public ClearPayPointCommand ClearPayPointCommand { get; set; }
    public SelectManagerCommand SelectManagerCommand { get; set; }
    public ClearManagerCommand ClearManagerCommand { get; set; }
    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public ClearDepartmentCommand ClearDepartmentCommand { get; set; }

    #endregion

    public DepartmentSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        UserCanCreate = Charon.CanCreateDepartment();
        UserCanDelete = Charon.CanDeleteDepartment();

        payPoint = string.Empty;
        newDepartmentName = string.Empty;

        // Get departments and make sure that it can be determined if they should be deletable (no employee or department reference broken by removal.
        // Include children should include employees and sub departments without too much overloading (?)
        Departments = new ObservableCollection<Department>(AsyncHelper.RunSync(() => Helios.StaffReader.DepartmentsAsync(null, EPullType.IncludeChildren)).OrderBy(d => d.Name));
        
        ActivateCreationCommand = new ActivateCreationCommand(this);
        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
        SelectPayPointCommand = new SelectPayPointCommand(this);
        SelectManagerCommand = new SelectManagerCommand(this);
        SelectDepartmentCommand = new SelectDepartmentCommand(this);
        ClearManagerCommand = new ClearManagerCommand(this);
        ClearDepartmentCommand = new ClearDepartmentCommand(this);
        ClearPayPointCommand = new ClearPayPointCommand(this);
    }

    public void ActivateCreation()
    {
        InCreation = !InCreation;
    }

    public void Create()
    {
        if (!CanCreate)  return;

        var newDepartment  = new Department
        {
            Name = NewDepartmentName,
            PayPoint = PayPoint,
            HeadID = DepartmentHead?.ID ?? 0,
            OverDepartmentName = ParentDepartment?.Name ?? string.Empty,
        };

        Helios.StaffCreator.Department(newDepartment);

        Departments.Clear();

        var depList = AsyncHelper.RunSync(() => Helios.StaffReader.DepartmentsAsync(null, EPullType.IncludeChildren)).OrderBy(d => d.Name);

        foreach (var department in depList)
        {
            Departments.Add(department);
        }

        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == NewDepartmentName);

        NewDepartmentName = string.Empty;
        PayPoint = string.Empty;
        ParentDepartment = null;
        DepartmentHead = null;

        InCreation = false;
    }

    public void Delete()
    {
        // Confirm ability and desire to delete the department.
        if (!CanDelete || SelectedDepartment is null) return;
        if (MessageBox.Show($"Are you sure that you want to delete the {SelectedDepartment.Name} Department?",
                "Confirm Deletion", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) !=
            MessageBoxResult.Yes) return;

        // Delete from database.
        Helios.StaffDeleter.Department(SelectedDepartment);

        // Delete from department list.
        Departments.Remove(SelectedDepartment);

        SelectedDepartment = null;
    }

    public void SelectPayPoint()
    {
        var payPointSelector = new PayPointSelectionWindow(Helios, Charon);
        payPointSelector.ShowDialog();
        
        if (payPointSelector.DialogResult != true) return;
        if (payPointSelector.VM.SelectedPayPoint is null) return;

        PayPoint = payPointSelector.VM.SelectedPayPoint.Name;
    }

    public void ClearPayPoint()
    {
        PayPoint = string.Empty;
    }

    public void SelectManager()
    {
        var fullEmployeeList = AsyncHelper.RunSync(() => Helios.StaffReader.EmployeesAsync()).OrderBy(e => e.FullName).Select(e => new EmployeeVM(e, Charon, Helios)).ToList();

        var mangerSelector = new EmployeeSelectionWindow(fullEmployeeList, true, ParentDepartment?.Name);
        mangerSelector.ShowDialog();

        if (mangerSelector.DialogResult != true) return;
        if (mangerSelector.VM.SelectedEmployee is null) return;

        DepartmentHead = mangerSelector.VM.SelectedEmployee.Employee;
    }

    public void ClearManager()
    {
        DepartmentHead = null;
    }

    public void SelectDepartment()
    {
        var departmentSelector = new DepartmentSelectionWindow(Helios, Charon);
        departmentSelector.ShowDialog();

        if (departmentSelector.DialogResult != true) return;
        if (departmentSelector.VM.SelectedDepartment is null) return;

        ParentDepartment = departmentSelector.VM.SelectedDepartment;
    }

    public void ClearDepartment()
    {
        ParentDepartment = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

