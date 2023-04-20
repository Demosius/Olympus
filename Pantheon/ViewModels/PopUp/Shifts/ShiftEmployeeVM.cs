using Pantheon.ViewModels.Commands.Shifts;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Pantheon.ViewModels.Controls.Shifts;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Shifts;

public class ShiftEmployeeVM : INotifyPropertyChanged
{
    public Department Department { get; set; }
    public Shift Shift { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    #region Notifiable Properties

    private ObservableCollection<EmployeeShift> employeeShifts;
    public ObservableCollection<EmployeeShift> EmployeeShifts
    {
        get => employeeShifts;
        set
        {
            employeeShifts = value;
            OnPropertyChanged(nameof(EmployeeShifts));
        }
    }

    private string employeeSearchString;
    public string EmployeeSearchString
    {
        get => employeeSearchString;
        set
        {
            employeeSearchString = value;
            OnPropertyChanged(nameof(EmployeeSearchString));
            ApplyFilters();
        }
    }

    #endregion

    private List<EmployeeShift> empShifts;

    public Dictionary<int, Employee> Employees { get; set; }

    public ConfirmEmployeeAssignmentCommand ConfirmEmployeeAssignmentCommand { get; set; }

    public ShiftEmployeeVM(ShiftVM shift)
    {
        Shift = shift.Shift;
        Department = shift.Department ?? Shift.Department ?? new Department(Shift.ID.Split('|')[0]);

        Helios = shift.Helios;
        Charon = shift.Charon;

        employeeShifts = new ObservableCollection<EmployeeShift>();
        Employees = new Dictionary<int, Employee>();
        employeeSearchString = string.Empty;
        empShifts = new List<EmployeeShift>();

        ConfirmEmployeeAssignmentCommand = new ConfirmEmployeeAssignmentCommand(this);

        SetData();
    }

    public void SetData()
    {
        empShifts = Helios.StaffReader.EmployeeShifts(Shift).ToList(); 
        Employees = Helios.StaffReader.Employees(e => e.DepartmentName == Department.Name).ToDictionary(e => e.ID, e => e);

        // Set current connections as active and original.
        foreach (var employeeShift in empShifts)
        {
            employeeShift.Active = true;
            employeeShift.Original = true;
            employeeShift.Shift = Shift;
            if (Employees.TryGetValue(employeeShift.EmployeeID, out var employee))
                employeeShift.Employee = employee;
        }

        // Create remaining potential employee connections.
        var currentConnections = empShifts.Select(es => es.EmployeeID);
        foreach (var employee in Employees.Values.Where(e => !currentConnections.Contains(e.ID)))
        {
            var newConn = new EmployeeShift(employee, Shift);
            empShifts.Add(newConn);
        }

        // Sort empShifts.
        empShifts.Sort();

        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var regex = new Regex(EmployeeSearchString, RegexOptions.IgnoreCase);
        EmployeeShifts = new ObservableCollection<EmployeeShift>(empShifts.Where(e => regex.IsMatch(e.Employee?.FullName ?? "")));
    }

    public void ConfirmEmployeeAssignment()
    {
        Helios.StaffUpdater.EmployeeToShiftConnections(EmployeeShifts);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}