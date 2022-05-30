using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Prometheus.ViewModels.Pages.Inventory;

public class BinVM : INotifyPropertyChanged
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    private List<NAVBin> bins;
    public List<NAVBin> Bins
    {
        get => bins;
        set
        {
            bins = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<NAVBin> displayBins;
    public ObservableCollection<NAVBin> DisplayBins
    {
        get => displayBins;
        set
        {
            displayBins = value;
            OnPropertyChanged();
        }
    }

    private NAVBin? selectedBin;
    public NAVBin? SelectedBin
    {
        get => selectedBin;
        set
        {
            selectedBin = value;
            OnPropertyChanged();
        }
    }

    private string binFilter;
    public string BinFilter
    {
        get => binFilter;
        set
        {
            binFilter = value;
            OnPropertyChanged();
            _ = Task.Run(ApplyFilter);
        }
    }

    public BinVM()
    {
        bins = new List<NAVBin>();
        displayBins = new ObservableCollection<NAVBin>();
        selectedBin = new NAVBin();
        binFilter = string.Empty;

        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        {
            Bins = new List<NAVBin>();
            DisplayBins = new ObservableCollection<NAVBin>(Bins);
        }
        else
        {
            _ = Task.Run(SetBins);
        }
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        SetBins();
    }

    private void ApplyFilter()
    {
        DisplayBins = BinFilter == ""
            ? new ObservableCollection<NAVBin>(Bins)
            : new ObservableCollection<NAVBin>(Bins.Where(b => b.Code.ToLower().Contains(BinFilter.ToLower()))
                .ToList());
    }

    private void SetBins()
    {
        if (Helios is null) return;
        Bins = Helios.InventoryReader.NAVBins(pullType: EPullType.ObjectOnly);
        DisplayBins = new ObservableCollection<NAVBin>(Bins);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}