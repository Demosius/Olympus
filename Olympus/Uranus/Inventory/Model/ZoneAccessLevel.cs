using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Inventory.Model
{
    public class ZoneAccessLevel
    {
        [PrimaryKey, ForeignKey(typeof(NAVZone))]
        public string ZoneID { get; set; }
        public EAccessLevel AccessLevel { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVZone Zone { get; set; }
    }
}
