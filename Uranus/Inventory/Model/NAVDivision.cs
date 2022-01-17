using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Inventory.Model
{
    [Table("ItemDivision")]
    public class NAVDivision
    {
        [PrimaryKey]
        public int Code { get; set; }
        public string Description { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVCategory> Categories { get; set; }
    }
}
