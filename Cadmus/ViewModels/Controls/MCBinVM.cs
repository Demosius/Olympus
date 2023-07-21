using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Uranus;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Controls;

public class MCBinVM : INotifyPropertyChanged
{
    public NAVBin Bin { get; set; }
    public MixedCartonVM MixedCartonVM { get; set; }

    #region Bin Access

    public string Code => Bin.Code;
    public string ZoneCode => Bin.ZoneCode;
    public string ID => Bin.ID;

    #endregion

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

    private bool showStock;
    public bool ShowStock
    {
        get => showStock;
        set
        {
            showStock = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HideStock));
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

    public bool HideStock => !showStock;

    #endregion

    public MCBinVM(MixedCartonVM mcVM, NAVBin bin, Helios helios, IReadOnlyDictionary<string, StockNote>? stockNotes = null)
    {
        Bin = bin;
        MixedCartonVM = mcVM;

        Stock = new ObservableCollection<MCStockVM>();

        var stockList = Bin.Stock.Values
            .Where(s => s is MixedCartonStock mc && mc.MixedCarton == mcVM.MixedCarton)
            .Select(s => (MixedCartonStock) s);


        cases = 0;
        foreach (var mcStock in stockList)
        {
            UnevenLevels |= mcStock.UnevenStockLevels;
            cases += mcStock.CaseQty;
            foreach (var stockVM in mcStock.Stock.Select(stock => new MCStockVM(this, stock, helios, stockNotes)))
            {
                Stock.Add(stockVM);
                if (mcStock.UnevenStockLevels)
                    stockVM.UnevenLevel = mcStock.CaseMode() != mcStock.MixedCarton.Cartons(stockVM.Stock);
            }
        }

        mcQty = $"x{cases}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}