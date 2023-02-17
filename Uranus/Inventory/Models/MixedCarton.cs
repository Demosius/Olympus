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
}