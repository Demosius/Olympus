using Pantheon.Annotations;
using Pantheon.ViewModel.Commands.Employees;
using Pantheon.ViewModel.Controls.Shifts;
using Pantheon.ViewModel.Pages;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    private ObservableCollection<Shift> shifts;
    public ObservableCollection<Shift> Shifts
    {
        get => shifts;
        set
        {
            shifts = value;
            OnPropertyChanged();
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
                SingleRule ??= Employee is null ? new SingleRuleVM() : new SingleRuleVM(Employee);
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
                RecurringRule ??= Employee is null ? new RecurringRuleVM() : new RecurringRuleVM(Employee);
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
                if (RosterRule is null)
                {
                    if (Employee is null)
                        RosterRule = new RosterRuleVM();
                    else if (RosterRules.Count == 0)
                        RosterRule = new RosterRuleVM(Employee);
                    else
                        RosterRule = new RosterRuleVM(Employee, RosterRules.First());
                }
                SingleCheck = false;
                RecurringCheck = false;
            }
            OnPropertyChanged();
        }
    }

    private SingleRuleVM? singleRule;
    public SingleRuleVM? SingleRule
    {
        get => singleRule;
        set
        {
            singleRule = value;
            OnPropertyChanged();
        }
    }

    private RecurringRuleVM? recurringRule;
    public RecurringRuleVM? RecurringRule
    {
        get => recurringRule;
        set
        {
            recurringRule = value;
            OnPropertyChanged();
        }
    }

    private RosterRuleVM? rosterRule;
    public RosterRuleVM? RosterRule
    {
        get => rosterRule;
        set
        {
            rosterRule = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ShiftRuleSingle> SingleRules { get; set; }
    public ObservableCollection<ShiftRuleRecurring> RecurringRules { get; set; }
    public ObservableCollection<ShiftRuleRoster> RosterRules { get; set; }

    private ShiftRule? selectedRule;
    public ShiftRule? SelectedRule
    {
        get => selectedRule;
        set
        {
            selectedRule = value;
            OnPropertyChanged();
        }
    }
    

    #endregion

    public Dictionary<string, Shift?> ShiftDict { get; set; }

    #region Commands

    public ConfirmShiftAdjustmentsCommand ConfirmShiftAdjustmentsCommand { get; set; }
    public AddSingleRuleCommand AddSingleRuleCommand { get; set; }
    public AddRecurringRuleCommand AddRecurringRuleCommand { get; set; }
    public AddRosterRuleCommand AddRosterRuleCommand { get; set; }

    #endregion

    public EmployeeShiftVM()
    {
        shiftNames = new ObservableCollection<string>();
        empShifts = new ObservableCollection<EmployeeShift>();
        shifts = new ObservableCollection<Shift>();
        ShiftDict = new Dictionary<string, Shift?>();
        selectedShiftName = string.Empty;
        SingleRules = new ObservableCollection<ShiftRuleSingle>();
        RecurringRules = new ObservableCollection<ShiftRuleRecurring>();
        RosterRules = new ObservableCollection<ShiftRuleRoster>();

        ConfirmShiftAdjustmentsCommand = new ConfirmShiftAdjustmentsCommand(this);
        AddSingleRuleCommand = new AddSingleRuleCommand(this);
        AddRecurringRuleCommand = new AddRecurringRuleCommand(this);
        AddRosterRuleCommand = new AddRosterRuleCommand(this);
    }

    public void SetData(EmployeePageVM employeePageVM, Employee employee)
    {
        ParentVM = employeePageVM;
        Employee = employee;
        Helios = ParentVM.Helios;
        Charon = ParentVM.Charon;

        if (ParentVM.EmployeeDataSet is null || Helios is null || Charon is null) return;

        Employee.DepartmentName = Employee.Department?.Name ?? Employee.DepartmentName;

        SingleRules.Clear();
        RecurringRules.Clear();
        RosterRules.Clear();

        foreach (var ruleSingle in Employee.SingleRules) SingleRules.Add(ruleSingle);
        foreach (var ruleRecurring in Employee.RecurringRules) RecurringRules.Add(ruleRecurring);
        foreach (var ruleRoster in Employee.RosterRules) RosterRules.Add(ruleRoster);

        // We will require a basic list of shifts, and a dictionary where the Key by ID is preserved.
        var shiftList = ParentVM.EmployeeDataSet.Shifts.Values.Where(s => s.DepartmentName == Employee.DepartmentName).ToList();
        var shiftDict = shiftList.ToDictionary(s => s.ID, s => s);

        Shifts = new ObservableCollection<Shift>(shiftList);

        ShiftDict = shiftList.ToDictionary(s => s.Name, s => s)!;
        ShiftDict.Add("<-- No Default -->", null);

        ShiftNames = new ObservableCollection<string>(ShiftDict.Keys);

        if (Employee.DefaultShift is null)
            if (shiftDict.TryGetValue(Employee.DefaultShiftID, out var shift))
                Employee.DefaultShift = shift;

        SelectedShiftName = ShiftDict.TryGetValue(Employee.DefaultShift?.Name ?? "", out _)
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
        foreach (var shift in ShiftDict.Values.Where(s => s is not null && !currentConnections.Contains(s.ID)))
        {
            var newConn = new EmployeeShift(Employee, shift!);
            EmpShifts.Add(newConn);
        }
    }

    public void ConfirmShiftAdjustments()
    {
        if (Helios is null || Employee is null) return;

        Helios.StaffUpdater.EmployeeToShiftConnections(EmpShifts);

        if (Employee.DefaultShift != ShiftDict[SelectedShiftName])
        {
            var shift = ShiftDict[SelectedShiftName];
            Employee.DefaultShift = shift;
            Employee.DefaultShiftID = shift?.ID ?? "";
            Helios.StaffUpdater.Employee(Employee);
        }
    }

    public void AddSingleRule()
    {
        if (Helios is null || Charon is null || Employee is null || SingleRule is null || !SingleRule.IsValid()) return;
        SingleRules.Add(SingleRule.ShiftRule);
        Employee.SingleRules.Add(SingleRule.ShiftRule);
        SingleRule = new SingleRuleVM(Employee);
        Helios.StaffCreator.ShiftRuleSingle(SingleRule.ShiftRule);
    }

    public void AddRecurringRule()
    {
        if (Helios is null || Charon is null || Employee is null || RecurringRule is null || !RecurringRule.IsValid()) return;
        RecurringRules.Add(RecurringRule.ShiftRule);
        Employee.RecurringRules.Add(RecurringRule.ShiftRule);
        RecurringRule = new RecurringRuleVM(Employee);
        Helios.StaffCreator.ShiftRuleRecurring(RecurringRule.ShiftRule);
    }

    public void AddRosterRule()
    {
        if (Helios is null || Charon is null || Employee is null || RosterRule is null || !RosterRule.IsValid()) return;
        RosterRules.Add(RosterRule.ShiftRule);
        Employee.RosterRules.Add(RosterRule.ShiftRule);
        RosterRule = new RosterRuleVM(Employee, RosterRules.First());
        Helios.StaffCreator.ShiftRuleRoster(RosterRule.ShiftRule);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}