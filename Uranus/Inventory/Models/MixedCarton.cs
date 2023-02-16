using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class MixedCarton
{
    [PrimaryKey] public string Name { get; set; }

    [OneToMany(nameof(MixedCartonItem.MixedCartonName), nameof(MixedCartonItem.MixedCarton), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<MixedCartonItem> Items { get; set; }

    public MixedCarton()
    {
        Name = string.Empty;
        Items = new List<MixedCartonItem>();
    }
}