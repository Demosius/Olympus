using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Controls;

public class MCStockVM : INotifyPropertyChanged
{
    public Stock Stock { get; set; }
    public MCBinVM Bin { get; set; }

    #region INotifyPropertyChanged Members

    public int Sku => Stock.ItemNumber;
    public NAVItem? Item => Stock.Item;
    public string ItemName => Stock.Item?.Description ?? string.Empty;
    public MixedCarton MixedCarton => Bin.MixedCartonVM.MixedCarton;
    public string MCName => MixedCarton.Name;
    public int LocationQty => Stock.BaseQty;

    private MixedCartonItem mcItem;
    public MixedCartonItem MCItem
    {
        get => mcItem;
        set
        {
            mcItem = value;
            OnPropertyChanged();
        }
    }

    public int QtyPerCarton => MCItem.QtyPerCarton;

    #endregion

    public MCStockVM(MCBinVM bin, Stock stock)
    {
        Stock = stock;
        Bin = bin;

        mcItem = MixedCarton.Items.First(mci => mci.ItemNumber == Sku);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}