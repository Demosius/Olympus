using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Models;

[Table("TOLineBatchAnalysis")]
public class NAVTransferOrder
{
    [PrimaryKey] public string ID { get; set; } // Document/Transfer No:ItemNumber:UoM.
    [ForeignKey(typeof(Store))] public string StoreNumber { get; set; } // Transfer-to Code
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public string DocumentNumber { get; set; }
    public int Qty { get; set; }
    public EUoM UoM { get; set; }
    public int AvailableQty { get; set; }
    public DateTime CreationTime { get; set; }

    [ManyToOne(nameof(StoreNumber), nameof(Models.Store.TransferOrders), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Store? Store { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.TransferOrders), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }

    public NAVTransferOrder()
    {
        ID = string.Empty;
        DocumentNumber = string.Empty;
        StoreNumber = string.Empty;
    }

    public NAVTransferOrder(string id, string documentNumber, string storeNumber, int itemNumber, int qty, EUoM uoM, int availableQty, DateTime creationTime, Store? store, NAVItem? item)
    {
        ID = id;
        DocumentNumber = documentNumber;
        StoreNumber = storeNumber;
        ItemNumber = itemNumber;
        Qty = qty;
        UoM = uoM;
        AvailableQty = availableQty;
        CreationTime = creationTime;
        Store = store;
        Item = item;
    }
}