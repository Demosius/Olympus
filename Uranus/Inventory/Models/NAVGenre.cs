using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

[Table("ItemGenre")]
public class NAVGenre
{
    [PrimaryKey] public int Code { get; set; }
    public string Description { get; set; }

    [OneToMany(nameof(NAVItem.GenreCode), nameof(NAVItem.Genre), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVItem> Items { get; set; }

    public NAVGenre()
    {
        Description = string.Empty;
        Items = new List<NAVItem>();
    }

    public NAVGenre(int code, string description, List<NAVItem> items)
    {
        Code = code;
        Description = description;
        Items = items;
    }

    public override string ToString()
    {
        return $"{Code} - {Description}";
    }
}