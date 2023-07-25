using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class StockNote
{
    [PrimaryKey] public string ID { get; set; }  // [BinID]:[ItemNumber], e.g: 9600:PR:PR11 012:123456
    [ForeignKey(typeof(NAVBin))] public string BinID { get; set; }
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    public string Comment { get; set; }

    public StockNote()
    {
        ID = string.Empty;
        BinID = string.Empty;
        Comment = string.Empty;
    }

    public StockNote(string binID, int itemNumber, string comment)
    {
        ID = $"{binID}:{itemNumber}";
        BinID = binID;
        ItemNumber = itemNumber;
        Comment = comment;
    }
}