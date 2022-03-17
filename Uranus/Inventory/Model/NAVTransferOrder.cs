using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Model;

[Table("TOLineBatchAnalysis")]
public class NAVTransferOrder
{
    [PrimaryKey] public string ID { get; set; } // Document/Transfer No.
    [ForeignKey(typeof(Store))] public string StoreNumber { get; set; } // Transfer-to Code
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public int Qty { get; set; }
    public EUoM UoM { get; set; }
    public int AvailableQty { get; set; }
    public DateTime CreationTime { get; set; }

    [ManyToOne(nameof(StoreNumber), nameof(Model.Store.TransferOrders), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Store? Store { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.TransferOrders), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }

    public NAVTransferOrder()
    {
        ID = string.Empty;
        StoreNumber = string.Empty;
    }

    public NAVTransferOrder(string id, string storeNumber, int itemNumber, int qty, EUoM uoM, int availableQty, DateTime creationTime, Store? store, NAVItem? item)
    {
        ID = id;
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