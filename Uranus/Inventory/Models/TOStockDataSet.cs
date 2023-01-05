using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class TOStockDataSet
{
    public Dictionary<int, NAVItem> Items { get; set; }
    public Dictionary<string, NAVZone> Zones { get; set; }
    public Dictionary<string, NAVBin> Bins { get; set; }
    public Dictionary<string, Store> Stores { get; set; }
    public Dictionary<int, NAVPlatform> Platforms { get; set; }
    public Dictionary<int, NAVCategory> Categories { get; set; }
    public Dictionary<int, NAVDivision> Divisions { get; set; }
    public Dictionary<int, NAVGenre> Genres { get; set; }
    public List<NAVStock> NAVStock { get; set; }
    public List<NAVUoM> UoMs { get; set; }
    public List<Stock> Stock { get; set; }
    public List<NAVTransferOrder> TransferOrders { get; set; }


    public TOStockDataSet(IEnumerable<NAVItem> items, IEnumerable<NAVZone> zones, IEnumerable<NAVBin> bins,
        IEnumerable<NAVStock> stock, IEnumerable<NAVUoM> uomList, IEnumerable<NAVTransferOrder> transferOrders,
        IEnumerable<Store> stores, IEnumerable<NAVPlatform> platforms, IEnumerable<NAVGenre> genres,
        IEnumerable<NAVCategory> categories, IEnumerable<NAVDivision> divisions)
    {
        Items = items.ToDictionary(i => i.Number, i => i);
        Zones = zones.ToDictionary(z => z.ID, z => z);
        NAVStock = stock.ToList();
        Bins = bins.ToDictionary(b => b.ID, b => b);
        UoMs = uomList.ToList();
        Stock = new List<Stock>();
        TransferOrders = transferOrders.ToList();
        Stores = stores.ToDictionary(s => s.Number, s => s);
        Genres = genres.ToDictionary(g => g.Code, g => g);
        Platforms = platforms.ToDictionary(p => p.Code, p => p);
        Categories = categories.ToDictionary(c => c.Code, c => c);
        Divisions = divisions.ToDictionary(d => d.Code, d => d);

        SetRelationships();
    }

    public void SetRelationships()
    {
        SetFromUoMs();
        SetFromBins();
        SetFromStock();
        SetFromTOs();
        SetFromItems();
    }

    private void SetFromUoMs()
    {
        foreach (var navUoM in UoMs)
        {
            if (Items.TryGetValue(navUoM.ItemNumber, out var item))
            {
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
    }

    private void SetFromBins()
    {
        foreach (var (_, bin) in Bins)
        {
            if (!Zones.TryGetValue(bin.ZoneID, out var zone)) continue;
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

            if (Zones.TryGetValue(navStock.ZoneID, out var zone))
            {
                zone.NAVStock.Add(navStock);
                navStock.Zone = zone;
            }

            if (Bins.TryGetValue(navStock.BinID, out var bin))
            {
                bin.NAVStock.Add(navStock);
                navStock.Bin = bin;
            }

            var stock = new Stock(navStock);
            stock.AddStock();

            if (!stock.Merged) Stock.Add(stock);
        }
    }

    private void SetFromTOs()
    {
        foreach (var transferOrder in TransferOrders)
        {
            if (Items.TryGetValue(transferOrder.ItemNumber, out var item))
            {
                item.AddTO(transferOrder);
            }

            if (Stores.TryGetValue(transferOrder.StoreNumber, out var store))
            {
                store.AddTO(transferOrder);
            }
        }
    }

    private void SetFromItems()
    {
        foreach (var item in Items.Values)
        {
            if (Categories.TryGetValue(item.CategoryCode, out var category))
            {
                category.Items.Add(item);
                item.Category = category;
            }

            if (Platforms.TryGetValue(item.PlatformCode, out var platform))
            {
                platform.Items.Add(item);
                item.Platform = platform;
            }

            if (Divisions.TryGetValue(item.DivisionCode, out var division))
            {
                division.Items.Add(item);
                item.Division = division;
            }

            if (!Genres.TryGetValue(item.GenreCode, out var genre)) continue;
            genre.Items.Add(item);
            item.Genre = genre;
        }
    }
}