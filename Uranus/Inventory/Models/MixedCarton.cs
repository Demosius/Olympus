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

    private int? unitsPerCarton;
    [Ignore] public int UnitsPerCarton 
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

    public MixedCarton()
    {
        ID = Guid.NewGuid();
        Name = string.Empty;
        Items = new List<MixedCartonItem>();
    }
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

    public static string GetDescription(IEnumerable<NAVItem> items) =>
        Utility.LongestCommonSubstring(items.Select(i => i.Description)).Trim();
}