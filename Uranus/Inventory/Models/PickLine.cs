using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class PickLine
{
    [PrimaryKey] public string ID { get; set; }
    public EAction ActionType { get; set; }
    [ForeignKey(typeof(NAVLocation))] public string LocationCode { get; set; }
    public string ZoneCode { get; set; }
    [ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }
    public string Number { get; set; }
    public int LineNumber { get; set; }
    [ForeignKey(typeof(BatchTOLine))] public string CartonID { get; set; }
    [ForeignKey(typeof(Batch))] public string BatchID { get; set; }
    public string PickerID { get; set; }
    public string SourceNo { get; set; }
    public string SourceLineNo { get; set; }
    public string BinCode { get; set; }
    [ForeignKey(typeof(NAVBin))] public string BinID { get; set; }
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public string Description { get; set; }
    public int Qty { get; set; }
    public int BaseQty { get; set; }
    public int QtyPerUoM { get; set; }
    public EUoM UoM { get; set; }
    public int QtyOutstanding { get; set; }
    public int QtyToHandle { get; set; }
    public int QtyHandled { get; set; }
    public DateTime DueDate { get; set; }

    [ManyToOne(nameof(BatchID), nameof(Models.Batch.PickLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public Batch? Batch { get; set; }
    [ManyToOne(nameof(CartonID), nameof(Models.BatchTOLine.PickLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public BatchTOLine? BatchTOLine { get; set; }
    [ManyToOne(nameof(BinID), nameof(NAVBin.PickLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVBin? Bin { get; set; }
    [ManyToOne(nameof(ZoneID), nameof(NAVZone.PickLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVZone? Zone { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.PickLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVItem? Item { get; set; }
    [ManyToOne(nameof(LocationCode), nameof(NAVLocation.PickLines), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVLocation? Location { get; set; }

    public PickLine()
    {
        ID = "Uninitialised";
        LocationCode = string.Empty;
        ZoneCode = string.Empty;
        ZoneID = string.Empty;
        Number = string.Empty;
        CartonID = string.Empty;
        BatchID = string.Empty;
        PickerID = string.Empty;
        SourceLineNo = string.Empty;
        SourceNo = string.Empty;
        BinID = string.Empty;
        BinCode = string.Empty;
        Description = string.Empty;
    }

    public static string GetID(string pickNo, int lineNo) => $"{pickNo}:{lineNo}";

    public static string GetZoneID(string locationCode, string zoneCode) => $"{locationCode}:{zoneCode}";

    public static string GetBinID(string zoneID, string binCode) => $"{zoneID}:{binCode}";

    public static string GetBinID(string locationCode, string zoneCode, string binCode) =>
        GetBinID(GetZoneID(locationCode, zoneCode), binCode);
    
    public void InitializeData()
    {
        ID = GetID(Number, LineNumber);
        ZoneID = GetZoneID(LocationCode, ZoneCode);
        BinID = GetBinID(ZoneID, BinCode);
    }
}