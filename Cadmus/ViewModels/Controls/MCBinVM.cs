using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Controls;

public class MCBinVM : INotifyPropertyChanged
{
    public NAVBin Bin { get; set; }
    public MixedCartonVM MixedCartonVM { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<MCStockVM> Stock { get; set; }

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

    public MCBinVM(MixedCartonVM mcVM, NAVBin bin)
    {
        Bin = bin;
        MixedCartonVM = mcVM;

        Stock = new ObservableCollection<MCStockVM>(
            Bin.Stock.Values
            .Where(s => mcVM.ItemNumbers
                .Contains(s.ItemNumber))
            .Select(s => new MCStockVM(this, s)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}