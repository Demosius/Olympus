using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Controls.Shifts;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interfaces;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class EmployeeShiftVM : INotifyPropertyChanged
{
    public EmployeeVM EmployeeVM { get; set; }
    public Employee Employee => EmployeeVM.Employee;

    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public IShiftRuleVM? Rule =>
        RosterCheck ? RosterRule :
        SingleCheck ? SingleRule : 
        RecurringCheck ? RecurringRule :
        null;

    public bool InEdit =>
        RosterCheck ? RosterRule?.InEdit ?? false :
        SingleCheck ? SingleRule?.InEdit ?? false : 
        RecurringCheck && (RecurringRule?.InEdit ?? false);

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
                SingleRule ??= new SingleRuleVM(Employee);
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
                RecurringRule ??= new RecurringRuleVM(Employee);
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
                RosterRule ??= RosterRules.Count == 0
                    ? new RosterRuleVM(EmployeeVM.Employee)
                    : new RosterRuleVM(EmployeeVM.Employee, RosterRules.First());

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
    public EditShiftRuleCommand EditShiftRuleCommand { get; set; }
    public DeleteShiftRuleCommand DeleteShiftRuleCommand { get; set; }
    public AddRuleCommand AddRuleCommand { get; set; }
    public CancelRuleEditCommand CancelRuleEditCommand { get; set; }

    #endregion

    public EmployeeShiftVM(EmployeeVM employee)
    {
        EmployeeVM = employee;
        Helios = EmployeeVM.Helios;
        Charon = EmployeeVM.Charon;

        shiftNames = new ObservableCollection<string>();
        empShifts = new ObservableCollection<EmployeeShift>();
        shifts = new ObservableCollection<Shift>();
        ShiftDict = new Dictionary<string, Shift?>();
        selectedShiftName = string.Empty;
        SingleRules = new ObservableCollection<ShiftRuleSingle>();
        RecurringRules = new ObservableCollection<ShiftRuleRecurring>();
        RosterRules = new ObservableCollection<ShiftRuleRoster>();
        EditShiftRuleCommand = new EditShiftRuleCommand(this);
        DeleteShiftRuleCommand = new DeleteShiftRuleCommand(this);
        CancelRuleEditCommand = new CancelRuleEditCommand(this);
        ConfirmShiftAdjustmentsCommand = new ConfirmShiftAdjustmentsCommand(this);
        AddRuleCommand = new AddRuleCommand(this);

        SetData();
    }

    private void SetData()
    {
        SingleRules.Clear();
        RecurringRules.Clear();
        RosterRules.Clear();

        foreach (var ruleSingle in EmployeeVM.SingleRules) SingleRules.Add(ruleSingle);
        foreach (var ruleRecurring in EmployeeVM.RecurringRules) RecurringRules.Add(ruleRecurring);
        foreach (var ruleRoster in EmployeeVM.RosterRules) RosterRules.Add(ruleRoster);

        // We will require a basic list of shifts, and a dictionary where the Key by ID is preserved.
        var shiftList = Helios.StaffReader.Shifts(Employee).ToList();
        var shiftDict = shiftList.ToDictionary(s => s.ID, s => s);

        Shifts = new ObservableCollection<Shift>(shiftList);

        ShiftDict = shiftList.ToDictionary(s => s.Name, s => s)!;
        ShiftDict.Add("<-- No Default -->", null);

        ShiftNames = new ObservableCollection<string>(ShiftDict.Keys);

        if (EmployeeVM.DefaultShift is null)
            if (shiftDict.TryGetValue(EmployeeVM.DefaultShiftID, out var shift))
                EmployeeVM.DefaultShift = shift;

        SelectedShiftName = ShiftDict.TryGetValue(EmployeeVM.DefaultShift?.Name ?? "", out _)
            ? EmployeeVM.DefaultShift?.Name ?? ""
            : "<-- No Default -->";

        EmpShifts = new ObservableCollection<EmployeeShift>(Helios.StaffReader.EmployeeShifts(Employee)
            .Where(es => shiftDict.ContainsKey(es.ShiftID)));

        // Set current connections as active and original.
        foreach (var employeeShift in EmpShifts)
        {
            employeeShift.Active = true;
            employeeShift.Original = true;
            employeeShift.Employee = EmployeeVM.Employee;
            if (shiftDict.TryGetValue(employeeShift.ShiftID, out var shift))
                employeeShift.Shift = shift;
        }

        // Create remaining potential shift connections.
        var currentConnections = empShifts.Select(es => es.ShiftID);
        foreach (var shift in ShiftDict.Values.Where(s => s is not null && !currentConnections.Contains(s.ID)))
        {
            var newConn = new EmployeeShift(EmployeeVM.Employee, shift!);
            EmpShifts.Add(newConn);
        }
    }

    public void ConfirmShiftAdjustments()
    {
        Helios.StaffUpdater.EmployeeToShiftConnections(EmpShifts);

        if (EmployeeVM.DefaultShift == ShiftDict[SelectedShiftName]) return;

        var shift = ShiftDict[SelectedShiftName];
        EmployeeVM.DefaultShift = shift;
        EmployeeVM.DefaultShiftID = shift?.ID ?? "";
        Helios.StaffUpdater.Employee(EmployeeVM.Employee);
    }

    private void AddSingleRule()
    {
        if (SingleRule is null || !SingleRule.IsValid) return;
        var shiftRule = SingleRule.ShiftRule;

        if (SingleRule.InEdit)
            Helios.StaffUpdater.ShiftRuleSingle(shiftRule);
        else
            Helios.StaffCreator.ShiftRuleSingle(shiftRule);

        SingleRules.Add(shiftRule);
        EmployeeVM.SingleRules.Add(shiftRule);
        SingleRule = new SingleRuleVM(EmployeeVM.Employee);
    }

    private void AddRecurringRule()
    {
        if (RecurringRule is null ||
            !RecurringRule.IsValid) return;
        var shiftRule = RecurringRule.ShiftRule;

        if (RecurringRule.InEdit)
            Helios.StaffUpdater.ShiftRuleRecurring(shiftRule);
        else
            Helios.StaffCreator.ShiftRuleRecurring(shiftRule);

        RecurringRules.Add(shiftRule);
        EmployeeVM.RecurringRules.Add(shiftRule);
        RecurringRule = new RecurringRuleVM(EmployeeVM.Employee);
    }

    private void AddRosterRule()
    {
        if (RosterRule is null || !RosterRule.IsValid) return;
        var shiftRule = RosterRule.RosterRule;

        if (RosterRule.InEdit)
            Helios.StaffUpdater.ShiftRuleRoster(shiftRule);
        else
            Helios.StaffCreator.ShiftRuleRoster(shiftRule);

        RosterRules.Add(shiftRule);
        EmployeeVM.RosterRules.Add(shiftRule);
        RosterRule = new RosterRuleVM(EmployeeVM.Employee, RosterRules.First());
    }

    public void AddRule()
    {
        if (RosterCheck)
            AddRosterRule();
        else if (RecurringCheck)
            AddRecurringRule();
        else if (SingleCheck)
            AddSingleRule();
    }

    private void CancelSingleRuleEdit()
    {
        if (SingleRule is null || SingleRule.IsNew || SingleRule.Original is null) return;

        SingleRules.Add(SingleRule.Original);
        EmployeeVM.SingleRules.Add(SingleRule.Original);
        SingleRule = new SingleRuleVM(EmployeeVM.Employee);
    }

    private void CancelRecurringRuleEdit()
    {
        if (RecurringRule is null || RecurringRule.IsNew || RecurringRule.Original is null) return;

        RecurringRules.Add(RecurringRule.Original);
        EmployeeVM.RecurringRules.Add(RecurringRule.Original);
        RecurringRule = new RecurringRuleVM(EmployeeVM.Employee);
    }

    private void CancelRosterRuleEdit()
    {
        if (RosterRule is null || RosterRule.IsNew || RosterRule.Original is null) return;

        RosterRules.Add(RosterRule.Original);
        EmployeeVM.RosterRules.Add(RosterRule.Original);
        RosterRule = new RosterRuleVM(EmployeeVM.Employee, RosterRule.Original);
    }

    public void CancelRuleEdit()
    {
        if (RecurringCheck)
            CancelRecurringRuleEdit();
        else if (RosterCheck)
            CancelRosterRuleEdit();
        else if (SingleCheck)
            CancelSingleRuleEdit();
    }

    public void EditShiftRule()
    {
        if (SelectedRule is null) return;

        switch (SelectedRule)
        {
            case ShiftRuleSingle single:
                CancelSingleRuleEdit();
                SingleRule = new SingleRuleVM(single);
                SingleRules.Remove(single);
                EmployeeVM.SingleRules.Remove(single);
                SingleCheck = true;
                break;
            case ShiftRuleRecurring recurring:
                CancelRecurringRuleEdit();
                RecurringRule = new RecurringRuleVM(recurring);
                RecurringRules.Remove(recurring);
                EmployeeVM.RecurringRules.Remove(recurring);
                RecurringCheck = true;
                break;
            case ShiftRuleRoster roster:
                CancelRosterRuleEdit();
                RosterRule = new RosterRuleVM(roster);
                RosterRules.Remove(roster);
                EmployeeVM.RosterRules.Remove(roster);
                RosterCheck = true;
                break;
        }

        OnPropertyChanged(nameof(InEdit));

        SelectedRule = null;
    }

    public void DeleteShiftRule()
    {
        if (SelectedRule is null) return;

        switch (SelectedRule)
        {
            case ShiftRuleSingle single:
                SingleRules.Remove(single);
                Helios.StaffDeleter.SingleRule(single);
                EmployeeVM.SingleRules.Remove(single);
                break;
            case ShiftRuleRecurring recurring:
                RecurringRules.Remove(recurring);
                Helios.StaffDeleter.RecurringRule(recurring);
                EmployeeVM.RecurringRules.Remove(recurring);
                break;
            case ShiftRuleRoster roster:
                RosterRules.Remove(roster);
                Helios.StaffDeleter.RosterRule(roster);
                EmployeeVM.RosterRules.Remove(roster);
                break;
        }

        SelectedRule = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}