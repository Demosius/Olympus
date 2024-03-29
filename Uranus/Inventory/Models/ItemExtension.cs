﻿using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class ItemExtension
{
    [PrimaryKey] public int ItemNumber { get; set; }
    public bool SiteLevelTarget { get; set; }

    [OneToOne(nameof(ItemNumber), nameof(NAVItem.Extension), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }

    public ItemExtension()
    {

    }

    public ItemExtension(NAVItem item)
    {
        Item = item;
        ItemNumber = item.Number;
        SiteLevelTarget = false;
    }
}