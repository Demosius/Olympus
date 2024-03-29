﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

[Table("ItemUoM")]
public class NAVUoM
{
    // Combination of ItemNumber and Code (e.g. 271284:CASE)
    [PrimaryKey] public string ID { get; set; }
    public string Code { get; set; }
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public int QtyPerUoM { get; set; }
    public int MaxQty { get; set; }
    public bool ExcludeCartonization { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Cube { get; set; }
    public double Weight { get; set; }

    private EUoM? uom;
    [Ignore]
    public EUoM UoM
    {
        get
        {
            if (uom is not null) return (EUoM)uom;

            if (!Enum.TryParse(Code, out EUoM u))
            {
                u = EUoM.EACH;
                Code = "EACH";
            }
            uom = u;

            return (EUoM)uom;
        }
    }

    [OneToMany(nameof(NAVStock.UoMID), nameof(NAVStock.UoM), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> Stock { get; set; }

    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.UoMs), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }

    // Transfer Orders
    [Ignore] public List<NAVTransferOrder> TransferOrders { get; set; }
    [Ignore] public int TODemandQty { get; set; }
    [Ignore] public int TODemandBaseQty => TODemandQty * QtyPerUoM;
    [Ignore] public double TODemandCube => TODemandQty * Cube;

    public NAVUoM()
    {
        ID = string.Empty;
        Code = string.Empty;
        ExcludeCartonization = false;
        Stock = new List<NAVStock>();
        TransferOrders = new List<NAVTransferOrder>();
    }

    public NAVUoM(EUoM uom) : this()
    {
        this.uom = uom;
        Code = EnumConverter.UoMToString(uom);
    }

    public NAVUoM(NAVItem item, EUoM uom) : this(uom)
    {
        Item = item;
        ItemNumber = item.Number;
    }

    public void AddTO(NAVTransferOrder transferOrder)
    {
        if (transferOrder.ItemNumber != ItemNumber || transferOrder.UoM != UoM) return;
        TransferOrders.Add(transferOrder);
        TODemandQty += transferOrder.Qty;
    }

    public void RemoveTO(NAVTransferOrder transferOrder)
    {
        if (transferOrder.ItemNumber != ItemNumber || transferOrder.UoM != UoM || !TransferOrders.Contains(transferOrder)) return;
        TransferOrders.Remove(transferOrder);
        TODemandQty -= transferOrder.Qty;
    }
}