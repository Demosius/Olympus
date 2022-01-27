using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Model
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
