using System;
using System.Text.RegularExpressions;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class MixedCartonItem
{
    [ForeignKey(typeof(MixedCarton))] public Guid MixedCartonID { get; set; }
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public string Identifier { get; set; }
    public int QtyPerCarton { get; set; }

    [ManyToOne(nameof(MixedCartonID), nameof(Models.MixedCarton.Items), CascadeOperations = CascadeOperation.CascadeRead)]
    public MixedCarton? MixedCarton { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.MixedCartons), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVItem? Item { get; set; }

    [Ignore] public double Cube => (Item?.Cube ?? 0) * QtyPerCarton;
    [Ignore] public double Weight => (Item?.Weight ?? 0) * QtyPerCarton;
    [Ignore] public double Length => Item?.Length ?? 0;
    [Ignore] public double Width => Item?.Width ?? 0;
    [Ignore] public double Height => Item?.Height ?? 0;
    [Ignore] public string ItemSignature => $"{ItemNumber}[{QtyPerCarton}]";

    public MixedCartonItem()
    {
        MixedCartonID = Guid.Empty;
        Identifier = string.Empty;
    }

    public MixedCartonItem(Guid mixedCartonID, int itemNumber)
    {
        MixedCartonID = mixedCartonID;
        ItemNumber = itemNumber;
        Identifier = string.Empty;
    }

    public MixedCartonItem(MixedCarton mixedCarton, NAVItem item) : this(mixedCarton.ID, item.Number)
    {
        Item = item;
        MixedCarton = mixedCarton;
        item.MixedCartons.Add(this);
        mixedCarton.Items.Add(this);
        Identifier = Regex.Replace(Item.Description, MixedCarton.Name, "").Trim();
    }

    /// <summary>
    /// Remove self from associated item and carton.
    /// </summary>
    public void Remove()
    {
        Item?.MixedCartons.Remove(this);
        MixedCarton?.Items.Remove(this);
    }
}