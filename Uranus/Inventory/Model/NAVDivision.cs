using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model;

[Table("ItemDivision")]
public class NAVDivision
{
    [PrimaryKey] public int Code { get; set; }
    public string Description { get; set; }

    [OneToMany(nameof(NAVCategory.DivisionCode), nameof(NAVCategory.Division), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVCategory> Categories { get; set; }
    [OneToMany(nameof(NAVItem.DivisionCode), nameof(NAVItem.Division), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<NAVItem> Items { get; set; }
}