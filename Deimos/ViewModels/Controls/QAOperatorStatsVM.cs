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
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class QAOperatorStatsVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM Deimos { get; set; }
    public QAToolVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public List<QACarton> AllCartons { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<QACarton> Cartons { get; set; }

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

        AllCartons = new List<QACarton>();
        Cartons = new ObservableCollection<QACarton>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (StartDate is null || EndDate is null || StartDate > EndDate) return;
        var fromDate = (DateTime)StartDate;
        var toDate = (DateTime)EndDate;

        AllCartons = await Helios.StaffReader.QACartonsAsync(fromDate, toDate);

        ApplyFilters();
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var cartons = AllCartons.Where(c =>
            Regex.IsMatch(c.EmployeeID, FilterString));

        Cartons.Clear();
        foreach (var carton in cartons) Cartons.Add(carton);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}