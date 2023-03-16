using Pantheon.ViewModels.Commands.Rosters;
using Styx;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Pantheon.Annotations;
using Uranus;
using Uranus.Commands;
using Uranus.Extensions;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Rosters;

public class RosterCreationVM : INotifyPropertyChanged, IDBInteraction
{
    public Department Department { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public DepartmentRoster? Roster { get; set; }

    #region Notifiable Properties

    private string rosterName;
    public string RosterName
    {
        get => rosterName;
        set
        {
            rosterName = value;
            OnPropertyChanged(nameof(RosterName));
        }
    }

    private DateTime startDate;
    public DateTime StartDate
    {
        get => startDate;
        set
        {
            value = value.AddDays(DayOfWeek.Monday - value.DayOfWeek);
            startDate = value;
            OnPropertyChanged(nameof(StartDate));
            RosterName = $"{value.FiscalWeek()} ({value.Year})";
        }
    }

    private bool useSaturdays;
    public bool UseSaturdays
    {
        get => useSaturdays;
        set
        {
            useSaturdays = value;
            OnPropertyChanged(nameof(UseSaturdays));
        }
    }

    private bool useSundays;
    public bool UseSundays
    {
        get => useSundays;
        set
        {
            useSundays = value;
            OnPropertyChanged(nameof(UseSundays));
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public ConfirmDepartmentRosterCreationCommand ConfirmDepartmentRosterCreationCommand { get; set; }

    #endregion

    public RosterCreationVM(Department department, Helios helios, Charon charon)
    {
        Department = department;
        Helios = helios;
        Charon = charon;

        startDate = DateTime.Today.AddDays(DayOfWeek.Monday - DateTime.Today.DayOfWeek + 7);
        rosterName = $"{startDate.FiscalWeek()} ({startDate.Year})";

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ConfirmDepartmentRosterCreationCommand = new ConfirmDepartmentRosterCreationCommand(this);

        RefreshData();
    }
    
    public void RefreshData()
    {
        if (Department.DepartmentRosters.Any())
            StartDate = Department.DepartmentRosters.Select(dr => dr.StartDate).Max().AddDays(7);
    }

    public bool ConfirmDepartmentRosterCreation()
    {
        Roster = new DepartmentRoster(RosterName, StartDate, UseSaturdays, UseSundays, Department);

        // Check name.
        //if (Helios.StaffReader.RosterNameExists(RosterName, Department.Name))
        if (Department.DepartmentRosters.Select(r => r.Name).Contains(RosterName))
        {
            if (MessageBox.Show($"The roster '{RosterName}' already exists for Department: {Department.Name}.\n\n" +
                                "Would you like to continue and create a duplicate roster?", "Duplicate Roster?",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return false;
        }

        Helios.StaffCreator.DepartmentRoster(Roster);
        return true;
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}