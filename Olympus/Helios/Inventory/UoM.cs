using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory
{
    public enum EUoM
    {
        EACH,
        PACK,
        CASE
    }

    public class UoM
    {
        public Item Item { get; }
        public EUoM Type { get; }
        public int Qty { get; set; }
        public int MaxQty { get; set; }
        public bool InnerPack { get; set; }
        public bool ExcludeCartonization { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Cube { get; set; }
        public float Weight { get; set; }

        public UoM(Item item, EUoM type, int qty, bool innerPack, float length, 
                    float width, float height, float cube, float weight, bool excludeCartonization = false)
        {
            Item = item;
            Type = type;
            Qty = qty;
            InnerPack = innerPack;
            ExcludeCartonization = excludeCartonization;
            Length = length;
            Width = width;
            Height = height;
            Cube = cube;
            Weight = weight;
            // Assign itself to the associated item.
            switch (type)
            {
                case EUoM.CASE:
                    Item.Case = this;
                    break;
                case EUoM.PACK:
                    item.Pack = this;
                    break;
                case EUoM.EACH:
                    item.Each = this;
                    break;
                default:
                    throw new Exception("UoM created without UoM Enum value.");
            }
        }

        /// <summary>
        ///  A simple conversion for units of measure from the base string to the Enum varaible.
        /// </summary>
        /// <param name="uomType">String representation of the unit of measure.</param>
        /// <returns>UoM Enum type.</returns>
        public static EUoM TypeFromString(string uomType)
        {
            switch (uomType.ToUpper())
            {
                case "CASE":
                    return EUoM.CASE;
                case "PACK":
                    return EUoM.PACK;
                default:
                    return EUoM.EACH;
            }
        }

    }

    public class SimpleUoM
    {
        public string Code { get; set; }
        public int ItemNumber { get; set; }
        public int QtyPerUoM { get; set; }
        public int MaxQty { get; set; }
        public bool InnerPack { get; set; }
        public bool ExcludeCartonization { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Cube { get; set; }
        public double Weight { get; set; }

        public SimpleUoM() { }

        public SimpleUoM(string code, int itemNumber, int qtyPerUoM, int maxQty,
                         bool innerPack, bool excludeCartonization, 
                         double length, double width, double height, double cube, double weight)
        {
            Code = code;
            ItemNumber = itemNumber;
            QtyPerUoM = qtyPerUoM;
            MaxQty = maxQty;
            InnerPack = innerPack;
            ExcludeCartonization = excludeCartonization;
            Length = length;
            Width = width;
            Height = height;
            Cube = cube;
            Weight = weight;
        }
    }
}
