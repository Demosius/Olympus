using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Deimos.Interfaces;
using Deimos.Models;
using Deimos.ViewModels.Commands;
using Morpheus;
using Morpheus.ViewModels.Controls;
using Quest.Properties;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Quest.ViewModels.Components;

public class PickRateTrackerVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public TimeSpan FromTime { get; set; }
    public TimeSpan ToTime { get; set; }

    public List<PickEvent> AllEvents { get; set; }
    public List<PickSession> AllSessions { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<EmployeeStatisticsReport> RFTStats { get; set; }
    public ObservableCollection<EmployeeStatisticsReport> PTLStats { get; set; }

    private DateTime date;
    public DateTime Date
    {
        get => date;
        set
        {
            date = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    public string FromTimeString
    {
        get => FromTime.ToString();
        set
        {
            if (TimeSpan.TryParse(value, out var time))
                FromTime = time;
            if (FromTime > ToTime)
            {
                ToTime = time;
                OnPropertyChanged(nameof(ToTime));
            }
            OnPropertyChanged();
        }
    }

    public string ToTimeString
    {
        get => ToTime.ToString();
        set
        {
            if (TimeSpan.TryParse(value, out var time))
                ToTime = time;
            if (FromTime > ToTime)
            {
                FromTime = time;
                OnPropertyChanged(nameof(FromTime));
            }
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    private PickRateTrackerVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;

        date = DateTime.Today;
        FromTime = new TimeSpan(0, 0, 0, 0);
        ToTime = new TimeSpan(0, 23, 59, 59);

        AllEvents = new List<PickEvent>();
        AllSessions = new List<PickSession>();

        RFTStats = new ObservableCollection<EmployeeStatisticsReport>();
        PTLStats = new ObservableCollection<EmployeeStatisticsReport>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public static PickRateTrackerVM CreateEmpty(Helios helios, ProgressBarVM progressBar) => new(helios, progressBar);

    private async Task<PickRateTrackerVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<PickRateTrackerVM> CreateAsync(Helios helios, ProgressBarVM progressBar)
    {
        var ret = new PickRateTrackerVM(helios, progressBar);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        ProgressBar.StartTask("Pulling Pick Event Data...");
        AllSessions = await Helios.StaffReader.PickSessionsAsync(Date, true);
        AllEvents = AllSessions.SelectMany(s => s.PickEvents).ToList();
        ProgressBar.EndTask();

        ProgressBar.StartTask("Filtering data...");
        ApplyFilters();
        ProgressBar.EndTask();
    }

    public void ClearFilters()
    {
        FromTime = new TimeSpan(0, 0, 0, 0);
        ToTime = new TimeSpan(0, 59, 59, 59);
        ApplyFilters();
        OnPropertyChanged(nameof(FromTimeString));
        OnPropertyChanged(nameof(ToTimeString));
    }

    public void ApplyFilters()
    {
        RFTStats.Clear();
        PTLStats.Clear();

        var rftStats = AllSessions.Where(s => s.WithinTimespan(FromTime, ToTime) && s.TechType == ETechType.RFT)
            .GroupBy(s => (s.Operator, s.OperatorRF_ID)).Select(g =>
                new EmployeeStatisticsReport(g.Key.Operator, g.Key.OperatorRF_ID, g.ToList(), date, date)).OrderBy(r => r.DisplayName);

        var ptlStats = AllSessions.Where(s => s.WithinTimespan(FromTime, ToTime) && s.TechType == ETechType.PTL)
            .GroupBy(s => (s.Operator, s.OperatorRF_ID)).Select(g =>
                new EmployeeStatisticsReport(g.Key.Operator, g.Key.OperatorRF_ID, g.ToList(), date, date)).OrderBy(r => r.DisplayName);

        foreach (var ptlStat in ptlStats)
            PTLStats.Add(ptlStat);

        foreach (var rftStat in rftStats)
            RFTStats.Add(rftStat);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}