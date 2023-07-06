using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deimos.Models;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Extensions;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class ReportByWeekVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }
    public ErrorAllocationVM ParentVM { get; set; }
    public DeimosVM Deimos { get; set; }

    public DateTime? StartDate => ParentVM.StartDate;
    public DateTime? EndDate => ParentVM.EndDate;

    public List<WeeklyStatisticReport> ReportList { get; set; }

    #region INoptifyPropertyChanged Members

    public ObservableCollection<WeeklyStatisticReport> Reports { get; set; }

    private EErrorMethod selectedErrorMethod;
    public EErrorMethod SelectedErrorMethod
    {
        get => selectedErrorMethod;
        set
        {
            selectedErrorMethod = value;
            OnPropertyChanged();
        }
    }

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public ReportByWeekVM(ErrorAllocationVM parent)
    {
        ParentVM = parent;
        Helios = ParentVM.Helios;
        Deimos = ParentVM.ParentVM;

        ReportList = new List<WeeklyStatisticReport>();
        Reports = new ObservableCollection<WeeklyStatisticReport>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }


    public async Task RefreshDataAsync()
    {
        if (StartDate is null || EndDate is null) return;

        var fromDate = ((DateTime)StartDate).WeekStartSunday();
        var toDate = ((DateTime)EndDate).WeekEndSaturday();

        var (mispicks, sessions, tagTool) = await Helios.StaffReader.StatisticReportsAsync(fromDate, toDate, SelectedErrorMethod == EErrorMethod.ErrorDiscovered);
        await Task.Run(() =>
        {
            // Correctly identify pickers/operators for each session.
            foreach (var pickSession in sessions)
                tagTool.SetSessionPicker(pickSession);

            // Correctly identify pickers/operators for each mispick.
            foreach (var mispick in mispicks)
                tagTool.SetMispickOperator(mispick);

            ReportList.Clear();

            var weekStart = fromDate;

            while (weekStart < toDate)
            {
                var weekEnd = weekStart.AddDays(6);
                var start = weekStart;
                var weekSessions = sessions.Where(s => s.Date >= start && s.Date <= weekEnd);
                var weekMispicks = SelectedErrorMethod == EErrorMethod.ErrorMade
                    ? mispicks.Where(m => m.ErrorDate >= start && m.ErrorDate <= weekEnd)
                    : mispicks.Where(m => m.PostedDate >= start && m.PostedDate <= weekEnd);

                // Group sessions and mispicks each by operator RF and use those groups to create each line of reports.
                var sessionDict = weekSessions.GroupBy(s => (s.OperatorRF_ID, s.TechType)).ToDictionary(g => g.Key, s => s.ToList());
                var mispickDict = weekMispicks.GroupBy(m => (m.AssignedRF_ID, m.TechType)).ToDictionary(g => g.Key, g => g.ToList());

                // Get all relevant RF IDs from both pick sessions and mispicks.
                var keyTuples = sessionDict.Keys.ToList();
                keyTuples.AddRange(mispickDict.Keys);
                keyTuples = keyTuples.Distinct().ToList();

                foreach (var key in keyTuples)
                {
                    var rf = key.OperatorRF_ID;
                    var tech = key.TechType;

                    if (!sessionDict.TryGetValue(key, out var pickSessions)) pickSessions = new List<PickSession>();
                    if (!mispickDict.TryGetValue(key, out var mpList)) mpList = new List<Mispick>();

                    var picker = tagTool.EmployeeByRF(rf);
                    var newRep = new WeeklyStatisticReport(picker, rf, pickSessions, mpList,
                        weekStart, weekEnd, tech);

                    ReportList.Add(newRep);
                }

                weekStart = weekStart.AddDays(7);
            }
        });
        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
    }

    public void ApplyFilters()
    {
        var reports = ReportList.Where(report =>
            Regex.IsMatch(report.EmployeeName, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(report.RFID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(report.TechType.ToString(), FilterString, RegexOptions.IgnoreCase));

        Reports.Clear();
        foreach (var report in reports) Reports.Add(report);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}