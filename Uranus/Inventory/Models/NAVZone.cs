using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

[Table("ZoneList")]
public class NAVZone : IComparable<NAVZone>, IEquatable<NAVZone>
{
    [PrimaryKey, ForeignKey(typeof(ZoneExtension))] public string ID { get; set; } // Combination of LocationCode and Code (e.g. 9600:PK)
    public string Code { get; set; }
    [ForeignKey(typeof(NAVLocation))] public string LocationCode { get; set; }
    public string Description { get; set; }
    public int Ranking { get; set; }

    [ManyToOne(nameof(LocationCode), nameof(NAVLocation.Zones), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVLocation? Location { get; set; }

    [OneToMany(nameof(NAVBin.ZoneCode), nameof(NAVBin.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVBin> Bins { get; set; }
    [OneToMany(nameof(NAVMoveLine.ZoneID), nameof(NAVMoveLine.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVMoveLine> MoveLines { get; set; }
    [OneToMany(nameof(Models.NAVStock.ZoneID), nameof(Models.NAVStock.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> NAVStock { get; set; }

    [ManyToMany(typeof(BayZone), nameof(BayZone.ZoneID), nameof(Bay.Zones), CascadeOperations = CascadeOperation.All)]
    public List<Bay> Bays { get; set; }

    [OneToOne(nameof(ID), nameof(ZoneExtension.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public ZoneExtension? Extension { get; set; }

    [Ignore]
    public EAccessLevel AccessLevel
    {
        get => (Extension ??= new ZoneExtension(this)).AccessLevel;
        set => (Extension ??= new ZoneExtension(this)).AccessLevel = value;
    }

    [Ignore]
    public Site? Site
    {
        get => (Extension ??= new ZoneExtension(this)).Site;
        set
        {
            (Extension ??= new ZoneExtension(this)).Site = value;
            Extension.SiteName = value?.Name ?? "";
        }
    }

    [Ignore]
    public string SiteName
    {
        get => (Extension ??= new ZoneExtension(this)).SiteName;
        set => (Extension ??= new ZoneExtension(this)).SiteName = value;
    }

    [Ignore] public Dictionary<int, Stock> Stock { get; set; }

    public NAVZone()
    {
        ID = string.Empty;
        Code = string.Empty;
        LocationCode = string.Empty;
        Description = string.Empty;
        Bins = new List<NAVBin>();
        MoveLines = new List<NAVMoveLine>();
        NAVStock = new List<NAVStock>();
        Bays = new List<Bay>();
        Stock = new Dictionary<int, Stock>();
    }

    public NAVZone(string zoneCode, NAVLocation location)
    {
        Code = zoneCode;
        LocationCode = location.Code;
        Location = location;
        Location.Zones.Add(this);
        ID = $"{LocationCode}:{Code}";
        Description = "";
        Bins = new List<NAVBin>();
        Bays = new List<Bay>();
        MoveLines = new List<NAVMoveLine>();
        NAVStock = new List<NAVStock>();
        Stock = new Dictionary<int, Stock>();
    }

    public NAVZone(string id, string code, string locationCode, string description, int ranking, NAVLocation location,
        List<NAVBin> bins, List<NAVMoveLine> moveLines, List<NAVStock> navStock, List<Bay> bays, ZoneExtension? extension)
    {
        ID = id;
        Code = code;
        LocationCode = locationCode;
        Description = description;
        Ranking = ranking;
        Location = location;
        Bins = bins;
        MoveLines = moveLines;
        NAVStock = navStock;
        Bays = bays;
        Extension = extension;
        Stock = new Dictionary<int, Stock>();
    }

    public NAVZone(string zoneID)
    {
        ID = zoneID;
        var lc = zoneID.Split(':');
        LocationCode = lc[0];
        Code = lc[1];
        Description = string.Empty;
        Bins = new List<NAVBin>();
        MoveLines = new List<NAVMoveLine>();
        NAVStock = new List<NAVStock>();
        Bays = new List<Bay>();
        Stock = new Dictionary<int, Stock>();
    }

    public int CompareTo(NAVZone? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return other is null ? 1 : string.Compare(ID, other.ID, StringComparison.Ordinal);
    }

    public bool Equals(NAVZone? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID == other.ID;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((NAVZone)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return ID.GetHashCode();
    }

    public void AddStock(Stock newStock)
    {
        if (Stock.TryGetValue(newStock.ItemNumber, out var oldStock))
            oldStock.Add(newStock);
        else
            Stock.Add(newStock.ItemNumber, newStock.Copy());
    }

    public void RemoveStock(Stock stock)
    {
        if (!Stock.TryGetValue(stock.ItemNumber, out var currentStock)) return;

        currentStock.Sub(stock);
        if (currentStock.IsEmpty()) Stock.Remove(stock.ItemNumber);
    }
}