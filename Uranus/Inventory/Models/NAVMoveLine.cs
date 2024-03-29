﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Models;

[Table("MoveLine")]
public class NAVMoveLine : IComparable<NAVMoveLine>
{
    [PrimaryKey] public Guid ID { get; set; }
    public EAction ActionType { get; set; }
    [ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; } // Combination of LocationCode and ZoneCode (e.g. 9600:PK)
    [ForeignKey(typeof(NAVBin))] public string BinID { get; set; } // Combination of ZoneID and BinCode (e.g. 9600:PR:PR18 058)
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public string ZoneCode { get; set; }
    public string BinCode { get; set; }
    public int Qty { get; set; }
    public EUoM UoM { get; set; }
    [ForeignKey(typeof(NAVLocation))] public string LocationCode { get; set; }

    [ManyToOne(nameof(ZoneID), nameof(NAVZone.MoveLines), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVZone? Zone { get; set; }
    [ManyToOne(nameof(BinID), nameof(NAVBin.MoveLines), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin? Bin { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.MoveLines), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }
    [ManyToOne(nameof(LocationCode), nameof(NAVLocation.MoveLines), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVLocation? Location { get; set; }

    [Ignore] public int QtyPerUoM => UoM switch
    {
        EUoM.CASE => Item?.QtyPerCase ?? 1,
        EUoM.PACK => Item?.QtyPerPack ?? 1,
        _ => 1
    };

    [Ignore] public int BaseQty => Qty * QtyPerUoM;

    public NAVMoveLine()
    {
        ID = Guid.NewGuid();
        // Moves copied from NAV do not have associated location code in clipboard, so assume 9600 at default.
        LocationCode = "9600";
        ZoneID = string.Empty;
        BinID = string.Empty;
        ZoneCode = string.Empty;
        BinCode = string.Empty;
    }

    public void SetZone(NAVZone zone)
    {
        Zone = zone;
        Zone.MoveLines.Add(this);
    }

    public void SetBin(NAVBin bin)
    {
        Bin = bin;
        Bin.MoveLines.Add(this);
    }

    public void SetItem(NAVItem item)
    {
        Item = item;
        Item.MoveLines.Add(this);
    }

    public bool IsMatch(NAVMoveLine otherLines)
    {
        return BaseQty == otherLines.BaseQty &&
               ItemNumber == otherLines.ItemNumber &&
               ActionType != otherLines.ActionType;
    }

    public int CompareTo(NAVMoveLine? other)
    {
        return other is null ? 1 : BaseQty.CompareTo(other.BaseQty);
    }
}