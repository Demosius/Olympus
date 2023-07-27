using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Morpheus.ViewModels.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class QAOperatorStatsVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM Deimos { get; set; }
    public QAToolVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public List<QAOperatorVM> AllOperators { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<QAOperatorVM> Operators { get; set; }

    private QAOperatorVM selectedOperator;
    public QAOperatorVM SelectedOperator
    {
        get => selectedOperator;
        set
        {
            selectedOperator = value;
            OnPropertyChanged();
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

    private TimeSpan minTimeFilter;
    public string MinTimeFilter
    {
        get => minTimeFilter.ToString("g");
        set
        {
            _ = TimeSpan.TryParse(value, out minTimeFilter);
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private double bestCartonsPerHour;
    public double BestCartonsPerHour
    {
        get => bestCartonsPerHour;
        set
        {
            bestCartonsPerHour = value;
            OnPropertyChanged();
        }
    }

    private double bestItemsPerMinute;
    public double BestItemsPerMinute
    {
        get => bestItemsPerMinute;
        set
        {
            bestItemsPerMinute = value;
            OnPropertyChanged();
        }
    }

    private double bestScansPerMinute;
    public double BestScansPerMinute
    {
        get => bestScansPerMinute;
        set
        {
            bestScansPerMinute = value;
            OnPropertyChanged();
        }
    }

    private double bestUnitsPerMinute;
    public double BestUnitsPerMinute
    {
        get => bestUnitsPerMinute;
        set
        {
            bestUnitsPerMinute = value;
            OnPropertyChanged();
        }
    }

    private double averageCartonsPerHour;
    public double AverageCartonsPerHour
    {
        get => averageCartonsPerHour;
        set
        {
            averageCartonsPerHour = value;
            OnPropertyChanged();
        }
    }

    private double averageItemsPerMinute;
    public double AverageItemsPerMinute
    {
        get => averageItemsPerMinute;
        set
        {
            averageItemsPerMinute = value;
            OnPropertyChanged();
        }
    }

    private double averageScansPerMinute;
    public double AverageScansPerMinute
    {
        get => averageScansPerMinute;
        set
        {
            averageScansPerMinute = value;
            OnPropertyChanged();
        }
    }

    private double averageUnitsPerMinute;
    public double AverageUnitsPerMinute
    {
        get => averageUnitsPerMinute;
        set
        {
            averageUnitsPerMinute = value;
            OnPropertyChanged();
        }
    }

    private bool showGridView;
    public bool ShowGridView
    {
        get => showGridView;
        set
        {
            showGridView = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowListView));
        }
    }

    public bool ShowListView => !ShowGridView;

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public QAOperatorStatsVM(QAToolVM parentVM)
    {
        ParentVM = parentVM;
        Deimos = ParentVM.ParentVM;
        Helios = parentVM.Helios;
        ProgressBar = parentVM.ProgressBar;

        filterString = string.Empty;

        AllOperators = new List<QAOperatorVM>();
        Operators = new ObservableCollection<QAOperatorVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (StartDate is null || EndDate is null || StartDate > EndDate) return;
        var fromDate = (DateTime)StartDate;
        var toDate = (DateTime)EndDate;

        ProgressBar.StartTask("Pulling QA Stat Data...");
        var operators = await Helios.StaffReader.QAOperatorStatisticsAsync(fromDate, toDate);
        ProgressBar.EndTask();

        AllOperators = operators.Select(e => new QAOperatorVM(e)).ToList();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var cartons = AllOperators.Where(o =>
            o.ScanTime >= minTimeFilter && (
                Regex.IsMatch(o.ID.ToString(), FilterString, RegexOptions.IgnoreCase) ||
                Regex.IsMatch(o.FullName, FilterString, RegexOptions.IgnoreCase)));

        Operators.Clear();
        foreach (var carton in cartons) Operators.Add(carton);

        BestCartonsPerHour = Operators.Max(o => o.CartonsPerHour);
        BestItemsPerMinute = Operators.Max(o => o.ItemsPerMinute);
        BestScansPerMinute = Operators.Max(o => o.ScansPerMinute);
        BestUnitsPerMinute = Operators.Max(o => o.UnitsPerMinute);
        AverageCartonsPerHour = Operators.Average(o => o.CartonsPerHour);
        AverageItemsPerMinute = Operators.Average(o => o.ItemsPerMinute);
        AverageScansPerMinute = Operators.Average(o => o.ScansPerMinute);
        AverageUnitsPerMinute = Operators.Average(o => o.UnitsPerMinute);

        foreach (var operatorVM in Operators)
            operatorVM.SetPerformance(AverageCartonsPerHour, AverageItemsPerMinute, AverageScansPerMinute, AverageUnitsPerMinute,
                BestCartonsPerHour, BestItemsPerMinute, BestScansPerMinute, BestUnitsPerMinute);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}