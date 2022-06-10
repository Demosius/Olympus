using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Linq;

namespace Uranus.Inventory.Models;

[Table("BinContents")]
public class NAVStock
{
    // Combination of BinID and UoMID (e.g. 9600:PR:PR18 058:271284:CASE)
    [PrimaryKey] public string ID { get; set; }
    // Combination of ZoneID, and BinCode (e.g. 9600:PR:PR18 058)
    [ForeignKey(typeof(NAVBin))] public string BinID { get; set; }
    // Combination of LocationCode and ZoneCode (e.g. 9600:PR)
    [ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }
    // Combination of ItemNumber and UoMCode (e.g. 271284:CASE)
    // ReSharper disable once InconsistentNaming
    [ForeignKey(typeof(NAVUoM))] public string UoMID { get; set; }
    [ForeignKey(typeof(NAVLocation))] public string LocationCode { get; set; }
    public string ZoneCode { get; set; }
    public string BinCode { get; set; }
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public string UoMCode { get; set; }
    public int Qty { get; set; }
    public int PickQty { get; set; }
    public int PutAwayQty { get; set; }
    public int NegAdjQty { get; set; }
    public int PosAdjQty { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime TimeCreated { get; set; }
    public bool Fixed { get; set; }

    [ManyToOne(nameof(BinID), nameof(NAVBin.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin? Bin { get; set; }
    [ManyToOne(nameof(UoMID), nameof(NAVUoM.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVUoM? UoM { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }
    [ManyToOne(nameof(ZoneID), nameof(NAVZone.NAVStock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVZone? Zone { get; set; }
    [ManyToOne(nameof(LocationCode), nameof(NAVLocation.NAVStock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVLocation? Location { get; set; }

    public NAVStock()
    {
        ID = string.Empty;
        BinID = string.Empty;
        ZoneID = string.Empty;
        UoMID = string.Empty;
        LocationCode = string.Empty;
        ZoneCode = string.Empty;
        BinCode = string.Empty;
        UoMCode = string.Empty;
        DateCreated = DateTime.MinValue;
        TimeCreated = DateTime.MinValue;
        Fixed = false;
    }

    public NAVStock(string id, string binID, string zoneID, string uoMid, string locationCode, string zoneCode,
        string binCode, int itemNumber, string uoMCode, int qty, int pickQty, int putAwayQty, int negAdjQty,
        int posAdjQty, DateTime dateCreated, DateTime timeCreated, bool @fixed, NAVBin? bin, NAVUoM? uoM,
        NAVItem? item, NAVZone? zone, NAVLocation? location)
    {
        ID = id;
        BinID = binID;
        ZoneID = zoneID;
        UoMID = uoMid;
        LocationCode = locationCode;
        ZoneCode = zoneCode;
        BinCode = binCode;
        ItemNumber = itemNumber;
        UoMCode = uoMCode;
        Qty = qty;
        PickQty = pickQty;
        PutAwayQty = putAwayQty;
        NegAdjQty = negAdjQty;
        PosAdjQty = posAdjQty;
        DateCreated = dateCreated;
        TimeCreated = timeCreated;
        Fixed = @fixed;
        Bin = bin;
        UoM = uoM;
        Item = item;
        Zone = zone;
        Location = location;
    }

    public int GetBaseQty()
    {
        return Qty * (UoM ?? new NAVUoM(EUoM.EACH)).QtyPerUoM;
    }

    public EUoM GetEUoM()
    {
        return (EUoM)Enum.Parse(typeof(EUoM), !Enum.GetNames(typeof(EUoM)).Contains(UoMCode) ? "EACH" : UoMCode); // EnumConverter.StringToUoM(UoMCode);
    }

    public double GetWeight()
    {
        return Qty * (UoM?.Weight ?? 0);
    }
}