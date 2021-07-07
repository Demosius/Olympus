﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("ItemUoM")]
    public class NAVUoM
    {
        [PrimaryKey] // Combination of ItemNumber and Code (e.g. 271284:CASE)
        public string ID { get; set; }
        public string Code { get; set; }
        [ForeignKey(typeof(NAVItem))]
        public int ItemNumber { get; set; }
        public int QtyPerUoM { get; set; }
        public int MaxQty { get; set; }
        public bool ExcludCartonization { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Cube { get; set; }
        public double Weight { get; set; }

        [OneToMany]
        public List<NAVStock> Stock { get; set; }
        [ManyToOne]
        public NAVItem Item { get; set; }
    }
}
