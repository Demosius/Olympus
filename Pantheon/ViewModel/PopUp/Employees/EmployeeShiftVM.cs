using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModel.Commands;
using Pantheon.ViewModel.Commands.Employees;
using Pantheon.ViewModel.Pages;
using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.PopUp.Employees;

internal class EmployeeShiftVM : INotifyPropertyChanged
{
    public EmployeePageVM? ParentVM { get; set; }
    public Employee? Employee { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region Notifiable Properties

    private ObservableCollection<string> shiftNames;
    public ObservableCollection<string> ShiftNames
    {
        get => shiftNames;
        set
        {
            shiftNames = value;
            OnPropertyChanged(nameof(ShiftNames));
        }
    }

    private string selectedShiftName;
    public string SelectedShiftName
    {
        get => selectedShiftName;
        set
        {
            selectedShiftName = value;
            OnPropertyChanged(nameof(SelectedShiftName));
        }
    }

    private ObservableCollection<EmployeeShift> empShifts;
    public ObservableCollection<EmployeeShift> EmpShifts
    {
        get => empShifts;
        set
        {
            empShifts = value;
            OnPropertyChanged(nameof(EmpShifts));
        }
    }

    private bool singleCheck;
    public bool SingleCheck
    {
        get => singleCheck;
        set
        {
            singleCheck = value;
            if (singleCheck)
            {
                SingleRule ??= new ShiftRuleSingle();
                RecurringCheck = false;
                RosterCheck = false;
            }
            OnPropertyChanged();
        }
    }

    private bool recurringCheck;
    public bool RecurringCheck
    {
        get => recurringCheck;
        set
        {
            recurringCheck = value;
            if (recurringCheck)
            {
                RecurringRule ??= new ShiftRuleRecurring();
                SingleCheck = false;
                RosterCheck = false;
            }
            OnPropertyChanged();
        }
    }

    private bool rosterCheck;
    public bool RosterCheck
    {
        get => rosterCheck;
        set
        {
            rosterCheck = value;
            if (rosterCheck)
            {
                RosterRule ??= new ShiftRuleRoster();
                SingleCheck = false;
                RecurringCheck = false;
            }
            OnPropertyChanged();
        }
    }

    private ShiftRuleSingle? singleRule;
    public ShiftRuleSingle? SingleRule
    {
        get => singleRule;
        set
        {
            singleRule = value;
            OnPropertyChanged();
        }
    }

    private ShiftRuleRecurring? recurringRule;
    public ShiftRuleRecurring? RecurringRule
    {
        get => recurringRule;
        set
        {
            recurringRule = value;
            OnPropertyChanged();
        }
    }

    private ShiftRuleRoster? rosterRule;
    public ShiftRuleRoster? RosterRule
    {
        get => rosterRule;
        set
        {
            rosterRule = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public Dictionary<string, Shift?> Shifts { get; set; }

    public ConfirmShiftAdjustmentsCommand ConfirmShiftAdjustmentsCommand { get; set; }

    public EmployeeShiftVM()
    {
        shiftNames = new ObservableCollection<string>();
        empShifts = new ObservableCollection<EmployeeShift>();
        Shifts = new Dictionary<string, Shift?>();
        selectedShiftName = string.Empty;

        ConfirmShiftAdjustmentsCommand = new ConfirmShiftAdjustmentsCommand(this);
    }

    public void SetData(EmployeePageVM employeePageVM, Employee employee)
    {
        ParentVM = employeePageVM;
        Employee = employee;
        Helios = ParentVM.Helios;
        Charon = ParentVM.Charon;

        if (ParentVM.EmployeeDataSet is null || Helios is null || Charon is null) return;

        Employee.DepartmentName = Employee.Department?.Name ?? Employee.DepartmentName;

        // We will require a basic list of shifts, and a dictionary where the Key by ID is preserved.
        var shiftList = ParentVM.EmployeeDataSet.Shifts.Values.Where(s => s.DepartmentName == Employee.DepartmentName).ToList();
        var shiftDict = shiftList.ToDictionary(s => s.ID, s => s); 

        Shifts = shiftList.ToDictionary(s => s.Name, s => s)!;
        Shifts.Add("<-- No Default -->", null);

        ShiftNames = new ObservableCollection<string>(Shifts.Keys);

        if (Employee.DefaultShift is null)
            if (shiftDict.TryGetValue(Employee.DefaultShiftID, out var shift))
                Employee.DefaultShift = shift;

        SelectedShiftName = Shifts.TryGetValue(Employee.DefaultShift?.Name ?? "", out _)
            ? Employee.DefaultShift?.Name ?? ""
            : "<-- No Default -->";

        EmpShifts = new ObservableCollection<EmployeeShift>(Helios.StaffReader.EmployeeShifts(employee).Where(es => shiftDict.ContainsKey(es.ShiftID)));

        // Set current connections as active and original.
        foreach (var employeeShift in EmpShifts)
        {
            employeeShift.Active = true;
            employeeShift.Original = true;
            employeeShift.Employee = Employee;
            if (shiftDict.TryGetValue(employeeShift.ShiftID, out var shift))
                employeeShift.Shift = shift;
        }

        // Create remaining potential shift connections.
        var currentConnections = empShifts.Select(es => es.ShiftID);
        foreach (var shift in Shifts.Values.Where(s => s is not null && !currentConnections.Contains(s.ID)))
        {
            var newConn = new EmployeeShift(Employee, shift!);
            EmpShifts.Add(newConn);
        }
    }

    public void ConfirmShiftAdjustments()
    {
        if (Helios is null || Employee is null) return;

        Helios.StaffUpdater.EmployeeToShiftConnections(EmpShifts);

        if (Employee.DefaultShift != Shifts[SelectedShiftName])
        {
            var shift = Shifts[SelectedShiftName];
            Employee.DefaultShift = shift;
            Employee.DefaultShiftID = shift?.ID ?? "";
            Helios.StaffUpdater.Employee(Employee);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}