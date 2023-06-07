using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Controls;

/// <summary>
/// Mixed carton view model.
/// Represents the template, and contains instances of stock locations (bins) containing matching stock.
/// </summary>
public class MixedCartonVM : INotifyPropertyChanged
{
    public MixedCarton MixedCarton { get; set; }

    #region MC Access

    public List<int> ItemNumbers => MixedCarton.ItemNumbers;

    #endregion

    #region INotifyPropertChanged Members

    public ObservableCollection<MCBinVM> Bins { get; set; }
    
    #endregion

    public MixedCartonVM(MixedCarton mixedCarton, IEnumerable<NAVBin> bins)
    {
        MixedCarton = mixedCarton;

        Bins = new ObservableCollection<MCBinVM>(bins.Select(b => new MCBinVM(this, b)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}