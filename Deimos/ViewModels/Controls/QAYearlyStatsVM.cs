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

public class QAYearlyStatsVM : INotifyPropertyChanged, IDBInteraction, IRecalculate, IExport
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public DateTime EarliestDate { get; set; }

    public bool CanExport => true;

    #region INotifyPropertyChanged Members

    public ObservableCollection<DateTime> Years { get; set; }

    private DateTime? firstYear;
    public DateTime? FirstYear
    {
        get => firstYear;
        set
        {
            firstYear = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private DateTime? secondYear;
    public DateTime? SecondYear
    {
        get => secondYear;
        set
        {
            secondYear = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private QAStatsVM week1Stats;
    public QAStatsVM Year1Stats
    {
        get => week1Stats;
        set
        {
            week1Stats = value;
            OnPropertyChanged();
        }
    }

    private QAStatsVM week2Stats;
    public QAStatsVM Year2Stats
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

    private QAYearlyStatsVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;
        Years = new ObservableCollection<DateTime>();
        EarliestDate = DateTime.Today.Monday();

        week1Stats = new QAStatsVM(new QAStats(DateTime.Today, DateTime.Today), Helios);
        week2Stats = new QAStatsVM(new QAStats(DateTime.Today, DateTime.Today), Helios);

        statDiff = new QAStatDiffVM(week1Stats, Year2Stats);

        RefreshDataCommand = new RefreshDataCommand(this);
        RecalculateCommand = new RecalculateCommand(this);
        ExportToCSVCommand = new ExportToCSVCommand(this);
        ExportToExcelCommand = new ExportToExcelCommand(this);
        ExportToLabelsCommand = new ExportToLabelsCommand(this);
        ExportToPDFCommand = new ExportToPDFCommand(this);
    }

    private async Task<QAYearlyStatsVM> InitializeAsync()
    {
        await SetYears();
        return this;
    }

    public static Task<QAYearlyStatsVM> CreateAsync(Helios helios, ProgressBarVM progressBar)
    {
        var ret = new QAYearlyStatsVM(helios, progressBar);
        return ret.InitializeAsync();
    }

    public static QAYearlyStatsVM CreateEmpty(Helios helios, ProgressBarVM progressBar) => new(helios, progressBar);

    public async Task SetYears(DateTime? earliestDate = null)
    {
        if (earliestDate == null)
            EarliestDate = (await Helios.StaffReader.EarliestDataDateAsync()).EBFiscalYearStart();
        else
            EarliestDate = (DateTime)earliestDate;

        Years.Clear();

        var date = EarliestDate;
        var today = DateTime.Today;
        while (date <= today)
        {
            Years.Add(date);
            date = date.EBFiscalYearEnd().AddDays(1);
        }
    }

    public async Task RefreshDataAsync()
    {
        var eDate = (await Helios.StaffReader.EarliestDataDateAsync()).EBFiscalYearStart();
        if (eDate != EarliestDate) await SetYears(eDate);

        if (FirstYear is not null && Year1Stats.StartDate != (DateTime)FirstYear)
        {
            var fromDate = (DateTime)FirstYear;
            var toDate = fromDate.EBFiscalYearEnd();
            ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{fromDate:dd-MMM-yyyy} - {toDate:dd-MMM-yyyy}");
            Year1Stats = new QAStatsVM(await Helios.StaffReader.QAStatsAsync(fromDate, toDate, dateDescription: $"{fromDate.EBFiscalYear()}"), Helios);
            StatDiff.Stats1 = Year1Stats;
            ProgressBar.EndTask();
        }

        if (SecondYear is not null && Year2Stats.StartDate != (DateTime)SecondYear)
        {
            var fromDate = (DateTime)SecondYear;
            var toDate = fromDate.EBFiscalYearEnd();
            ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{fromDate:dd-MMM-yyyy} - {toDate:dd-MMM-yyyy}");
            Year2Stats = new QAStatsVM(await Helios.StaffReader.QAStatsAsync(fromDate, toDate, dateDescription: $"{fromDate.EBFiscalYear()}"), Helios);
            StatDiff.Stats2 = Year2Stats;
            ProgressBar.EndTask();
        }
    }

    public async Task Recalculate(object? parameter)
    {
        if (parameter is not string str) return;
        if (!int.TryParse(str, out int num)) return;

        var eDate = (await Helios.StaffReader.EarliestDataDateAsync()).EBFiscalYearStart();
        if (eDate != EarliestDate) await SetYears(eDate);

        switch (num)
        {
            case 1:
                ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{Year1Stats.StartDate:dd-MMM-yyyy} - {Year1Stats.EndDate:dd-MMM-yyyy}");
                var newStats = await Helios.StaffReader.QAStatsAsync(Year1Stats.StartDate, Year1Stats.EndDate, true,
                    $"{Year1Stats.StartDate.EBFiscalYear()}");
                newStats.Notes = Year1Stats.Notes;
                newStats.TargetAccuracy = Year1Stats.TargetAccuracy;
                newStats.DevelopmentOpportunity = Year1Stats.DevelopmentOpportunity;
                Year1Stats = new QAStatsVM(newStats, Helios);
                StatDiff.Stats1 = Year1Stats;
                ProgressBar.EndTask();
                break;
            case 2:
                ProgressBar.StartTask("Pulling/Calculating QA Stats:", $"{Year2Stats.StartDate:dd-MMM-yyyy} - {Year2Stats.EndDate:dd-MMM-yyyy}");
                newStats = await Helios.StaffReader.QAStatsAsync(Year2Stats.StartDate, Year2Stats.EndDate, true,
                    $"{Year2Stats.StartDate.EBFiscalYear()}");
                newStats.Notes = Year2Stats.Notes;
                newStats.TargetAccuracy = Year2Stats.TargetAccuracy;
                newStats.DevelopmentOpportunity = Year2Stats.DevelopmentOpportunity;
                Year2Stats = new QAStatsVM(newStats, Helios);
                StatDiff.Stats2 = Year2Stats;
                ProgressBar.EndTask();
                break;
            default: return;
        }
    }

    public async Task GenerateAllAsync()
    {
        ProgressBar.StartTask("Generating Missing Yearly QA Stats...");
        await Helios.StaffCreator.QAStatsAsync(EarliestDate, EDatePeriod.Year);
        ProgressBar.EndTask();
    }

    public async Task<List<QAStats>> GetAllQAStatsAsync()
    {
        ProgressBar.StartTask("Pulling Yearly QA Stats...");
        var stats = await Helios.StaffReader.QAStatListAsync(EDatePeriod.Year);
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
            Output.DataTableToCSV(QAStats.GetDataTable(stats), "QAYearlyStats.csv");
        });
    }

    public async Task ExportToExcel()
    {
        await GenerateAllAsync();
        var stats = await GetAllQAStatsAsync();

        await Task.Run(() =>
        {
            Output.DataTableToExcel(QAStats.GetDataTable(stats), "QAYearlyStats.xlsx");
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