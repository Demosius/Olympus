using Pantheon.Annotations;
using Styx;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.ViewModel.Commands;
using Uranus;
using Uranus.Commands;
using Uranus.Extension;
using Uranus.Interfaces;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel;

internal class RosterCreationVM : INotifyPropertyChanged, IDBInteraction
{
    public Department? Department { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

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
            RosterName = value.FiscalWeek();
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

    public RosterCreationVM()
    {
        startDate = DateTime.Today.AddDays(DayOfWeek.Monday - DateTime.Today.DayOfWeek + 7);
        rosterName = startDate.FiscalWeek();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ConfirmDepartmentRosterCreationCommand = new ConfirmDepartmentRosterCreationCommand(this);
    }

    public void SetDataSources(Department department, Helios helios, Charon charon)
    {
        Department = department;
        Helios = helios;
        Charon = charon;

        RefreshData();
    }

    public void RefreshData()
    {
        if (Department is null) throw new DataException("Department not set in RosterCreation.");
        
        if (Department.DepartmentRosters.Any())
            StartDate = Department.DepartmentRosters.Select(dr => dr.StartDate).Max().AddDays(7);
    }

    public void ConfirmDepartmentRosterCreation()
    {
        if (Helios is null) return;
        if (Department is null) throw new DataException("Department not set in RosterCreation.");

        Roster = new DepartmentRoster(RosterName, StartDate, UseSaturdays, UseSundays, Department);
        
        Helios.StaffCreator.DepartmentRoster(Roster);
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