using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Deimos.Models;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Serilog;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class ErrorAllocationVM : INotifyPropertyChanged, IDBInteraction, IRun
{
    public DeimosVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public PickHistoryVM PickHistoryVM { get; }
    public MispickDataVM MispickDataVM { get; }
    public ErrorsMadeVM ErrorsMadeVM { get; }
    public ErrorsDiscoveredVM ErrorsDiscoveredVM { get; set; }
    public StatisticsReportVM StatisticsReportVM { get; set; }
    public ReportByWeekVM ReportByWeekVM { get; set; }

    #region INotifyPropertyChanged Members

    public bool CanRun => StartDate is not null && EndDate is not null;


    public ObservableCollection<DataDateComparison> DataComps { get; set; }

    private DataDateComparison? selectedDateComp;
    public DataDateComparison? SelectedDateComp
    {
        get => selectedDateComp;
        set
        {
            selectedDateComp = value;
            OnPropertyChanged();
            PickHistoryVM.Date = selectedDateComp?.Date;
        }
    }

    private DateTime? startDate;
    public DateTime? StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            if (startDate is not null && (endDate < startDate || endDate is null))
            {
                endDate = ((DateTime)startDate).AddDays(6);
                OnPropertyChanged(nameof(endDate));
            }
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private DateTime? endDate;
    public DateTime? EndDate
    {
        get => endDate;
        set
        {
            endDate = value;
            if (startDate > endDate)
            {
                startDate = value;
                OnPropertyChanged(nameof(startDate));
            }
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private int dateRange;
    public int DateRange
    {
        get => dateRange;
        set
        {
            dateRange = value;
            OnPropertyChanged();
        }
    }

    private int datesWithoutData;
    public int DatesWithoutData
    {
        get => datesWithoutData;
        set
        {
            datesWithoutData = value;
            OnPropertyChanged();
        }
    }

    private int datesWithoutEvents;
    public int DatesWithoutEvents
    {
        get => datesWithoutEvents;
        set
        {
            datesWithoutEvents = value;
            OnPropertyChanged();
        }
    }

    private int datesWithoutMispicks;
    public int DatesWithoutMispicks
    {
        get => datesWithoutMispicks;
        set
        {
            datesWithoutMispicks = value;
            OnPropertyChanged();
        }
    }

    private bool overwrite;
    public bool Overwrite
    {
        get => overwrite;
        set
        {
            overwrite = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RunCommand RunCommand { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public ErrorAllocationVM(DeimosVM parentVM)
    {
        ParentVM = parentVM;
        Helios = ParentVM.Helios;
        ProgressBar = ParentVM.ProgressBar;

        DataComps = new ObservableCollection<DataDateComparison>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RunCommand = new RunCommand(this);

        PickHistoryVM = new PickHistoryVM(this);
        MispickDataVM = new MispickDataVM(this);
        ErrorsMadeVM = new ErrorsMadeVM(this);
        ErrorsDiscoveredVM = new ErrorsDiscoveredVM(this);
        StatisticsReportVM = new StatisticsReportVM(this);
        ReportByWeekVM = new ReportByWeekVM(this);

    }

    public async Task RefreshDataAsync()
    {
        DataComps.Clear();

        if (StartDate is null || EndDate is null)
        {
            SetDateData();
            return;
        }

        Mouse.OverrideCursor = Cursors.Wait;

        var eventTask = Helios.StaffReader.PickEventLineCountByDate((DateTime)StartDate, (DateTime)EndDate);
        var mispickTask = Helios.StaffReader.MispickLineCountByDate((DateTime)StartDate, (DateTime)EndDate);

        await Task.WhenAll(eventTask, mispickTask);

        var events = await eventTask.ConfigureAwait(false);
        var mispicks = await mispickTask.ConfigureAwait(false);

        var comps = DataDateComparison.GetDateComparisons(events, mispicks).OrderBy(d => d.Date);

        foreach (var comp in comps)
            DataComps.Add(comp);

        SetDateData();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    private void SetDateData()
    {
        if (StartDate is null || EndDate is null)
        {
            DateRange = 0;
            DatesWithoutData = 0;
            DatesWithoutEvents = 0;
            DatesWithoutMispicks = 0;
            return;
        }

        var sd = (DateTime)StartDate;
        var ed = (DateTime)EndDate;

        DateRange = ed.Subtract(sd).Days + 1;

        DatesWithoutData = DateRange - DataComps.Count;
        DatesWithoutEvents = DataComps.Count(d => !d.HasPickEvents);
        DatesWithoutMispicks = DataComps.Count(d => !d.HasMispicks);
    }

    public async Task Run()
    {
        if (StartDate is null || EndDate is null)
        {
            MessageBox.Show("Please ensure dates are set before running assignment tool.", "No Dates",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Mouse.OverrideCursor = Cursors.Wait;
        var errorTool = new ErrorAssignmentTool(Helios, (DateTime)StartDate, (DateTime)EndDate, Overwrite);

        var sw = new Stopwatch();
        sw.Start();
        var progress = ProgressBar.StartTask("Assigning errors...");
        bool success;
        try
        {
            success = await Task.Run(async () => await errorTool.AssignErrorsAsync(progress));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to assign errors.");
            MessageBox.Show($"Failed to complete error assignment.\n\nUnknown Error Occurred\n\n{ex}", "Failure", MessageBoxButton.OK,
                MessageBoxImage.Error);
            ProgressBar.EndTask();
            return;
        }
        ProgressBar.EndTask();
        sw.Stop();

        if (success)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MessageBox.Show("Error assignment complete.\n\n" +
                            $"Assigned: {errorTool.AssignedCount} - {errorTool.AssignedUnits}\n" +
                            $"Unassigned: {errorTool.UnassignedCount} - {errorTool.UnassignedUnits}\n\n" +
                            $"Time: {sw.Elapsed}",
                "Success", MessageBoxButton.OK);
        }
        else
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MessageBox.Show("Failed to complete error assignment.", "Failure", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}