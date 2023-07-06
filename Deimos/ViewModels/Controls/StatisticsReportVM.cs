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
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class StatisticsReportVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }
    public DeimosVM Deimos { get; set; }
    public ErrorAllocationVM ParentVM { get; set; }

    public DateTime? StartDate => ParentVM.StartDate;
    public DateTime? EndDate => ParentVM.EndDate;

    public List<EmployeeStatisticsReport> ReportList { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<EmployeeStatisticsReport> Reports { get; set; }

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

    private ETechType selectedTechType;
    public ETechType SelectedTechType
    {
        get => selectedTechType;
        set
        {
            selectedTechType = value;
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

    public StatisticsReportVM(ErrorAllocationVM parentVM)
    {
        ParentVM = parentVM;
        Helios = ParentVM.Helios;
        Deimos = ParentVM.ParentVM;
        
        ReportList = new List<EmployeeStatisticsReport>();
        Reports = new ObservableCollection<EmployeeStatisticsReport>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (StartDate is null || EndDate is null) return;

        var fromDate = (DateTime)StartDate;
        var toDate = (DateTime)EndDate;

        var (mispicks, sessions, tagTool) = await Helios.StaffReader.StatisticReportsAsync(fromDate, toDate, SelectedErrorMethod == EErrorMethod.ErrorDiscovered);
        await Task.Run(() =>
        {
            // Make sure that we are getting the correct tech.
            if (SelectedTechType != ETechType.All)
            {
                sessions = sessions.Where(s => s.TechType == SelectedTechType).ToList();
                mispicks = mispicks.Where(m => m.TechType == SelectedTechType).ToList();
            }

            // Correctly identify pickers/operators for each session.
            foreach (var pickSession in sessions)
                tagTool.SetSessionPicker(pickSession);

            // Correctly identify pickers/operators for each mispick.
            foreach (var mispick in mispicks)
                tagTool.SetMispickOperator(mispick);

            ReportList.Clear();

            // Group sessions and mispicks each by operator RF and use those groups to create each line of reports.
            var sessionDict = sessions.GroupBy(s => s.OperatorRF_ID).ToDictionary(g => g.Key, s => s.ToList());
            var mispickDict = mispicks.GroupBy(m => m.AssignedRF_ID).ToDictionary(g => g.Key, g => g.ToList());

            // Get all relevant RF IDs from both pick sessions and mispicks.
            var rfIDs = sessionDict.Keys.Select(k => k.ToString()).ToList();
            rfIDs.AddRange(mispickDict.Keys);
            rfIDs = rfIDs.Distinct().ToList();

            foreach (var rf in rfIDs)
            {
                if (!sessionDict.TryGetValue(rf, out var pickSessions)) pickSessions = new List<PickSession>();
                if (!mispickDict.TryGetValue(rf, out var mpList)) mpList = new List<Mispick>();

                var picker = tagTool.EmployeeByRF(rf);
                var newRep = new EmployeeStatisticsReport(picker, rf, pickSessions, mpList,
                    fromDate, toDate);

                ReportList.Add(newRep);
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