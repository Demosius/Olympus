﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model
{
    [Table("ZoneList")]
    public class NAVZone
    {
        [PrimaryKey] // Combination of LocationCode and Code (e.g. 9600:PK)
        public string ID { get; set; } 
        public string Code { get; set; }
        [ForeignKey(typeof(NAVLocation))]
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public int Ranking { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVLocation Location { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVBin> Bins { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public ZoneAccessLevel ZoneAccessLevel { get; set; }

        [ManyToMany(typeof(Bay), CascadeOperations = CascadeOperation.All)]
        public List<Bay> Bays { get; set; }

        [Ignore]
        public EAccessLevel AccessLevel
        {
            get => ZoneAccessLevel.AccessLevel;
            set => ZoneAccessLevel.AccessLevel = value;
        }
    }
}