using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory
{
    /// <summary>
    ///  Used for holding constants relevant to the Inventory namespace.
    ///  Common use is for holding Column names for data.
    /// </summary>
    public static class Constants
    {
        public static readonly Dictionary<string, string> BIN_COLUMNS = new Dictionary<string, string>
        {
            {"location", "Location Code" },
            {"zone_code", "Zone Code" },
            {"code", "Code" },
            {"description", "Description" },
            {"empty", "Empty" },
            {"assigned", "Bin Assigned" },
            {"ranking", "Bin Ranking" },
            {"used_cube", "Used Cubage" },
            {"max_cube", "Maximum Cubage" },
            {"last_cc_date", "CC Last Count Date" },
            {"last_pi_date", "PI - Last Count Date" }
        };

        public static readonly Dictionary<string, string> UOM_COLUMNS = new Dictionary<string, string>
        {
            {"code", "Code" },
            {"item_number", "Item No." },
            {"qty_per_uom", "Qty. per Unit of Measure" },
            {"max_qty", "Max Qty" },
            {"inner_pack", "Inner Pack" },
            {"exclude_cartonization", "Exclude Cartonization" },
            {"length", "Length (CM)" },
            {"width", "Width (CM)" },
            {"height", "Height (CM)" },
            {"cube", "CM Cubage" },
            {"weight", "Weight (Kg)" }
        };

        public static readonly Dictionary<string, string> STOCK_COLUMNS = new Dictionary<string, string>
        {
            { "location", "Location Code" },
            { "bin_code", "Bin Code" }, 
            { "item_number", "Item No." },
            { "uom_code", "Unit of Measure Code" },
            { "zone_code", "Zone Code" },
            { "barcode", "ItemBarcode" },
            { "qty", "Quantity" },
            { "pick_qty", "Pick Qty." },
            { "put_away_qty", "Put-away Qty." },
            { "neg_adj_qty", "Neg. Adjmt. Qty." },
            { "pos_adj_qty", "Pos. Adjmt. Qty." },
            { "date_created", "Date Created" },
            { "time_created", "Time Created" },
            { "fixed", "Fixed" }
        };

        public static readonly Dictionary<string, string> ITEM_COLUMNS = new Dictionary<string, string>
        {
            { "number", "ItemCode" },
            { "description", "ItemName" },
            { "preowned", "NewUsed" },
            { "category", "CategoryCode" },
            { "platform", "PlatformCode" },
            { "division", "DivisionCode" },
            { "genre", "GenreCode" },
            { "length", "Length" },
            { "width", "Width" },
            { "height", "Height" },
            { "cube", "Cubage" },
            { "weight", "Weight" }
        };

        public static readonly Dictionary<string, string> ZONE_COLUMNS = new Dictionary<string, string>
        {
            { "code", "Code" },
            { "type", "Type" },
            { "description", "Description" }
        };
    }
}
