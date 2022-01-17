using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Inventory.Model
{
    public class Batch
    {
        [PrimaryKey]
        public string ID { get; set; }
        public int Priority { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Move> Moves { get; set; }

    }
}
