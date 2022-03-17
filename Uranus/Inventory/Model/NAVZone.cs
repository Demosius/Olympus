﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model;

[Table("ZoneList")]
public class NAVZone
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
    [OneToMany(nameof(NAVStock.ZoneID), nameof(NAVStock.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> Stock { get; set; }

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

    public NAVZone()
    {
        ID = string.Empty;
        Code = string.Empty;
        LocationCode = string.Empty;
        Description = string.Empty;
        Bins = new List<NAVBin>();
        MoveLines = new List<NAVMoveLine>();
        Stock = new List<NAVStock>();
        Bays = new List<Bay>();
    }

    public NAVZone(string id, string code, string locationCode, string description, int ranking, NAVLocation location, List<NAVBin> bins, List<NAVMoveLine> moveLines, List<NAVStock> stock, List<Bay> bays, ZoneExtension? extension)
    {
        ID = id;
        Code = code;
        LocationCode = locationCode;
        Description = description;
        Ranking = ranking;
        Location = location;
        Bins = bins;
        MoveLines = moveLines;
        Stock = stock;
        Bays = bays;
        Extension = extension;
    }
}