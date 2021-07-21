using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("ItemList")]
    public class NAVItem
    {
        [PrimaryKey]
        public int Number { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        [ForeignKey(typeof(NAVCategory))]
        public int CategoryCode { get; set; }
        [ForeignKey(typeof(NAVPlatform))]
        public int PlatformCode { get; set; }
        [ForeignKey(typeof(NAVDivision))]
        public int DivisionCode { get; set; }
        [ForeignKey(typeof(NAVGenre))]
        public int GenreCode { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Cube { get; set; }
        public double Weight { get; set; }
        public bool PreOwned { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVUoM> UoMs { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVStock> NAVStock { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Stock> Stock { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVCategory Category { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVDivision Division { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVPlatform Platform { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVGenre Genre { get; set; }

        public override bool Equals(object obj) => this.Equals(obj as NAVItem);
        
        public bool Equals(NAVItem item)
        {
            if (item is null) return false;

            if (Object.ReferenceEquals(this, item)) return true;

            if (this.GetType() != item.GetType()) return false;

            return (Number == item.Number) && (Description == item.Description) && (Barcode == item.Barcode) 
                && (CategoryCode == item.CategoryCode) && (PlatformCode == item.PlatformCode)
                && (DivisionCode == item.DivisionCode) && (GenreCode == item.GenreCode)
                && (Length == item.Length) && (Width == item.Width) && (Height == item.Height)
                && (Cube == item.Cube) && (Weight == item.Weight) && (PreOwned == item.PreOwned);
        }

        public override int GetHashCode() => (Number, Description, Barcode, 
                                              CategoryCode, DivisionCode, 
                                              PlatformCode, GenreCode, Length, 
                                              Width, Height, Weight, Cube, PreOwned).GetHashCode();

        public static bool operator ==(NAVItem lhs, NAVItem rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NAVItem lhs, NAVItem rhs) => !(lhs == rhs);

        public override string ToString()
        {
            return $"{Number}|{Description}|{Barcode}: (DIV:{DivisionCode}, CAT:{CategoryCode}, PF:{PlatformCode}, GEN:{GenreCode}) - L:{Length}cm x W:{Width}cm x H:{Height}cm = Cube:{Cube}cm³, Weight:{Weight} - {(PreOwned ? "Used" : "New")} ";
        }
    }
}
