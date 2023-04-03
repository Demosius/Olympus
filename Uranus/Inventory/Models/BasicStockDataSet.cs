using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class BasicStockDataSet
{
    public Dictionary<int, NAVItem> Items { get; set; }
    public Dictionary<string, NAVZone> Zones { get; set; }
    public Dictionary<string, NAVBin> Bins { get; set; }
    public List<NAVStock> NAVStock { get; set; }
    public List<NAVUoM> UoMs { get; set; }
    public List<Stock> Stock { get; set; }

    public List<string> MissingZones = new();
    public List<string> MissingItems = new();
    public List<string> MissingBins = new();

    public BasicStockDataSet(IEnumerable<NAVItem> items, IEnumerable<NAVZone> zones, IEnumerable<NAVBin> bins,
        IEnumerable<NAVStock> stock, IEnumerable<NAVUoM> uomList)
    {
        Items = items.ToDictionary(i => i.Number, i => i);
        Zones = zones.ToDictionary(z => z.ID, z => z);
        NAVStock = stock.ToList();
        Bins = bins.ToDictionary(b => b.ID, b => b);
        UoMs = uomList.ToList();
        Stock = new List<Stock>();

        SetRelationships();
    }

    public void SetRelationships()
    {
        MissingBins.Clear();
        MissingZones.Clear();
        MissingItems.Clear();

        SetFromUoMs();
        SetFromBins();
        SetFromStock();

        var missingData = MissingDataText();
        if (missingData is not null) throw new Exception(missingData);
    }

    private void SetFromUoMs()
    {
        foreach (var navUoM in UoMs)
        {
            if (!Items.TryGetValue(navUoM.ItemNumber, out var item))
            {
                // TODO: When we can expect/confirm the presence of all/specific data in the item IMR report, include once more the missing items from UoM Data.
                //MissingItems.Add(navUoM.ItemNumber.ToString());
                item = new NAVItem(navUoM.ItemNumber);
                Items.Add(navUoM.ItemNumber, item);
            }
            navUoM.Item = item;
            switch (navUoM.UoM)
            {
                case EUoM.CASE:
                    item.Case = navUoM;
                    break;
                case EUoM.EACH:
                    item.Each = navUoM;
                    break;
                case EUoM.PACK:
                    item.Pack = navUoM;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(navUoM.UoM));
            }

        }
    }

    private void SetFromBins()
    {
        foreach (var (_, bin) in Bins)
        {
            if (!Zones.TryGetValue(bin.ZoneID, out var zone))
            {
                MissingZones.Add(bin.ZoneCode);
                continue;
            }
            zone.Bins.Add(bin);
            bin.Zone = zone;
        }
    }

    private void SetFromStock()
    {
        foreach (var navStock in NAVStock)
        {
            if (Items.TryGetValue(navStock.ItemNumber, out var item))
            {
                item.NAVStock.Add(navStock);
                navStock.Item = item;
            }
            else
            {
                MissingItems.Add(navStock.ItemNumber.ToString());
            }

            if (Zones.TryGetValue(navStock.ZoneID, out var zone))
            {
                zone.NAVStock.Add(navStock);
                navStock.Zone = zone;
            }
            else
            {
                MissingZones.Add(navStock.ZoneCode);
            }

            if (Bins.TryGetValue(navStock.BinID, out var bin))
            {
                bin.NAVStock.Add(navStock);
                navStock.Bin = bin;
            }
            else
            {
                MissingBins.Add(navStock.BinCode);
            }

            var stock = new Stock(navStock);
            stock.AddStock();

            if (!stock.Merged) Stock.Add(stock);
        }
    }

    private string? MissingDataText()
    {
        if (!MissingBins.Any() && !MissingZones.Any() && !MissingItems.Any()) return null;

        return "Missing data from database:\n" +
               $"{MissingBinsText()}{MissingZonesText()}{MissingItemsText()}";
    }

    private string MissingBinsText()
    {
        if (!MissingBins.Any()) return string.Empty;
        return "\n  Missing Bins:\n\t" +
               $"{(MissingBins.Count > 3 ? $"{string.Join("|", MissingBins.Take(3))}...(+{MissingBins.Count - 3})" : $"{string.Join("|", MissingBins)}")}\n";
    }

    private string MissingZonesText()
    {
        if (!MissingZones.Any()) return string.Empty;
        return "\n  Missing Zones:\n\t" +
               $"{(MissingZones.Count > 3 ? $"{string.Join("|", MissingZones.Take(3))}...(+{MissingZones.Count - 3})" : $"{string.Join("|", MissingZones)}")}\n";
    }

    private string MissingItemsText()
    {
        if (!MissingItems.Any()) return string.Empty;
        return "\n  Missing Items:\n\t" +
               $"{(MissingItems.Count > 3 ? $"{string.Join("|", MissingItems.Take(3))}...(+{MissingItems.Count - 3})" : $"{string.Join("|", MissingItems)}")}\n";
    }
}