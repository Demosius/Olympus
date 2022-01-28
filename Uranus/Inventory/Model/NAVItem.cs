using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Inventory.Model
{
    [Table("ItemList")]
    public class NAVItem
    {
        [PrimaryKey]
        public int Number { get; }
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
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVTransferOrder> TransferOrders { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVCategory Category { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVDivision Division { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVPlatform Platform { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVGenre Genre { get; set; }

        // Specific UoMs
        [Ignore]
        public NAVUoM Case { get; set; }
        [Ignore]
        public NAVUoM Pack { get; set; }
        [Ignore]
        public NAVUoM Each { get; set; }

        public NAVItem() {}

        public NAVItem(int num)
        {
            Number = num;
        }

        // Sets the specific UoMs, so we don't need to pull from an "unordered" list all the time.
        public void SetUoMs()
        {
            foreach (var uom in UoMs)
            {
                var e = uom.UoM;
                switch (e)
                {
                    case EUoM.Case:
                        Case = uom;
                        break;
                    case EUoM.Pack:
                        Pack = uom;
                        break;
                    case EUoM.Each:
                    default:
                        Each = uom;
                        break;
                }
            }
            Case ??= new(this, EUoM.Case);
            Pack ??= new(this, EUoM.Pack);
            Each ??= new(this, EUoM.Each);
        }

        public int GetBaseQty(int eaches = 0, int packs = 0, int cases = 0)
        {
            if (Each is null || Pack is null || Case is null) SetUoMs();
            return eaches * Each.QtyPerUoM + packs * Pack.QtyPerUoM + cases * Case.QtyPerUoM; 
        }

        /* Equality and Operator Overloading */
        public override bool Equals(object obj) => this.Equals(obj as NAVItem);
        
        public bool Equals(NAVItem item)
        {
            if (item is null) return false;

            if (ReferenceEquals(this, item)) return true;

            if (GetType() != item.GetType()) return false;

            return Number == item.Number && Description == item.Description && Barcode == item.Barcode 
                && CategoryCode == item.CategoryCode && PlatformCode == item.PlatformCode
                && DivisionCode == item.DivisionCode && GenreCode == item.GenreCode
                && Math.Abs(Length - item.Length) < 0.0001 && Math.Abs(Width - item.Width) < 0.0001 && Math.Abs(Height - item.Height) < 0.0001
                && Math.Abs(Cube - item.Cube) < 0.0001 && Math.Abs(Weight - item.Weight) < 0.0001 && PreOwned == item.PreOwned;
        }

        public override int GetHashCode() => Number.GetHashCode();

        public static bool operator ==(NAVItem lhs, NAVItem rhs)
        {
            if (lhs is not null) return lhs.Equals(rhs);
            return rhs is null;
        }

        public static bool operator !=(NAVItem lhs, NAVItem rhs) => !(lhs == rhs);

        public override string ToString()
        {
            return $"{Number}|{Description}|{Barcode}: (DIV:{DivisionCode}, CAT:{CategoryCode}, PF:{PlatformCode}, GEN:{GenreCode}) - L:{Length}cm x W:{Width}cm x H:{Height}cm = Cube:{Cube}cm³, Weight:{Weight} - {(PreOwned ? "Used" : "New")} ";
        }
    }
}
