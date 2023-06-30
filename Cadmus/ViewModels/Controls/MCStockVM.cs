using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cadmus.Annotations;
using Uranus;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Controls;

public class MCStockVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Stock Stock { get; set; }
    public MCBinVM Bin { get; set; }

    public StockNote StockNote { get; set; }

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

    private bool unevenLevel;
    public bool UnevenLevel
    {
        get => unevenLevel;
        set
        {
            unevenLevel = value;
            OnPropertyChanged();
        }
    }
    
    public string Note
    {
        get => StockNote.Comment;
        set
        {
            StockNote.Comment = value;
            OnPropertyChanged();
            _ = SaveNote();
        }
    }

    #endregion

    public MCStockVM(MCBinVM bin, Stock stock, Helios helios, IReadOnlyDictionary<string, StockNote>? stockNotes = null)
    {
        Helios = helios;
        Stock = stock;
        Bin = bin;

        mcItem = MixedCarton.Items.First(mci => mci.ItemNumber == Sku);

        if (stockNotes?.TryGetValue(StockID(), out var stockNote) ?? false)
            StockNote = stockNote;
        else
            StockNote = new StockNote(Bin.ID, Stock.ItemNumber, "");
    }

    private string StockID() => $"{Bin.ID}:{Stock.ItemNumber}";

    private async Task SaveNote()
    {
        await Helios.InventoryUpdater.StockNoteAsync(StockNote);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}