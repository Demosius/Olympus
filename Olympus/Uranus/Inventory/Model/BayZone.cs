using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Inventory.Model
{
    public class BayZone
    {
        [ForeignKey(typeof(Bay))]
        public string BayID { get; set; }
        [ForeignKey(typeof(NAVZone))]
        public string ZoneID { get; set; }
    }
}
