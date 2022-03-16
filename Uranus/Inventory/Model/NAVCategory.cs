using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model;

[Table("ItemCategory")]
public class NAVCategory
{
    [PrimaryKey] public int Code { get; set; }
    public string Description { get; set; }
    [ForeignKey(typeof(NAVDivision))] public int DivisionCode { get; set; }

    [ManyToOne(nameof(DivisionCode), nameof(NAVDivision.Categories), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVDivision Division { get; set; }

    [OneToMany(nameof(NAVItem.CategoryCode), nameof(NAVItem.Category), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVItem> Items { get; set; }

}