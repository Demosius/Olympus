using SQLite;
using SQLiteNetExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("UpdateTimes")]
    class TableUpdate
    {
        [PrimaryKey]
        public string TableName { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
