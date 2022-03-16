﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Model;

public class Bay
{
    [PrimaryKey] public string ID { get; set; } // Bay name. Called ID for consistency.

    [OneToMany(nameof(BinExtension.BayID), nameof(BinExtension.Bay), CascadeOperations = CascadeOperation.All)]
    public List<BinExtension> BinBays { get; set; }

    [ManyToMany(typeof(BayZone), nameof(BayZone.BayID), nameof(NAVZone.Bays), CascadeOperations = CascadeOperation.All)]
    public List<NAVZone> Zones { get; set; }

    // Does not hold bins directly.
    // Instead uses the bin extension references to get bins.
    private List<NAVBin> bins;
    [Ignore] public List<NAVBin> Bins
    {
        get
        {
            if (bins is not null) return bins;
            BinBays ??= new List<BinExtension>();
            bins = BinBays.Select(bb => bb.Bin).ToList();
            return bins;
        }
        set => bins = value;
    }
}