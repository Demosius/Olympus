using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Inventory.Model
{
    [Table("ItemUoM")]
    public class NAVUoM
    {
        [PrimaryKey] // Combination of ItemNumber and Code (e.g. 271284:CASE)
        public string ID { get; set; }
        public string Code { get; set; }
        [ForeignKey(typeof(NAVItem))]
        public int ItemNumber { get; set; }
        public int QtyPerUoM { get; set; }
        public int MaxQty { get; set; }
        public bool ExcludCartonization { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Cube { get; set; }
        public double Weight { get; set; }

        private EUoM? uom = null;
        [Ignore]
        public EUoM UoM 
        {
            get
            {
                if (uom == null)
                {
                    uom = EnumConverter.StringToUoM(Code);
                }
                return (EUoM)uom;
            }
        }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVStock> Stock { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVItem Item { get; set; }

        public NAVUoM()
        {
            QtyPerUoM = 0;
        }

        public NAVUoM(EUoM uom) : this()
        {
            this.uom = uom;
            Code = EnumConverter.UoMToString(uom);
        }

        public NAVUoM(NAVItem item, EUoM uom) : this(uom)
        {
            Item = item;
            ItemNumber = item.Number;
        }
    }
}
