using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cadmus.Interfaces;
using Cadmus.ViewModels.Commands;
using Deimos.Interfaces;
using Deimos.ViewModels.Commands;
using Morpheus.Helpers;
using Morpheus.ViewModels.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Extensions;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class QAWeeklyStatsVM : INotifyPropertyChanged, IDBInteraction, IRecalculate, IExport
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public DateTime EarliestDate { get; set; }

    public bool CanExport => true;

    #region INotifyPropertyChanged Members

    public ObservableCollection<DateTime> Weeks { get; set; }

    private DateTime? firstWeek;
    public DateTime? FirstWeek
    {
        get => firstWeek;
        set
        {
            firstWeek = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private DateTime? secondWeek;
    public DateTime? SecondWeek
    {
        get => secondWeek;
        set
        {
            secondWeek = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private QAStatsVM week1Stats;
    public QAStatsVM Week1Stats
    {
        get => week1Stats;
        set
        {
            week1Stats = value;
            OnPropertyChanged();
        }
    }

    private QAStatsVM week2Stats;
    public QAStatsVM Week2Stats
    {
        get => week2Stats;
        set
        {
            week2Stats = value;
            OnPropertyChanged();
        }
    }


    private QAStatDiffVM statDiff;
    public QAStatDiffVM StatDiff
    {
        get => statDiff;
        set
        {
            statDiff = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RecalculateCommand RecalculateCommand { get; set; }
    public ExportToCSVCommand ExportToCSVCommand { get; set; }
    public ExportToExcelCommand ExportToExcelCommand { get; set; }
    public ExportToLabelsCommand ExportToLabelsCommand { get; set; }
    public ExportToPDFCommand ExportToPDFCommand { get; set; }

    #endregion

    private QAWeeklyStatsVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;
        Weeks = new ObservableCollection<DateTime>();
        EarliestDate = DateTime.Today.Monday();

        week1Stats = new QAStatsVM(new QAStats(DateTime.Today, DateTime.Today), Helios);
        week2Stats = new QAStatsVM(new QAStats(DateTime.Today, DateTime.Today), Helios);

        statDiff = new QAStatDiffVM(week1Stats, Week2Stats);

        RefreshDataCommand = new RefreshDataCommand(this);
        RecalculateCommand = new RecalculateCommand(this);
        ExportToCSVCommand = new ExportToCSVCommand(this);
        ExportToExcelCommand = new ExportToExcelCommand(this);
        ExportToLabelsCommand = new ExportToLabelsCommand(this);
        ExportToPDFCommand = new ExportToPDFCommand(this);
    }

    private async Task<QAWeeklyStatsVM> InitializeAsync()
    {
        await SetWeeks();
        return this;
    }

    public static Task<QAWeeklyStatsVM> CreateAsync(Helios helios, ProgressBarVM progressBar)
    {
        var ret = new QAWeeklyStatsVM(helios, progressBar);
        return ret.InitializeAsync();
    }

    public static QAWeeklyStatsVM CreateEmpty(Helios helios, ProgressBarVM progressBar) => new(helios, progressBar);

    public async Task SetWeeks(DateTime? earliestDate = null)
    {
        if (earliestDate == null)
            EarliestDate = (await Helios.StaffReader.EarliestDataDateAsync()).WeekStartSunday();
        else
            EarliestDate = (DateTime) earliestDate;

        Weeks.Clear();

        var date = EarliestDate;
        var monday = DateTime.Today.Monday();
        while (date <= monday)
        {
            Weeks.Add(date);
            date = date.AddDays(7);
        }
    }

    public async Task RefreshDataAsync()
    { 
        var eDate = (await Helios.StaffReader.EarliestDataDateAsync()).WeekStartSunday();
        if (eDate != EarliestDate) await SetWeeks(eDate);

        if (FirstWeek is not null && Week1Stats.StartDate != (DateTime) FirstWeek)
        {
            var fromDate = (DateTime) FirstWeek;
            var toDate = fromDate.AddDays(6);
            ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{fromDate:dd-MMM-yyyy} - {toDate:dd-MMM-yyyy}");
            Week1Stats = new QAStatsVM(await Helios.StaffReader.QAStatsAsync(fromDate, toDate, dateDescription: $"{fromDate.EBFiscalWeekStringFull()}"), Helios);
            StatDiff.Stats1 = Week1Stats;
            ProgressBar.EndTask();
        }

        if (SecondWeek is not null && Week2Stats.StartDate != (DateTime) SecondWeek)
        {
            var fromDate = (DateTime) SecondWeek;
            var toDate = fromDate.AddDays(6);
            ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{fromDate:dd-MMM-yyyy} - {toDate:dd-MMM-yyyy}");
            Week2Stats = new QAStatsVM(await Helios.StaffReader.QAStatsAsync(fromDate, toDate, dateDescription: $"{fromDate.EBFiscalWeekStringFull()}"), Helios);
            StatDiff.Stats2 = Week2Stats;
            ProgressBar.EndTask();
        }
    }

    public async Task Recalculate(object? parameter)
    {
        if (parameter is not string str) return;
        if (!int.TryParse(str, out int num)) return;

        var eDate = (await Helios.StaffReader.EarliestDataDateAsync()).WeekStartSunday();
        if (eDate != EarliestDate) await SetWeeks(eDate);

        switch (num)
        {
            case 1:
                ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{Week1Stats.StartDate:dd-MMM-yyyy} - {Week1Stats.EndDate:dd-MMM-yyyy}");
                var newStats = await Helios.StaffReader.QAStatsAsync(Week1Stats.StartDate, Week1Stats.EndDate, true,
                    $"{Week1Stats.StartDate.EBFiscalWeekStringFull()}");
                newStats.Notes = Week1Stats.Notes;
                newStats.TargetAccuracy = Week1Stats.TargetAccuracy;
                newStats.DevelopmentOpportunity = Week1Stats.DevelopmentOpportunity;
                Week1Stats = new QAStatsVM(newStats, Helios);
                StatDiff.Stats1 = Week1Stats;
                ProgressBar.EndTask();
                break;
            case 2:
                ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{Week2Stats.StartDate:dd-MMM-yyyy} - {Week2Stats.EndDate:dd-MMM-yyyy}");
                newStats = await Helios.StaffReader.QAStatsAsync(Week2Stats.StartDate, Week2Stats.EndDate, true,
                    $"{Week2Stats.StartDate.EBFiscalWeekStringFull()}");
                newStats.Notes = Week2Stats.Notes;
                newStats.TargetAccuracy = Week2Stats.TargetAccuracy;
                newStats.DevelopmentOpportunity = Week2Stats.DevelopmentOpportunity;
                Week2Stats = new QAStatsVM(newStats, Helios);
                StatDiff.Stats2 = Week2Stats;
                ProgressBar.EndTask();
                break;
            default: return;
        }
    }

    public async Task GenerateAllAsync()
    {
        ProgressBar.StartTask("Generating Missing Weekly QA Stats...");
        await Helios.StaffCreator.QAStatsAsync(EarliestDate, EDatePeriod.Week);
        ProgressBar.EndTask();
    }

    public async Task<List<QAStats>> GetAllQAStatsAsync()
    {
        ProgressBar.StartTask("Pulling Weekly QA Stats...");
        var stats = await Helios.StaffReader.QAStatListAsync(EDatePeriod.Week);
        ProgressBar.EndTask();
        return stats;
    }

    public Task ExportToPDF()
    {
        throw new NotImplementedException();
    }

    public async Task ExportToCSV()
    {
        await GenerateAllAsync();
        var stats = await GetAllQAStatsAsync();

        await Task.Run(() =>
        {
            Output.DataTableToCSV(QAStats.GetDataTable(stats), "QAWeeklyStats.csv");
        });
    }

    public async Task ExportToExcel()
    {
        await GenerateAllAsync();
        var stats = await GetAllQAStatsAsync();

        await Task.Run(() =>
        {
            Output.DataTableToExcel(QAStats.GetDataTable(stats), "QAWeeklyStats.xlsx");
        });
    }

    public Task ExportToLabels()
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