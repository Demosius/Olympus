using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

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
