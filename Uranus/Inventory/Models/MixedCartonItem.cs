using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class MixedCartonItem
{
    [ForeignKey(typeof(MixedCarton))] public string MixedCartonName { get; set; }
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public int QtyPerCarton { get; set; }

    [ManyToOne(nameof(MixedCartonName), nameof(Models.MixedCarton.Items), CascadeOperations = CascadeOperation.CascadeRead)]
    public MixedCarton? MixedCarton { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.MixedCartons), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVItem? Item { get; set; }

    [Ignore] public double Cube => (Item?.Cube ?? 0) * QtyPerCarton;
    [Ignore] public double Weight => (Item?.Weight ?? 0) * QtyPerCarton;
    [Ignore] public double Length => Item?.Length ?? 0;
    [Ignore] public double Width => Item?.Width ?? 0;
    [Ignore] public double Height => Item?.Height ?? 0;

    public MixedCartonItem()
    {
        MixedCartonName = string.Empty;
    }

    public MixedCartonItem(string mixedCartonName, int itemNumber)
    {
        MixedCartonName = mixedCartonName;
        ItemNumber = itemNumber;
    }

    public MixedCartonItem(MixedCarton mixedCarton, NAVItem item) : this(mixedCarton.Name, item.Number)
    {
        Item = item;
        MixedCarton = mixedCarton;
        item.MixedCartons.Add(this);
        mixedCarton.Items.Add(this);
    }
}