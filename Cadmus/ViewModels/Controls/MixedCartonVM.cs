using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Cadmus.Annotations;
using Uranus;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Controls;

/// <summary>
/// Mixed carton view model.
/// Represents the template, and contains instances of stock locations (bins) containing matching stock.
/// </summary>
public class MixedCartonVM : INotifyPropertyChanged
{
    public MixedCarton MixedCarton { get; set; }

    public List<MCBinVM> AllBins { get; set; }

    #region MC Access

    public string Name => MixedCarton.Name;

    public List<int> ItemNumbers => MixedCarton.ItemNumbers;
    public string Platform => MixedCarton.Platform;
    public string Category => MixedCarton.Category;
    public string Division => MixedCarton.Division;
    public string Signature => MixedCarton.Signature;

    #endregion

    #region INotifyPropertChanged Members

    public ObservableCollection<MCBinVM> Bins { get; set; }

    private bool showBins;
    public bool ShowBins
    {
        get => showBins;
        set
        {
            showBins = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HideBins));
        }
    }

    public bool HideBins => !showBins;

    private int cases;
    public int Cases
    {
        get => cases;
        set
        {
            cases = value;
            OnPropertyChanged();
        }
    }


    private string mcQty;
    public string MCQty
    {
        get => mcQty;
        set
        {
            mcQty = value;
            OnPropertyChanged();
        }
    }

    private bool unevenLevels;
    public bool UnevenLevels
    {
        get => unevenLevels;
        set
        {
            unevenLevels = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public MixedCartonVM(MixedCarton mixedCarton, Helios helios, Dictionary<string, StockNote>? stockNotes = null)
    {
        MixedCarton = mixedCarton;

        AllBins = new List<MCBinVM>(MixedCarton.Bins.Distinct().Select(b => new MCBinVM(this, b, helios, stockNotes)));
        Bins = new ObservableCollection<MCBinVM>(AllBins);

        cases = AllBins.Sum(b => b.Cases);
        unevenLevels = AllBins.Any(b => b.UnevenLevels);
        mcQty = $"x{cases} [{AllBins.Count}]";
    }

    public int FilterZones(string zoneFilter)
    {
        var bins = AllBins.Where(b => Regex.IsMatch(b.ZoneCode, zoneFilter));
        Bins.Clear();
        foreach (var bin in bins)
            Bins.Add(bin);
        return Bins.Count;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}