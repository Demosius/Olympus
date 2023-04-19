using System.Collections.Generic;
// ReSharper disable StringLiteralTypo

namespace Uranus.Inventory;

/// <summary>
///  Used for holding constants relevant to the Inventory namespace.
///  Common use is for holding Column names for data.
/// </summary>
public static class Constants
{
    public static readonly Dictionary<string, int> NAVStockColumns = new()
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

    public static readonly Dictionary<string, int> NAVDivPfGenColumns = new()
    {
        { "Code", -1 },
        { "Description", -1 }
    };

    public static readonly Dictionary<string, int> NAVCategoryColumns = new()
    {
        { "Code", -1 },
        { "Description", -1 },
        { "Item Division Code", -1 }
    };

    public static readonly Dictionary<string, int> NAVLocationColumns = new()
    {
        { "Code", -1 },
        { "Name", -1 }
    };

    public static readonly Dictionary<string, int> NAVZoneColumns = new()
    {
        { "Location Code", -1 },
        { "Code", -1 },
        { "Description", -1 },
        { "Zone Ranking", -1 }
    };

    public static readonly Dictionary<string, int> NAVItemColumns = new()
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

    public static readonly Dictionary<string, int> NAV_UoMColumns = new()
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

    public static readonly Dictionary<string, int> NAVBinColumns = new()
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
        { "PI - Last Count Date", -1 }
    };

    public static readonly Dictionary<string, int> NAVToLineBatchColumns = new()
    {
        { "Document No.", -1 },
        { "Transfer-to Code", -1 },
        { "Item No.", -1 },
        { "Quantity", -1 },
        { "Unit of Measure", -1 },
        { @"Avail. UOM Fulfilment Qty", -1 },
        { "Created On Date", -1 },
        { "Created On Time", -1 }
    };

    public static readonly Dictionary<string, int> NAVMoveColumns = new()
    {
        { "Action Type", -1 },
        { "Item No.", -1 },
        { "Zone Code", -1 },
        { "Bin Code", -1 },
        { "Quantity", -1 },
        { "Unit of Measure Code", -1 },
    };

    public static readonly Dictionary<string, int> DematicPickEventColumns = new()
    {
        {"Timestamp", -1 },
        {"Operator ID", -1 },
        {"Operator Name", -1 },
        {"Qty", -1 },
        {"Container", -1 },
        {"Tech Type", -1 },
        {"Zone ID", -1 },
        {"Wave ID", -1 },
        {"Work Assignment", -1 },
        {"Store", -1 },
        {"Device ID", -1 },
        {"SKU ID", -1 },
        {"SKU Description", -1 },
        {"Cluster Ref", -1 },
    };

    public static readonly Dictionary<string, string> BinColumns = new()
    {
        { "location", "Location Code" },
        { "zone_code", "Zone Code" },
        { "code", "Code" },
        { "description", "Description" },
        { "empty", "Empty" },
        { "assigned", "Bin Assigned" },
        { "ranking", "Bin Ranking" },
        { "used_cube", "Used Cubage" },
        { "max_cube", "Maximum Cubage" },
        { "last_cc_date", "CC Last Count Date" },
        { "last_pi_date", "PI - Last Count Date" }
    };

    public static readonly Dictionary<string, string> UoMColumns = new()
    {
        { "code", "Code" },
        { "item_number", "Item No." },
        { "qty_per_uom", "Qty. per Unit of Measure" },
        { "max_qty", "Max Qty" },
        { "inner_pack", "Inner Pack" },
        { "exclude_cartonization", "Exclude Cartonization" },
        { "length", "Length (CM)" },
        { "width", "Width (CM)" },
        { "height", "Height (CM)" },
        { "cube", "CM Cubage" },
        { "weight", "Weight (Kg)" }
    };

    public static readonly Dictionary<string, string> StockColumns = new()
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

    public static readonly Dictionary<string, string> ItemColumns = new()
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

    public static readonly Dictionary<string, string> ZoneColumns = new()
    {
        { "code", "Code" },
        { "type", "Type" },
        { "description", "Description" }
    };
}