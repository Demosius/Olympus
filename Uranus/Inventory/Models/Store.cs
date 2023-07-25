using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

public enum EStoreType
{
    EBGames,
    ZING
}

public class Store
{
    [PrimaryKey, ForeignKey(typeof(NAVLocation))] public string Number { get; set; }
    public int WaveNumber { get; set; }
    public EVolume Volume { get; set; }
    public string CCNRegion { get; set; }
    public string RoadCCN { get; set; }
    public int TransitDays { get; set; }
    public string MBRegion { get; set; }
    public string RoadRegion { get; set; }
    public int SortingLane { get; set; }
    public string State { get; set; }
    public string Region { get; set; }
    public EStoreType Type { get; set; }

    [OneToMany(nameof(NAVTransferOrder.StoreNumber), nameof(NAVTransferOrder.Store), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVTransferOrder> TransferOrders { get; set; }
    [OneToMany(nameof(BatchTOLine.StoreNo), nameof(BatchTOLine.Store), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<BatchTOLine> BatchTOLines { get; set; }

    [OneToOne(nameof(Number), nameof(NAVLocation.Store), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVLocation? Location { get; set; }

    [Ignore] public string Wave => $"W{WaveNumber:00}";

    public Store()
    {
        Number = string.Empty;
        CCNRegion = string.Empty;
        RoadCCN = string.Empty;
        MBRegion = string.Empty;
        RoadRegion = string.Empty;
        State = string.Empty;
        Region = string.Empty;
        TransferOrders = new List<NAVTransferOrder>();
        BatchTOLines = new List<BatchTOLine>();
    }
    
    public void AddTO(NAVTransferOrder transferOrder)
    {
        if (transferOrder.StoreNumber != Number) return;
        transferOrder.Store = this;
        TransferOrders.Add(transferOrder);
    }

    public void AddTOLine(BatchTOLine line)
    {
        BatchTOLines.Add(line);
        line.SetStore(this);
    }

    public void SetTOLines(List<BatchTOLine> lines)
    {
        BatchTOLines =lines;
        foreach (var batchTOLine in lines)
            batchTOLine.SetStore(this);
    }
}