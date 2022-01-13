using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Olympus.Uranus.Inventory.Model
{
    public class Bay
    {
        [PrimaryKey]
        public string ID { get; set; } // Bay name. Called ID for consitency.

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BinExtension> BinBays { get; set; }

        [ManyToMany(typeof(BayZone), CascadeOperations = CascadeOperation.All)]
        public List<NAVZone> Zones { get; set; }

        // Does not hold bins directly.
        // Instead uses the bin extension references to get bins.
        private List<NAVBin> bins;
        [Ignore]
        public List<NAVBin> Bins
        {
            get
            {
                if (bins is null)
                {
                    if (BinBays is null)
                        BinBays = new List<BinExtension> { };
                    bins = BinBays.Select(bb => bb.Bin).ToList();
                }
                return bins;
            } 
            set { bins = value; }
        }
    }
}
