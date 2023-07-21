using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Prometheus.ViewModels.Pages.Inventory;

public class BinVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    public Charon Charon { get; set; }

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

    public ObservableCollection<NAVBin> DisplayBins { get; set; }

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

    public RefreshDataCommand RefreshDataCommand { get; set; }

    private BinVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        bins = new List<NAVBin>();
        DisplayBins = new ObservableCollection<NAVBin>();
        selectedBin = new NAVBin();
        binFilter = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task<BinVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<BinVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new BinVM(helios, charon);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        Bins = await Helios.InventoryReader.NAVBinsAsync(pullType: EPullType.ObjectOnly);
        DisplayBins = new ObservableCollection<NAVBin>(Bins);
    }

    private void ApplyFilter()
    {
        DisplayBins.Clear();

        var binList = Bins.Where(b => Regex.IsMatch(b.Code, BinFilter, RegexOptions.IgnoreCase));

        foreach (var bin in binList)
            DisplayBins.Add(bin);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}