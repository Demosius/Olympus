using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Extensions;

namespace Uranus.Inventory.Models;

public class MixedCarton
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Name { get; set; }
    public double Cube { get; set; }
    public double Weight { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    [OneToMany(nameof(MixedCartonItem.MixedCartonID), nameof(MixedCartonItem.MixedCarton), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<MixedCartonItem> Items { get; set; }

    private List<int>? itemNumbers;
    [Ignore]
    public List<int> ItemNumbers
    {
        get
        {
            if (itemNumbers?.Count != Items.Count)
                itemNumbers = Items.Select(i => i.ItemNumber).ToList();
            return itemNumbers;
        }
        set => itemNumbers = value;
    }

    private Dictionary<int, MixedCartonItem>? mcItemDict;
    [Ignore]
    public Dictionary<int, MixedCartonItem> MCItemDict
    {
        get => mcItemDict ??= Items.ToDictionary(i => i.ItemNumber, i => i);
        set => mcItemDict = value;
    }

    private int? unitsPerCarton;
    [Ignore]
    public int UnitsPerCarton
    {
        get => unitsPerCarton ??= Items.Sum(i => i.QtyPerCarton);
        set => unitsPerCarton = value;
    }

    private string? signature;
    [Ignore]
    public string Signature
    {
        get => signature ??= GetSignature();
        set => signature = value;
    }

    private string? platform;
    [Ignore]
    public string Platform
    {
        get => platform ??= string.Join('|', Items.Select(i => i.Platform).Distinct());
        set => platform = value;
    }

    private string? category;
    [Ignore]
    public string Category
    {
        get => category ??= string.Join('|', Items.Select(i => i.Category).Distinct());
        set => category = value;
    }

    private string? division;
    [Ignore]
    public string Division
    {
        get => division ??= string.Join('|', Items.Select(i => i.Division).Distinct());
        set => division = value;
    }

    [Ignore] public List<NAVBin> Bins { get; set; }

    public MixedCarton()
    {
        ID = Guid.NewGuid();
        Name = string.Empty;
        Items = new List<MixedCartonItem>();
        Bins = new List<NAVBin>();
    }

    public MixedCartonItem? Item(int itemNumber)
    {
        _ = MCItemDict.TryGetValue(itemNumber, out var item);
        return item;
    }

    public int Cartons(Stock stock)
    {
        if (!MCItemDict.TryGetValue(stock.ItemNumber, out var mcItem)) return 0;
        return stock.EachQty / mcItem.QtyPerCarton;
    }

    public int Cartons(List<Stock> stock) => stock.Select(Cartons).Where(i => i != 0).Min();

    /// <summary>
    /// Creates a useful signature for comparison to other MC objects based on the associated items and quantities.
    /// </summary>
    /// <returns></returns>
    private string GetSignature()
    {
        return string.Join(':', Items.OrderBy(i => i.ItemNumber).Select(i => i.ItemSignature));
    }

    /// <summary>
    /// Calculate core carton values (cube, size, etc.) based on the contained items.
    /// </summary>
    public void CalculateValuesFromItems()
    {
        Cube = Items.Sum(i => i.Cube);
        Weight = Items.Sum(i => i.Weight);
        Length = Items.Max(i => i.Length);
        Width = Items.Max(i => i.Width);
        Height = Items.Max(i => i.Height);
    }

    /// <summary>
    /// Checks a given list of moves to determine its validity for this Mixed Carton.
    /// </summary>
    /// <param name="moves"></param>
    /// <returns>True if matches this Mixed carton template items and ratios.</returns>
    public bool IsValidMoveSet(IEnumerable<Move> moves)
    {
        // Check count.
        var moveList = moves.ToList();
        if (moveList.Count != Items.Count) return false;

        var cases = 0;

        // Match move against item.
        foreach (var move in moveList)
        {
            var item = Items.FirstOrDefault(i => i.ItemNumber == move.ItemNumber);
            if (item is null || move.TakeEaches % item.QtyPerCarton != 0) return false;
            if (cases == 0)
                cases = move.TakeEaches / item.QtyPerCarton;
            else
                if (move.TakeEaches / item.QtyPerCarton != cases) return false;
        }

        return true;
    }

    /// <summary>
    /// Checks a given list of stock to determine its validity for this Mixed Carton.
    /// </summary>
    /// <param name="stockList">Stock to check against.</param>
    /// <returns>True if matches this Mixed carton template items and ratios such that it can contain a qty of this mixed carton.</returns>
    public bool IsValidStock(List<Stock> stockList)
    {
        // Must contain all items, but can contain more. (usually shouldn't, but mixed pallet locations can and do happen)
        if (stockList.Count < Items.Count) return false;

        // Check items against stock.
        return !(from item in Items
            let stock = stockList.FirstOrDefault(s => s.ItemNumber == item.ItemNumber)
            where stock is null || stock.PackQty > 0 || stock.CaseQty > 0 || stock.EachQty < item.QtyPerCarton
            select item).Any();
    }

    /// <summary>
    /// Given a list of stock returns the stock that is valid to this Mixed carton, leaving non-valid stock behind.
    ///
    /// Does not account for appropriate levels, just the presence of the items, excepting that they contain the minimum required for a single carton.
    /// </summary>
    /// <param name="stockList">Ref: The non-valid stock will remain after processing.</param>
    /// <returns>A list of stock items valid to this Mixed Carton object. Will be empty if not ALL items are represented.</returns>
    public List<Stock> GetValidStock(ref List<Stock> stockList)
    {
        var cartonStock = new List<Stock>();

        // Must contain all items, but can contain more. (usually shouldn't, but mixed pallet locations can and do happen)
        if (stockList.Count < Items.Count) return cartonStock;

        // Check items against stock.
        foreach (var item in Items)
        {
            var stock = stockList.FirstOrDefault(s => s.ItemNumber == item.ItemNumber);
            if (stock is null || stock.PackQty > 0 || stock.CaseQty > 0 || stock.EachQty < item.QtyPerCarton) break;
            cartonStock.Add(stock);
            stockList.Remove(stock);
        }

        if (cartonStock.Count == Items.Count) return cartonStock;

        stockList.AddRange(cartonStock);
        cartonStock.Clear();

        return cartonStock;
    }

    /// <summary>
    /// Calculate stock that would not not fit into this mixed carton.
    /// </summary>
    /// <param name="stock"></param>
    /// <returns>int: units left over once cartons is calculated.</returns>
    public int CalculateRemaining(List<Stock> stock)
    {
        var ret = 0;

        var cartons = Cartons(stock);

        foreach (var s in stock)
        {
            if (!MCItemDict.TryGetValue(s.ItemNumber, out var mcItem))
                ret += s.BaseQty;
            else
                ret += s.BaseQty - cartons * mcItem.QtyPerCarton;
        }

        return ret;
    }

    /// <summary>
    /// Generate a display string to show and represent each sku within a given carton.
    /// </summary>
    /// <returns></returns>
    public string GetMixedContentDisplay()
    {
        string? s = null;
        foreach (var mixedCartonItem in Items.OrderBy(i => i.ItemNumber))
        {
            var a = $"{mixedCartonItem.ItemNumber}{(mixedCartonItem.Identifier.Length is > 0 and < 4 ? $" ({mixedCartonItem.Identifier.PadBoth(3)})" : "")} x {mixedCartonItem.QtyPerCarton}";
            if (s is null)
                s = a;
            else
                s += $"\n{a}";
        }

        return s ?? string.Empty;
    }

    public static MixedCarton? GetMixedCarton(IEnumerable<NAVStock> stock)
    {
        var stockList = stock.ToList();

        // Ensure there are only eaches.
        if (stockList.Any(navStock => navStock.GetEUoM() != EUoM.EACH)) return null;

        var items = stockList.Where(m => m.Item is not null).Select(m => m.Item!).ToList();

        if (!items.Any()) return null;

        var mc = new MixedCarton
        {
            Name = GetDescription(items)
        };

        var caseQty = Utility.HCF(stockList.Select(s => s.Qty));

        if (caseQty == 0) return null;

        foreach (var navStock in stockList.Where(navStock => navStock.Item is not null))
        {
            // Creation of MCI(mc, i) adds itself to both MC and I.
            _ = new MixedCartonItem(mc, navStock.Item!)
            {
                QtyPerCarton = navStock.Qty / caseQty
            };
        }

        mc.CalculateValuesFromItems();

        return mc;
    }

    public static MixedCarton? GetMixedCarton(List<Stock> stockList)
    {
        // Ensure there are only eaches.
        if (stockList.Any(s => s.PackQty > 0 || s.CaseQty > 0)) return null;

        var items = stockList.Where(m => m.Item is not null).Select(s => s.Item!).ToList();

        if (!items.Any()) return null;

        var mc = new MixedCarton
        {
            Name = GetDescription(items)
        };

        var caseQty = Utility.HCF(stockList.Select(s => s.EachQty));

        if (caseQty == 0) return null;

        foreach (var stock in stockList.Where(navStock => navStock.Item is not null))
        {
            // Creation of MCI(mc, i) adds itself to both MC and I.
            _ = new MixedCartonItem(mc, stock.Item!)
            {
                QtyPerCarton = stock.EachQty / caseQty
            };
        }

        mc.CalculateValuesFromItems();

        return mc;
    }

    public static string GetDescription(IEnumerable<NAVItem> items) =>
        Utility.LongestCommonSubstring(items.Select(i => i.Description)).Trim();
}