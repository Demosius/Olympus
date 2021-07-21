using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("ItemPlatform")]
    public class NAVPlatform
    {
        [PrimaryKey]
        public int Code { get; set; }
        public string Description { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVItem> Items { get; set; }
    }
}
