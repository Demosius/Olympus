using Pantheon.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class DepartmentRosterVM : INotifyPropertyChanged
{
    public DepartmentRoster DepartmentRoster { get; set; }
    public Helios Helios { get; set; }
    public bool IsInitialized { get; set; }
    private Dictionary<DateTime, DailyRosterVM> dailyRosterVMs = new();
    private Dictionary<int, EmployeeRosterVM> employeeRosterVMs = new();

    #region INotifyPropertyChanged Members 

    public ObservableCollection<EmployeeRosterVM> EmployeeRosters { get; set; }

    private DailyRosterVM? mondayRoster;
    public DailyRosterVM? MondayRoster
    {
        get => mondayRoster;
        set
        {
            mondayRoster = value;
            OnPropertyChanged(nameof(MondayRoster));
        }
    }

    private DailyRosterVM? tuesdayRoster;
    public DailyRosterVM? TuesdayRoster
    {
        get => tuesdayRoster;
        set
        {
            tuesdayRoster = value;
            OnPropertyChanged(nameof(TuesdayRoster));
        }
    }

    private DailyRosterVM? wednesdayRoster;
    public DailyRosterVM? WednesdayRoster
    {
        get => wednesdayRoster;
        set
        {
            wednesdayRoster = value;
            OnPropertyChanged(nameof(WednesdayRoster));

        }
    }

    private DailyRosterVM? thursdayRoster;
    public DailyRosterVM? ThursdayRoster
    {
        get => thursdayRoster;
        set
        {
            thursdayRoster = value;
            OnPropertyChanged(nameof(ThursdayRoster));
        }
    }

    private DailyRosterVM? fridayRoster;
    public DailyRosterVM? FridayRoster
    {
        get => fridayRoster;
        set
        {
            fridayRoster = value;
            OnPropertyChanged(nameof(FridayRoster));
        }
    }

    private DailyRosterVM? saturdayRoster;
    public DailyRosterVM? SaturdayRoster
    {
        get => saturdayRoster;
        set
        {
            saturdayRoster = value;
            OnPropertyChanged(nameof(SaturdayRoster));
        }
    }

    private DailyRosterVM? sundayRoster;
    public DailyRosterVM? SundayRoster
    {
        get => sundayRoster;
        set
        {
            sundayRoster = value;
            OnPropertyChanged(nameof(SundayRoster));
        }
    }

    private int employeeColWidth = 200;
    public int EmployeeColWidth
    {
        get => employeeColWidth;
        set
        {
            employeeColWidth = value;
            OnPropertyChanged(nameof(EmployeeColWidth));
        }
    }

    private int monColWidth = 150;
    public int MonColWidth
    {
        get => monColWidth;
        set
        {
            monColWidth = value;
            OnPropertyChanged(nameof(MonColWidth));
        }
    }

    private int tuesColWidth = 150;
    public int TuesColWidth
    {
        get => tuesColWidth;
        set
        {
            tuesColWidth = value;
            OnPropertyChanged(nameof(TuesColWidth));
        }
    }

    private int wedColWidth = 150;
    public int WedColWidth
    {
        get => wedColWidth;
        set
        {
            wedColWidth = value;
            OnPropertyChanged(nameof(WedColWidth));
        }
    }

    private int thursColWidth = 150;
    public int ThursColWidth
    {
        get => thursColWidth;
        set
        {
            thursColWidth = value;
            OnPropertyChanged(nameof(ThursColWidth));
        }
    }

    private int friColWidth = 150;
    public int FriColWidth
    {
        get => friColWidth;
        set
        {
            friColWidth = value;
            OnPropertyChanged(nameof(FriColWidth));
        }
    }

    private int satColWidth = 150;
    public int SatColWidth
    {
        get => satColWidth;
        set
        {
            satColWidth = value;
            OnPropertyChanged(nameof(SatColWidth));
        }
    }

    private int sunColWidth = 150;
    public int SunColWidth
    {
        get => sunColWidth;
        set
        {
            sunColWidth = value;
            OnPropertyChanged(nameof(SunColWidth));
        }
    }

    #endregion

    /// <summary>
    /// For Testing purposes.
    /// </summary>
    public DepartmentRosterVM()
    {
        DepartmentRoster = new DepartmentRoster();
        Helios = new Helios(Settings.Default.SolLocation);
        EmployeeRosters = new ObservableCollection<EmployeeRosterVM>();
        IsInitialized = true;
    }

    public DepartmentRosterVM(DepartmentRoster roster, Helios helios)
    {
        DepartmentRoster = roster;
        Helios = helios;
        EmployeeRosters = new ObservableCollection<EmployeeRosterVM>();
        IsInitialized = false;
    }

    /// <summary>
    /// Full initialization can take some time, so only call to initialize when the specific Department Roster is to be used/viewed.
    /// </summary>
    public void Initialize()
    {
        if (IsInitialized) return;

        if (!DepartmentRoster.IsLoaded) Helios.StaffReader.FillDepartmentRoster(DepartmentRoster);

        // Daily rosters.
        foreach (var dailyRoster in DepartmentRoster.DailyRosters)
        {
            var drVM = new DailyRosterVM(dailyRoster, this);
            dailyRosterVMs.Add(dailyRoster.Date, drVM);
            switch (dailyRoster.Day)
            {
                case DayOfWeek.Monday:
                    MondayRoster = drVM;
                    break;
                case DayOfWeek.Tuesday:
                    TuesdayRoster = drVM;
                    break;
                case DayOfWeek.Wednesday:
                    WednesdayRoster = drVM;
                    break;
                case DayOfWeek.Thursday:
                    ThursdayRoster = drVM;
                    break;
                case DayOfWeek.Friday:
                    FridayRoster = drVM;
                    break;
                case DayOfWeek.Saturday:
                    SaturdayRoster = drVM;
                    break;
                case DayOfWeek.Sunday:
                    SundayRoster = drVM;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dailyRoster.Day), dailyRoster.Day, "Unaccounted day of the week.");
            }
        }

        // EmployeeRosters
        foreach (var employeeRoster in DepartmentRoster.EmployeeRosters)
        {
            var erVM = new EmployeeRosterVM(employeeRoster, this);
            employeeRosterVMs.Add(employeeRoster.EmployeeID, erVM);
            foreach (var roster in employeeRoster.Rosters)
            {
                dailyRosterVMs.TryGetValue(roster.Date, out var drVM);
                if (drVM is null)
                {
                    drVM = new DailyRosterVM(new DailyRoster(DepartmentRoster.Department!, DepartmentRoster, roster.Date), this);
                    dailyRosterVMs.Add(roster.Date, drVM);
                }
                erVM.AddRoster(roster, drVM);
            }
        }

        IsInitialized = true;
    }

    public void GenerateRosterAssignments()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [Annotations.NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DepartmentRoster.ToString();

}