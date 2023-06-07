using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

[Table("ItemPlatform")]
public class NAVPlatform
{
    [PrimaryKey] public int Code { get; set; }
    public string Description { get; set; }

    [OneToMany(nameof(NAVItem.PlatformCode), nameof(NAVItem.Platform), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVItem> Items { get; set; }

    public NAVPlatform()
    {
        Description = string.Empty;
        Items = new List<NAVItem>();
    }

    public NAVPlatform(int code, string description)
    {
        Code = code;
        Description = description;
        Items = new List<NAVItem>();
    }

    public NAVPlatform(int code, string description, List<NAVItem> items) : this(code, description)
    {
        Items = items;
    }

    public void AddItem(NAVItem item)
    {
        item.Platform = this;
        Items.Add(item);
    }

    public override string ToString()
    {
        return $"{Code} - {Description}";
    }
}