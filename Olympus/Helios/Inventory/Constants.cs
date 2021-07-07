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
        public static readonly Dictionary<string, int> NAV_STOCK_COLUMNS = new Dictionary<string, int>
        {
            { "Location Code", -1 },
            { "Zone Code", -1 },
            { "Bin Code", -1 },
            { "Item No.", -1 },
            { "ItemBarcode", -1 },
            { "Unit of Measure Code", -1 },
            { "Quantity", -1 },
            { "Pick Qty.", -1 },
            { "Put-away Qty.", -1 },
            { "Neg. Adjmt. Qty.", -1 },
            { "Pos. Adjmt. Qty.", -1 },
            { "Date Created", -1 },
            { "Time Created", -1 },
            { "Fixed", -1 }
        };

        public static readonly Dictionary<string, int> NAV_DIV_PF_GEN_COLUMNS = new Dictionary<string, int>
        {
            { "Code", -1 },
            { "Description", -1 }
        };

        public static readonly Dictionary<string, int> NAV_CATEGORY_COLUMNS = new Dictionary<string, int>
        {
            { "Code", -1 },
            { "Description", -1 },
            { "Item Division Code", -1 }
        };

        public static readonly Dictionary<string, int> NAV_LOCATION_COLUMNS = new Dictionary<string, int>
        {
            { "Code", -1 },
            { "Name", -1 }
        };

        public static readonly Dictionary<string, int> NAV_ZONE_COLUMNS = new Dictionary<string, int>
        {
            { "Locaiton Code", -1 },
            { "Code", -1 },
            { "Description", -1 },
            { "Zone Ranking", -1 }
        };

        public static readonly Dictionary<string, int> NAV_ITEM_COLUMNS = new Dictionary<string, int>
        {
            { "ItemCode", -1 },
            { "ItemName", -1 },
            { "PrimaryBarcode", -1 },
            { "NewUsed", -1 },
            { "CategoryCode", -1 },
            { "PlatformCode", -1 },
            { "DivisionCode", -1 }, 
            { "GenreCode", -1 },
            { "Length", -1 },
            { "Width", -1 },
            { "Height", -1 },
            { "Cubage", -1 },
            { "Weight", -1 }
        };

        public static readonly Dictionary<string, int> NAV_UOM_COLUMNS = new Dictionary<string, int>
        {
            { "Code", -1 },
            { "Item No.", -1 },
            { "Qty. per Unit of Measure", -1 },
            { "Max Qty", -1 },
            { "Inner Pack", -1 },
            { "Exclude Cartonization", -1 },
            { "Length (CM)", -1 },
            { "Width (CM)", -1 },
            { "Height (CM)", -1 },
            { "CM Cubage", -1 },
            { "Weight (Kg)", -1 }
        };

        public static readonly Dictionary<string, int> NAV_BIN_COLUMNS = new Dictionary<string, int>
        {
            { "Location Code", -1 },
            { "Zone Code", -1 },
            { "Code", -1 },
            { "Description", -1 },
            { "Empty", -1 },
            { "Bin Assigned", -1 },
            { "Bin Ranking", -1 },
            { "Used Cubage", -1 },
            { "Maximum Cubage", -1 },
            { "CC Last Count Date", -1 },
            { "PU - Last Count Date", -1 }
        };

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
            { "barcode", "PrimaryBarcode" },
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
