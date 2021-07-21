using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("ItemCategory")]
    public class NAVCategory
    {
        [PrimaryKey]
        public int Code { get; set; }
        public string Description { get; set; }
        [ForeignKey(typeof(NAVDivision))]
        public int DivisionCode { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVDivision Division { get; set; }

    }
}
