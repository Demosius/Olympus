using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("LocationList")]
    public class NAVLocation
    {
        [PrimaryKey]
        public string Code { get; set; }
        public string Name { get; set; }

        [OneToMany]
        public List<NAVZone> Zones { get; set; }
    }
}
