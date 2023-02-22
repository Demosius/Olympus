using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class MixedCarton
{
    [PrimaryKey] public string Name { get; set; }
    public double Cube { get; set; }
    public double Weight { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    [OneToMany(nameof(MixedCartonItem.MixedCartonName), nameof(MixedCartonItem.MixedCarton), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<MixedCartonItem> Items { get; set; }

    private int? unitsPerCarton;
    [Ignore] public int UnitsPerCarton 
    { 
        get => unitsPerCarton ??= Items.Sum(i => i.QtyPerCarton);
        set => unitsPerCarton = value;
    }

    public MixedCarton()
    {
        Name = string.Empty;
        Items = new List<MixedCartonItem>();
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
            if (s is null) 
                s = $"{mixedCartonItem.ItemNumber} x {mixedCartonItem.QtyPerCarton}";
            else 
                s += $"\n{mixedCartonItem.ItemNumber} x {mixedCartonItem.QtyPerCarton}";
        }

        return s ?? string.Empty;
    }
}