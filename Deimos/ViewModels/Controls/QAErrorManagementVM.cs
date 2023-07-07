using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deimos.ViewModels.Commands;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Serilog;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class QAErrorManagementVM : INotifyPropertyChanged, IDBInteraction, IFilters, IRefreshingControl
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public List<QALineVM> AllQAErrorLines { get; set; }

    #region INotifyPropertChanged Members

    public ObservableCollection<QALineVM> QAErrorLines { get; set; }

    private QALineVM? selectedQALine;
    public QALineVM? SelectedQALine
    {
        get => selectedQALine;
        set
        {
            selectedQALine = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BlackoutPrompt));
        }
    }

    private DateTime? startDate;
    public DateTime? StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            if (startDate is not null)
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

    private bool? blackoutFilter;
    public bool? BlackoutFilter
    {
        get => blackoutFilter;
        set
        {
            blackoutFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private int atFault;
    public int AtFault
    {
        get => atFault;
        set
        {
            atFault = value;
            OnPropertyChanged();
        }
    }

    private int notAtFault;
    public int NotAtFault
    {
        get => notAtFault;
        set
        {
            notAtFault = value;
            OnPropertyChanged();
        }
    }

    private int errorCount;
    public int ErrorCount
    {
        get => errorCount;
        set
        {
            errorCount = value;
            OnPropertyChanged();
        }
    }

    private int blackouts;
    public int Blackouts
    {
        get => blackouts;
        set
        {
            blackouts = value;
            OnPropertyChanged();
        }
    }

    private int fullCount;
    public int FullCount
    {
        get => fullCount;
        set
        {
            fullCount = value;
            OnPropertyChanged();
        }
    }

    public string BlackoutPrompt => $"Set {SelectedQALine?.ErrorType ?? ""} to Blackout = '{!(SelectedQALine?.Blackout ?? false)}'";

    #endregion

    #region Comnmands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public SetBlackoutCommand SetBlackoutCommand { get; set; }

    #endregion

    public QAErrorManagementVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;

        filterString = string.Empty;

        AllQAErrorLines = new List<QALineVM>();
        QAErrorLines = new ObservableCollection<QALineVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        SetBlackoutCommand = new SetBlackoutCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (StartDate is null || EndDate is null || StartDate > EndDate) return;
        var fromDate = (DateTime)StartDate;
        var toDate = (DateTime)EndDate;

        ProgressBar.StartTask("Pulling QA Error lines.", $"{StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}");

        try
        {
            AllQAErrorLines = (await Helios.StaffReader.QAErrorLines(fromDate, toDate)).Select(l => new QALineVM(l, this)).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull QA Error Lines and children.");
            throw;
        }

        ProgressBar.EndTask();

        ApplyFilters();
    }

    public void Count()
    {
        AtFault = QAErrorLines.Count(l => l.AtFault);
        FullCount = QAErrorLines.Count;
        NotAtFault = FullCount - AtFault;
        Blackouts = QAErrorLines.Count(l => l.Blackout);
        ErrorCount = FullCount - Blackouts;
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        OnPropertyChanged(nameof(FilterString));
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var lines = AllQAErrorLines.Where(l =>
            (BlackoutFilter is null || l.Blackout == BlackoutFilter) &&
            (Regex.IsMatch(l.CartonID, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(l.BinCode, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(l.ItemDescription, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(l.PickerName, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(l.PickerRFID, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(l.ItemNumber, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(l.ErrorType, FilterString, RegexOptions.IgnoreCase)));

        QAErrorLines.Clear();

        foreach (var line in lines)
            QAErrorLines.Add(line);

        Count();
    }

    public async Task SetBlackoutAsync()
    {
        if (SelectedQALine is null) return;
        var errorType = SelectedQALine.ErrorType;
        var blackout = SelectedQALine.Blackout;
        
        var lines = QAErrorLines.Where(l => l.ErrorType == errorType).ToList();
        foreach (var line in lines) line.SetBlackOut(!blackout);
        await Helios.StaffUpdater.QALinesAsync(lines.Select(vm => vm.QALine).ToList());

        Count();
        OnPropertyChanged(nameof(BlackoutPrompt));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}