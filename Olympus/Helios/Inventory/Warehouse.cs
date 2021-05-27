using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Inventory
{
    /// <summary>
    ///  The representation of the entire warehouse (or at least one "location" - e.g. '9600'.
    ///  Contains all the zones, bays, bins, items, stock and uoms.
    /// </summary>
    public class Warehouse
    {
        public string Location { get; }

        public Dictionary<string, Item> Items { get; }
        public Dictionary<string, Bin> Bins { get; }
        public Dictionary<string, Zone> Zones { get; }

        /// <summary>
        ///  Base constructor for the warehouse.
        /// </summary>
        public Warehouse(string location = "9600")
        {
            Location = location;

            Items = new Dictionary<string, Item> { };
            Bins = new Dictionary<string, Bin> { };
            Zones = new Dictionary<string, Zone> { };
        }


    }
}
