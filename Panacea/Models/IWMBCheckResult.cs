using System.Collections.Generic;
using System.Linq;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Panacea.Models;

public class IWMBCheckResult
{
    public NAVItem Item { get; set; }

    public EZoneType ZoneType { get; set; }

    public bool AllowSeparatedUoMs { get; set; }

    private readonly List<string> zoneList;
    private readonly List<string> binList;

    public string ZoneString { get; set; }
    public string BinString { get; set; }

    public bool HasMultipleBins { get; set; }

    #region Direct Item access

    public int Number => Item.Number;
    public string Description => Item.Description;

    #endregion

    public IWMBCheckResult(NAVItem item, EZoneType zoneType, bool allowSeparatedUoMs = false)
    {
        Item = item;
        ZoneType = zoneType;
        AllowSeparatedUoMs = allowSeparatedUoMs;

        zoneList = new List<string>();
        binList = new List<string>();

        var hasEach = false;
        var hasPack = false;
        var hasCase = false;

        HasMultipleBins = false;

        foreach (var (s, stock) in item.StockDict)
        {
            if (stock.Zone is null || stock.Bin is null || stock.Zone.ZoneType != zoneType) continue;

            zoneList.Add(stock.Zone.Code);
            binList.Add(stock.GetUoMString());

            if (stock.Cases is not null)
            {
                if (hasCase) HasMultipleBins = true;
                hasCase = true;
            }

            if (stock.Packs is not null)
            {
                if (hasPack) HasMultipleBins = true;
                hasPack = true;
            }

            if (stock.Eaches is not null)
            {
                if (hasEach) HasMultipleBins = true;
                hasEach = true;
            }

            if (!AllowSeparatedUoMs && binList.Count > 1) HasMultipleBins = true;
        }

        ZoneString = string.Join("|", zoneList.Distinct());
        BinString = string.Join(" | ", binList.Distinct());
    }
}