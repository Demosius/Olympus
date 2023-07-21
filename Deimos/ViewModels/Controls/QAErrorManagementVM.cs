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
using Uranus.Staff.Models;

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
            OnPropertyChanged(nameof(BackoutPrompt));
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
    
    private int pickerErrors;
    public int PickerErrors
    {
        get => pickerErrors;
        set
        {
            pickerErrors = value;
            OnPropertyChanged();
        }
    }

    private int receivingErrors;
    public int ReceivingErrors
    {
        get => receivingErrors;
        set
        {
            receivingErrors = value;
            OnPropertyChanged();
        }
    }

    private int replenErrors;
    public int ReplenErrors
    {
        get => replenErrors;
        set
        {
            replenErrors = value;
            OnPropertyChanged();
        }
    }

    private int stockingErrors;
    public int StockingErrors
    {
        get => stockingErrors;
        set
        {
            stockingErrors = value;
            OnPropertyChanged();
        }
    }

    private int heatMapErrors;
    public int HeatMapErrors
    {
        get => heatMapErrors;
        set
        {
            heatMapErrors = value;
            OnPropertyChanged();
        }
    }

    private int qaErrors;
    public int QAErrors
    {
        get => qaErrors;
        set
        {
            qaErrors = value;
            OnPropertyChanged();
        }
    }

    private int otherDeptErrors;
    public int OtherDeptErrors
    {
        get => otherDeptErrors;
        set
        {
            otherDeptErrors = value;
            OnPropertyChanged();
        }
    }

    private int warehouseErrorCount;
    public int WarehouseErrorCount
    {
        get => warehouseErrorCount;
        set
        {
            warehouseErrorCount = value;
            OnPropertyChanged();
        }
    }

    private int systemErrors;
    public int SystemErrors
    {
        get => systemErrors;
        set
        {
            systemErrors = value;
            OnPropertyChanged();
        }
    }

    private int supplierErrors;
    public int SupplierErrors
    {
        get => supplierErrors;
        set
        {
            supplierErrors = value;
            OnPropertyChanged();
        }
    }

    private int otherExternalErrors;
    public int OtherExternalErrors
    {
        get => otherExternalErrors;
        set
        {
            otherExternalErrors = value;
            OnPropertyChanged();
        }
    }

    private int fullErrorCount;
    public int FullErrorCount
    {
        get => fullErrorCount;
        set
        {
            fullErrorCount = value;
            OnPropertyChanged();
        }
    }

    public string BackoutPrompt => $"Set {SelectedQALine?.ErrorType ?? ""} to Backout = '{!(SelectedQALine?.External ?? false)}'";

    #endregion

    #region Comnmands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public SetBackoutCommand SetBackoutCommand { get; set; }

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
        SetBackoutCommand = new SetBackoutCommand(this);
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
        PickerErrors = QAErrorLines.Count(l => l.PickerError);
        ReceivingErrors = QAErrorLines.Count(l => l.ReceiveError);
        ReplenErrors = QAErrorLines.Count(l => l.ReplenError);
        StockingErrors = QAErrorLines.Count(l => l.StockingError);
        HeatMapErrors = QAErrorLines.Count(l => l.HeatMapError);
        QAErrors = QAErrorLines.Count(l => l.QAError);
        SystemErrors = QAErrorLines.Count(l => l.SystemError);
        SupplierErrors = QAErrorLines.Count(l => l.SupplierError);

        OtherDeptErrors = QAErrorLines.Count(l => l.ErrorCategory == EErrorCategory.OtherDept);
        WarehouseErrorCount = QAErrorLines.Count(l => l.ErrorAllocation == EErrorAllocation.Warehouse);
        OtherExternalErrors = QAErrorLines.Count(l => l.ErrorCategory == EErrorCategory.OtherExternal);
        FullErrorCount = QAErrorLines.Count;
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
            Regex.IsMatch(l.CartonID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(l.BinCode, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(l.ItemDescription, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(l.PickerName, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(l.PickerRFID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(l.ItemNumber, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(l.ErrorType, FilterString, RegexOptions.IgnoreCase));

        QAErrorLines.Clear();

        foreach (var line in lines)
            QAErrorLines.Add(line);

        Count();
    }

    public async Task SetBackoutAsync()
    {
        if (SelectedQALine is null) return;
        var errorType = SelectedQALine.ErrorType;
        var blackout = SelectedQALine.External;
        
        var lines = QAErrorLines.Where(l => l.ErrorType == errorType).ToList();
        foreach (var line in lines) line.SetExternal(!blackout);
        await Helios.StaffUpdater.QALinesAsync(lines.Select(vm => vm.QALine).ToList());

        Count();
        OnPropertyChanged(nameof(BackoutPrompt));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}