using System.Collections.Generic;
using System.Data;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Panacea.Models;

public class NegativeCheckResult
{
    public Stock Stock { get; }
    public EUoM UoM { get; }
    public SubStock? SubStock { get; }

    #region Direct (Sub)Stock Access

    public string Zone => Stock.Zone?.Code ?? Stock.ZoneID;
    public string Bin => Stock.Bin?.Code ?? Stock.BinID;
    public int Item => Stock.ItemNumber;
    public string Description => Stock.Item?.Description ?? string.Empty;
    public int Qty => SubStock?.Qty ?? 0;
    public int AvailableQty => SubStock?.AvailableQty ?? 0;
    public int BalanceQty => SubStock?.BalanceQty ?? 0;

    #endregion

    public string OverstockString { get; }

    public NegativeCheckResult(Stock stock, EUoM uom)
    {
        Stock = stock;
        UoM = uom;

        SubStock = UoM switch
        {
            EUoM.EACH => Stock.Eaches,
            EUoM.PACK => Stock.Packs,
            EUoM.CASE => Stock.Cases,
            _ => null
        };

        // Stock MUST have SubStock for given UoM.
        if (SubStock is null) throw new DataException($"Attempted to create negative stock result against non-existent UoM/Bin/Item combination.\n\n\tUoM:{UoM}");

        var osList = new List<string>();

        if (Stock.Item is null)
        {
            OverstockString = string.Empty;
            return;
        }

        foreach (var (_, itemStock) in Stock.Item.StockDict)
        {
            if (itemStock.Zone?.ZoneType == EZoneType.Overstock) osList.Add(itemStock.Bin?.Code ?? itemStock.BinID);
        }

        osList.Sort();
        OverstockString = string.Join("|", osList);
    }
}