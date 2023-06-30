using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class MixedCartonStock : Stock
{
    public MixedCarton MixedCarton { get; set; }
    public List<Stock> Stock { get; set; }

    public int UnitQty => UnevenStockLevels ? Stock.Sum(s => s.EachQty) : MixedCarton.UnitsPerCarton * CaseQty;
    public new int EachQty => UnitQty;
    public new static int PackQty => 0;
    public new int CaseQty { get; set; }

    public bool SuccessfullyGenerated { get; set; }
    public bool UnevenStockLevels { get; set; }

    private string? mixedContentDisplay;
    public string MixedContentDisplay
    {
        get => mixedContentDisplay ??= MixedCarton.GetMixedContentDisplay();
        set => mixedContentDisplay = value;
    }

    public MixedCartonStock(MixedCarton mixedCarton)
    {
        MixedCarton = mixedCarton;
        Stock = new List<Stock>();
    }

    /// <summary>
    /// Determine MC stock based on list of given stock.
    /// Will check that they are appropriately matched and create a mixed carton type based on this stock.
    /// </summary>
    /// <param name="stock">Group of stock.</param>
    public MixedCartonStock(List<Stock> stock)
    {
        // Validate.
        if (!IsValidStockGroup(stock))
        {
            MixedCarton = new MixedCarton();
            Stock = new List<Stock>();
            return;
        }

        Stock = stock;
        var mc = MixedCarton.GetMixedCarton(Stock);

        if (mc is null)
        {
            MixedCarton = new MixedCarton();
            return;
        }

        MixedCarton = mc;

        SetBaseStockValues();

        SuccessfullyGenerated = true;
    }

    /// <summary>
    /// Determine MC Stock based on given mc template and list of given stock.
    /// Will check that they are appropriately matched based on the given mc type, and other factors.
    /// </summary>
    /// <param name="mixedCarton">Mixed Carton Template object.</param>
    /// <param name="stock">Group of stock, which will b e left with items not belonging to mixed carton.</param>
    public MixedCartonStock(MixedCarton mixedCarton, ref List<Stock> stock)
    {
        MixedCarton = mixedCarton;
        Stock = MixedCarton.GetValidStock(ref stock);

        if (!IsValidStockGroup(Stock))
        {
            stock.AddRange(Stock);
            Stock.Clear();
            return;
        }

        SetBaseStockValues();

        SuccessfullyGenerated = true;
    }

    public int CaseMode()
    {
        if (!UnevenStockLevels) return CaseQty;

        var list = Stock.Select(s => MixedCarton.Cartons(s));
        
        return list.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;
    }

    /// <summary>
    /// Checks that a given list of stock is valid.
    /// Must have more than one item. Only eaches. Matching bin locations.
    /// </summary>
    /// <param name="stock"></param>
    /// <returns></returns>
    private static bool IsValidStockGroup(IReadOnlyCollection<Stock> stock)
    {
        var bins = stock.Select(s => s.BinID).Distinct();

        return bins.Count() == 1 &&
               stock.Count > 1 &&
               !stock.Any(s =>
                   s.CaseQty > 0 ||
                   s.PackQty > 0);
    }

    /// <summary>
    /// Set up all basic Stock values as determined and copied from the listed stock - particularly the first stock item in list.
    /// Assumes stock list is accurate, valid, and usable.
    /// Also assumes MixedCarton Object is accurately set.
    /// </summary>
    private void SetBaseStockValues()
    {
        ItemNumber = (int)Math.Round(Stock.Average(m => m.ItemNumber));

        Item = new NAVItem(ItemNumber)
        {
            Description = MixedCarton.Name
        };

        var itemCase = new NAVUoM(Item, EUoM.CASE)
        {
            QtyPerUoM = MixedCarton.UnitsPerCarton,
            Cube = MixedCarton.Cube,
            Weight = MixedCarton.Weight,
            Length = MixedCarton.Length,
            Height = MixedCarton.Height,
            Width = MixedCarton.Width
        };

        CaseQty = MixedCarton.Cartons(Stock);

        UnevenStockLevels = (from stock in Stock
            let mci = MixedCarton.Item(stock.ItemNumber)
            where mci is null || mci.QtyPerCarton * CaseQty != stock.EachQty
            select stock).Any();

        Item.UoMs.Add(itemCase);
        Item.Case = itemCase;

        Bin = Stock.First().Bin;
    }

    /// <summary>
    /// Finds the common values in the descriptions across multiple items.
    /// </summary>
    /// <param name="items">List of items that, in theory, have something in common.</param>
    /// <returns>Common String</returns>
    public static string GetDescription(IEnumerable<NAVItem> items) =>
        Utility.LongestCommonSubstring(items.Select(i => i.Description)).Trim();

}