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

public class QAMonthlyStatsVM : INotifyPropertyChanged, IDBInteraction, IRecalculate, IExport
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public DateTime EarliestDate { get; set; }

    public bool CanExport => true;

    #region INotifyPropertyChanged Members

    public ObservableCollection<DateTime> Months { get; set; }

    private DateTime? firstMonth;
    public DateTime? FirstMonth
    {
        get => firstMonth;
        set
        {
            firstMonth = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private DateTime? secondMonth;
    public DateTime? SecondMonth
    {
        get => secondMonth;
        set
        {
            secondMonth = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private QAStatsVM week1Stats;
    public QAStatsVM Month1Stats
    {
        get => week1Stats;
        set
        {
            week1Stats = value;
            OnPropertyChanged();
        }
    }

    private QAStatsVM week2Stats;
    public QAStatsVM Month2Stats
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

    private QAMonthlyStatsVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;
        Months = new ObservableCollection<DateTime>();
        EarliestDate = DateTime.Today.Monday();

        week1Stats = new QAStatsVM(new QAStats(DateTime.Today, DateTime.Today), Helios);
        week2Stats = new QAStatsVM(new QAStats(DateTime.Today, DateTime.Today), Helios);

        statDiff = new QAStatDiffVM(week1Stats, Month2Stats);

        RefreshDataCommand = new RefreshDataCommand(this);
        RecalculateCommand = new RecalculateCommand(this);
        ExportToCSVCommand = new ExportToCSVCommand(this);
        ExportToExcelCommand = new ExportToExcelCommand(this);
        ExportToLabelsCommand = new ExportToLabelsCommand(this);
        ExportToPDFCommand = new ExportToPDFCommand(this);
    }

    private async Task<QAMonthlyStatsVM> InitializeAsync()
    {
        await SetMonths();
        return this;
    }

    public static Task<QAMonthlyStatsVM> CreateAsync(Helios helios, ProgressBarVM progressBar)
    {
        var ret = new QAMonthlyStatsVM(helios, progressBar);
        return ret.InitializeAsync();
    }

    public static QAMonthlyStatsVM CreateEmpty(Helios helios, ProgressBarVM progressBar) => new(helios, progressBar);

    public async Task SetMonths(DateTime? earliestDate = null)
    {
        if (earliestDate == null)
            EarliestDate = (await Helios.StaffReader.EarliestDataDateAsync()).EBFiscalMonthStart();
        else
            EarliestDate = (DateTime)earliestDate;

        Months.Clear();

        var date = EarliestDate;
        var today = DateTime.Today;
        while (date <= today)
        {
            Months.Add(date);
            date = date.EBFiscalMonthEnd().AddDays(1);
        }
    }

    public async Task RefreshDataAsync()
    {
        var eDate = (await Helios.StaffReader.EarliestDataDateAsync()).EBFiscalMonthStart();
        if (eDate != EarliestDate) await SetMonths(eDate);

        if (FirstMonth is not null && Month1Stats.StartDate != (DateTime)FirstMonth)
        {
            var fromDate = (DateTime)FirstMonth;
            var toDate = fromDate.EBFiscalMonthEnd();
            ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{fromDate:dd-MMM-yyyy} - {toDate:dd-MMM-yyyy}");
            Month1Stats = new QAStatsVM(await Helios.StaffReader.QAStatsAsync(fromDate, toDate, dateDescription: $"{fromDate.EBFiscalMonthStringFull()}"), Helios);
            StatDiff.Stats1 = Month1Stats;
            ProgressBar.EndTask();
        }

        if (SecondMonth is not null && Month2Stats.StartDate != (DateTime)SecondMonth)
        {
            var fromDate = (DateTime)SecondMonth;
            var toDate = fromDate.EBFiscalMonthEnd();
            ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{fromDate:dd-MMM-yyyy} - {toDate:dd-MMM-yyyy}");
            Month2Stats = new QAStatsVM(await Helios.StaffReader.QAStatsAsync(fromDate, toDate, dateDescription: $"{fromDate.EBFiscalMonthStringFull()}"), Helios);
            StatDiff.Stats2 = Month2Stats;
            ProgressBar.EndTask();
        }
    }

    public async Task Recalculate(object? parameter)
    {
        if (parameter is not string str) return;
        if (!int.TryParse(str, out int num)) return;

        var eDate = (await Helios.StaffReader.EarliestDataDateAsync()).EBFiscalMonthStart();
        if (eDate != EarliestDate) await SetMonths(eDate);

        switch (num)
        {
            case 1:
                ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{Month1Stats.StartDate:dd-MMM-yyyy} - {Month1Stats.EndDate:dd-MMM-yyyy}");
                var newStats = await Helios.StaffReader.QAStatsAsync(Month1Stats.StartDate, Month1Stats.EndDate, true,
                    $"{Month1Stats.StartDate.EBFiscalMonthStringFull()}");
                newStats.Notes = Month1Stats.Notes;
                newStats.TargetAccuracy = Month1Stats.TargetAccuracy;
                newStats.DevelopmentOpportunity = Month1Stats.DevelopmentOpportunity;
                Month1Stats = new QAStatsVM(newStats, Helios);
                StatDiff.Stats1 = Month1Stats;
                ProgressBar.EndTask();
                break;
            case 2:
                ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{Month2Stats.StartDate:dd-MMM-yyyy} - {Month2Stats.EndDate:dd-MMM-yyyy}");
                newStats = await Helios.StaffReader.QAStatsAsync(Month2Stats.StartDate, Month2Stats.EndDate, true,
                    $"{Month2Stats.StartDate.EBFiscalMonthStringFull()}");
                newStats.Notes = Month2Stats.Notes;
                newStats.TargetAccuracy = Month2Stats.TargetAccuracy;
                newStats.DevelopmentOpportunity = Month2Stats.DevelopmentOpportunity;
                Month2Stats = new QAStatsVM(newStats, Helios);
                StatDiff.Stats2 = Month2Stats;
                ProgressBar.EndTask();
                break;
            default: return;
        }
    }

    public async Task GenerateAllAsync()
    {
        ProgressBar.StartTask("Generating Missing Monthly QA Stats...");
        await Helios.StaffCreator.QAStatsAsync(EarliestDate, EDatePeriod.Month);
        ProgressBar.EndTask();
    }

    public async Task<List<QAStats>> GetAllQAStatsAsync()
    {
        ProgressBar.StartTask("Pulling Monthly QA Stats...");
        var stats = await Helios.StaffReader.QAStatListAsync(EDatePeriod.Month);
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
            Output.DataTableToCSV(QAStats.GetDataTable(stats), "QAMonthlyStats.csv");
        });
    }

    public async Task ExportToExcel()
    {
        await GenerateAllAsync();
        var stats = await GetAllQAStatsAsync();

        await Task.Run(() =>
        {
            Output.DataTableToExcel(QAStats.GetDataTable(stats), "QAMonthlyStats.xlsx");
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