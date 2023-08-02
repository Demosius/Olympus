using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

public enum EStoreType
{
    Unknown,
    EBGames,
    ZING
}

public enum EFreightOption
{
    Restock,
    CasePick,
    NR,
    Overnight,
    Road1,
    Road2,
    ExtRoad,
    Special
}

public class Store
{
    [PrimaryKey, ForeignKey(typeof(NAVLocation))] public string Number { get; set; }
    public int WaveNumber { get; set; }
    public EVolume Volume { get; set; }
    // Region Selectors
    public string Restock { get; set; }
    public string CasePick { get; set; }
    public string NR { get; set; }
    public string Overnight { get; set; }
    public string Road1 { get; set; }
    public string Road2 { get; set; }
    public string ExtRoad { get; set; }
    public string Special { get; set; }
    
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

        Restock = string.Empty;
        CasePick = string.Empty;
        NR = string.Empty;
        Overnight = string.Empty;
        Road1 = string.Empty;
        Road2 = string.Empty;
        ExtRoad = string.Empty;
        Special = string.Empty;

        RoadCCN = string.Empty;
        MBRegion = string.Empty;
        RoadRegion = string.Empty;
        State = string.Empty;
        Region = string.Empty;
        TransferOrders = new List<NAVTransferOrder>();
        BatchTOLines = new List<BatchTOLine>();
    }

    /// <summary>
    /// Given new store object (with matching number) update this object.
    /// Only updates values if incoming store HAS a value.
    /// If empty (or default), assume old value takes precedence.
    /// </summary>
    /// <param name="newStore"></param>
    public void Update(Store newStore)
    {
        if (newStore.Number != Number) return;

        if (newStore.WaveNumber != 0) WaveNumber = newStore.WaveNumber;
        if (newStore.Volume != EVolume.Unknown) Volume = newStore.Volume;

        if (newStore.Restock != string.Empty) Restock = newStore.Restock;
        if (newStore.CasePick != string.Empty) CasePick = newStore.CasePick;
        if (newStore.NR != string.Empty) NR = newStore.NR;
        if (newStore.Overnight != string.Empty) Overnight = newStore.Overnight;
        if (newStore.Road1 != string.Empty) Road1 = newStore.Road1;
        if (newStore.Road2 != string.Empty) Road2 = newStore.Road2;
        if (newStore.ExtRoad != string.Empty) ExtRoad = newStore.ExtRoad;
        if (newStore.Special != string.Empty) Special = newStore.Special;

        if (newStore.RoadCCN != string.Empty) RoadCCN = newStore.RoadCCN;
        if (newStore.TransitDays != 0) TransitDays = newStore.TransitDays;
        if (newStore.MBRegion != string.Empty) MBRegion = newStore.MBRegion;
        if (newStore.RoadRegion != string.Empty) RoadRegion = newStore.RoadRegion;
        if (newStore.SortingLane != 0) SortingLane = newStore.SortingLane;
        if (newStore.State != string.Empty) State = newStore.State;
        if (newStore.Region != string.Empty) Region = newStore.Region;
        if (newStore.Type != EStoreType.Unknown) Type = newStore.Type;
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
        BatchTOLines = lines;
        foreach (var batchTOLine in lines)
            batchTOLine.SetStore(this);
    }

    public string FreightRegion(EFreightOption freightOption = EFreightOption.Restock)
    {
        return freightOption switch
        {
            EFreightOption.Restock => Restock,
            EFreightOption.CasePick => CasePick == string.Empty ? Restock : CasePick,
            EFreightOption.NR => NR == string.Empty ? Restock : NR,
            EFreightOption.Overnight => Overnight == string.Empty ? Restock : Overnight,
            EFreightOption.Road1 => Road1 == string.Empty ? Restock : Road1,
            EFreightOption.Road2 => Road2 == string.Empty ? Restock : Road2,
            EFreightOption.ExtRoad => ExtRoad == string.Empty ? Restock : ExtRoad,
            EFreightOption.Special => Special == string.Empty ? Restock : Special,
            _ => throw new ArgumentOutOfRangeException(nameof(freightOption), freightOption, null)
        };
    }
}