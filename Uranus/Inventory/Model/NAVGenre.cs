using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model;

[Table("ItemGenre")]
public class NAVGenre
{
    [PrimaryKey] public int Code { get; set; }
    public string Description { get; set; }

    [OneToMany(nameof(NAVItem.GenreCode), nameof(NAVItem.Genre), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVItem> Items { get; set; }
}