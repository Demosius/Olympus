using System.Collections.Generic;

namespace Olympus.Helios.Inventory
{
    public class Bay
    {
        public Zone Zone { get; }
        public string Name { get; set; }

        public Dictionary<string, Bin> Bins { get; }

        public Bay(Zone zone, string name)
        {
            Zone = zone;
            Name = name;

            Bins = new Dictionary<string, Bin> { };
        }

        public void AddBin(Bin bin)
        {
            if (!Bins.ContainsKey(bin.Code)) Bins.Add(bin.Code, bin);
        }
    }
}
