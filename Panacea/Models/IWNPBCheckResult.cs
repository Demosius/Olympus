using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Panacea.Models;

/// <summary>
/// Items With No Pick Bin Check Results.
/// </summary>
public class IWNPBCheckResult : INotifyPropertyChanged
{
    public NAVItem Item { get; set; }
    public string BinString { get; set; }   // Concat string of bins in which the item resides.
    public string ZoneString { get; set; }  // Concat string of zones in which the item resides.

    #region Item Access

    public int ItemNumber => Item.Number;

    public string ItemDescription  => Item.Description;

    public string Platform => Item.Platform?.ToString() ?? $"{Item.PlatformCode} - ";
    public string Category => Item.Category?.ToString() ?? $"{Item.CategoryCode} - ";
    public string Genre => Item.Genre?.ToString() ?? $"{Item.GenreCode} - ";
    public string Division => Item.Division?.ToString() ?? $"{Item.DivisionCode} - ";

    public int TODemandBaseQty => Item.TODemandBaseQty;

    public double TODemandCube => Item.TODemandCube;

    #endregion

    public IWNPBCheckResult(NAVItem item)
    {
        Item = item;
        BinString = string.Join("|", Item.StockDict.Values.Select(stock => stock.BinCode).ToList().Distinct());
        ZoneString = string.Join("|", Item.StockDict.Values.Select(stock => stock.ZoneCode).ToList().Distinct());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}