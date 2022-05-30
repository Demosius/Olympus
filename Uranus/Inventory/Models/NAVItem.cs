using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

[Table("ItemList")]
public class NAVItem
{
    [PrimaryKey, ForeignKey(typeof(ItemExtension))] public int Number { get; set; }
    public string Description { get; set; }
    public string Barcode { get; set; }
    [ForeignKey(typeof(NAVCategory))] public int CategoryCode { get; set; }
    [ForeignKey(typeof(NAVPlatform))] public int PlatformCode { get; set; }
    [ForeignKey(typeof(NAVDivision))] public int DivisionCode { get; set; }
    [ForeignKey(typeof(NAVGenre))] public int GenreCode { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Cube { get; set; }
    public double Weight { get; set; }
    public bool PreOwned { get; set; }

    [OneToOne(nameof(Number), nameof(ItemExtension.Item), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public ItemExtension? Extension { get; set; }

    [OneToMany(nameof(NAVUoM.ItemNumber), nameof(NAVUoM.Item), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVUoM> UoMs { get; set; }
    [OneToMany(nameof(Models.NAVStock.ItemNumber), nameof(Models.NAVStock.Item), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> NAVStock { get; set; }
    [OneToMany(nameof(Models.Stock.ItemNumber), nameof(Models.Stock.Item), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Stock> Stock { get; set; }
    [OneToMany(nameof(NAVTransferOrder.ItemNumber), nameof(NAVTransferOrder.Item), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVTransferOrder> TransferOrders { get; set; }
    [OneToMany(nameof(Move.ItemNumber), nameof(Move.Item), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Move> Moves { get; set; }
    [OneToMany(nameof(NAVMoveLine.ItemNumber), nameof(NAVMoveLine.Item), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVMoveLine> MoveLines { get; set; }
    [OneToMany(nameof(SiteItemLevel.ItemNumber), nameof(SiteItemLevel.Item), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<SiteItemLevel> SiteLevels { get; set; }

    [ManyToOne(nameof(CategoryCode), nameof(NAVCategory.Items), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVCategory? Category { get; set; }
    [ManyToOne(nameof(DivisionCode), nameof(NAVDivision.Items), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVDivision? Division { get; set; }
    [ManyToOne(nameof(PlatformCode), nameof(NAVPlatform.Items), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVPlatform? Platform { get; set; }
    [ManyToOne(nameof(GenreCode), nameof(NAVGenre.Items), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVGenre? Genre { get; set; }

    [Ignore]
    public bool SiteLevelTarget
    {
        get => (Extension ??= new ItemExtension(this)).SiteLevelTarget;
        set => (Extension ??= new ItemExtension(this)).SiteLevelTarget = value;
    }

    // Specific UoMs
    [Ignore] public NAVUoM? Case { get; set; }
    [Ignore] public NAVUoM? Pack { get; set; }
    [Ignore] public NAVUoM? Each { get; set; }

    public NAVItem()
    {
        Description = string.Empty;
        Barcode = string.Empty;
        PreOwned = false;
        UoMs = new List<NAVUoM>();
        NAVStock = new List<NAVStock>();
        Stock = new List<Stock>();
        TransferOrders = new List<NAVTransferOrder>();
        Moves = new List<Move>();
        MoveLines = new List<NAVMoveLine>();
        SiteLevels = new List<SiteItemLevel>();
    }

    public NAVItem(int num) : this()
    {
        Number = num;
    }

    public void SetData(Dictionary<int, NAVCategory> categoryDict, Dictionary<int, NAVPlatform> platformDict, Dictionary<int, NAVDivision> divisionDict, Dictionary<int, NAVGenre> genreDict)
    {
        if (categoryDict.TryGetValue(CategoryCode, out var navCategory))
        {
            Category = navCategory;
            Category.Items.Add(this);
        }

        if (divisionDict.TryGetValue(DivisionCode, out var navDivision))
        {
            Division = navDivision;
            Division.Items.Add(this);
        }

        if (platformDict.TryGetValue(PlatformCode, out var navPlatform))
        {
            Platform = navPlatform;
            Platform.Items.Add(this);
        }

        if (!genreDict.TryGetValue(GenreCode, out var navGenre)) return;
        Genre = navGenre;
        Genre.Items.Add(this);
    }

    // Sets the specific UoMs, so we don't need to pull from an "unordered" list all the time.
    public void SetUoMs()
    {
        foreach (var uom in UoMs)
        {
            var e = uom.UoM;
            switch (e)
            {
                case EUoM.CASE:
                    Case = uom;
                    break;
                case EUoM.PACK:
                    Pack = uom;
                    break;
                case EUoM.EACH:
                default:
                    Each = uom;
                    break;
            }
        }
    }

    public int GetBaseQty(int eaches = 0, int packs = 0, int cases = 0)
    {
        return eaches * (Each ?? new NAVUoM()).QtyPerUoM + packs * (Pack ?? new NAVUoM()).QtyPerUoM + cases * (Case ?? new NAVUoM()).QtyPerUoM;
    }

    /* Equality and Operator Overloading */
    public override bool Equals(object? obj) => Equals(obj as NAVItem);

    public bool Equals(NAVItem? item)
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

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Number.GetHashCode();

    public static bool operator ==(NAVItem? lhs, NAVItem? rhs) => lhs?.Equals(rhs) ?? rhs is null;

    public static bool operator !=(NAVItem? lhs, NAVItem? rhs) => !(lhs == rhs);

    public override string ToString()
    {
        return $"{Number}|{Description}|{Barcode}: (DIV:{DivisionCode}, CAT:{CategoryCode}, PF:{PlatformCode}, GEN:{GenreCode}) - L:{Length}cm x W:{Width}cm x H:{Height}cm = Cube:{Cube}cm³, Weight:{Weight} - {(PreOwned ? "Used" : "New")} ";
    }
}