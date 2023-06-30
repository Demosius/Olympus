using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using ExcelDataReader;
using Serilog;
using Uranus.Inventory;
using Uranus.Inventory.Models;
using Uranus.Staff.Models;
// ReSharper disable StringLiteralTypo

namespace Uranus;

public static class DataConversion
{
    // Set dictionary head positions, based on a string array from the head of the assumed data.
    public static void SetHeadPosFromArray(ref Dictionary<string, int> headDict, string[] headArr, string dataType)
    {
        List<string> missingHeads = new();

        foreach (var key in headDict.Keys)
        {
            headDict[key] = Array.IndexOf(headArr, key);
            if (headDict[key] == -1) missingHeads.Add(key);
        }

        if (missingHeads.Count > 0) throw new InvalidDataException($"Missing columns for {dataType} conversion.", missingHeads);
    }

    // Throw error if missing headers/columns.
    public static void CheckColumns(Dictionary<string, int> headDict, string[] headArr, string dataType)
    {
        var missingHeads = headDict.Keys.Where(key => Array.IndexOf(headArr, key) == -1).ToList();

        if (missingHeads.Any()) throw new InvalidDataException($"Missing columns for {dataType} conversion.", missingHeads);
    }

    /// <summary>
    /// Turns clipboard data into a list of divisions.
    /// </summary>
    /// <returns></returns>
    public static List<NAVTransferOrder> NAVRawStringToTransferOrders(string rawData)
    {
        var headDict = Constants.NAVToLineBatchColumns;

        // Get raw data from clipboard and check that it has data.
        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var transferOrders = NAVStreamToTransferOrders(stream, headDict);

        return transferOrders;
    }

    // Convert a memory stream into a Division list.
    private static List<NAVTransferOrder> NAVStreamToTransferOrders(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVTransferOrder> tos = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Transfer Orders");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line is not null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                var documentNumber = row[headDict["Document No."]];
                var uom = EnumConverter.StringToUoM(row[headDict["Unit of Measure"]]);
                if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var item)) item = 0;
                if (!int.TryParse(row[headDict["Quantity"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qty)) qty = 0;
                if (!int.TryParse(row[headDict["Avail. UOM Fulfilment Qty"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var availQty)) availQty = 0;
                if (!DateTime.TryParse(row[headDict["Created On Date"]], provider, DateTimeStyles.None, out var date)) date = new DateTime();
                if (!DateTime.TryParse(row[headDict["Created On Time"]], provider, DateTimeStyles.NoCurrentDateDefault, out var time)) time = new DateTime();

                var to = new NAVTransferOrder
                {
                    DocumentNumber = documentNumber,
                    ItemNumber = item,
                    ID = $"{documentNumber}:{item}:{uom}",
                    StoreNumber = row[headDict["Transfer-to Code"]],
                    Qty = qty,
                    UoM = uom,
                    AvailableQty = availQty,
                    CreationTime = date.Date.Add(time.TimeOfDay)
                };
                tos.Add(to);
            }

            line = reader.ReadLine();
        }

        return tos;
    }

    // Turns clipboard data into a list of divisions.
    public static List<NAVDivision> NAVRawStringToDivisions(string rawData)
    {
        var headDict = Constants.NAVDivPfGenColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var divisions = NAVStreamToDivisions(stream, headDict);

        return divisions;
    }

    // Convert a memory stream into a Division list.
    private static List<NAVDivision> NAVStreamToDivisions(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVDivision> divs = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Divisions");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var code)) code = 0;

                var div = new NAVDivision
                {
                    Code = code,
                    Description = row[headDict["Description"]]
                };
                divs.Add(div);
            }

            line = reader.ReadLine();
        }

        return divs;
    }

    // Turns clipboard data into a list of categories.
    public static List<NAVCategory> NAVRawStringToCategories(string rawData)
    {
        var headDict = Constants.NAVCategoryColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var categories = NAVStreamToCategories(stream, headDict);

        return categories;
    }

    // Convert a memory stream into a Category list.
    private static List<NAVCategory> NAVStreamToCategories(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVCategory> cats = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Categories");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var code)) code = 0;
                if (!int.TryParse(row[headDict["Item Division Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var div)) div = 0;

                var cat = new NAVCategory
                {
                    Code = code,
                    Description = row[headDict["Description"]],
                    DivisionCode = div
                };
                cats.Add(cat);
            }

            line = reader.ReadLine();
        }

        return cats;
    }

    // Turns clipboard data into a list of platforms.
    public static List<NAVPlatform> NAVRawStringToPlatform(string rawData)
    {
        var headDict = Constants.NAVDivPfGenColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var platforms = NAVStreamToPlatforms(stream, headDict);

        return platforms;
    }

    // Convert a memory stream into a Platform list.
    private static List<NAVPlatform> NAVStreamToPlatforms(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVPlatform> pfList = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Platforms");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var code)) code = 0;

                var pf = new NAVPlatform
                {
                    Code = code,
                    Description = row[headDict["Description"]]
                };
                pfList.Add(pf);
            }

            line = reader.ReadLine();
        }

        return pfList;
    }

    // Turns clipboard data into a list of genres.
    public static List<NAVGenre> NAVRawStringToGenres(string rawData)
    {
        var headDict = Constants.NAVDivPfGenColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var genres = NAVStreamToGenres(stream, headDict);

        return genres;
    }

    // Convert a memory stream into a Genre list.
    private static List<NAVGenre> NAVStreamToGenres(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVGenre> gens = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Genres");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var code)) code = 0;

                var gen = new NAVGenre
                {
                    Code = code,
                    Description = row[headDict["Description"]]
                };
                gens.Add(gen);
            }

            line = reader.ReadLine();
        }

        return gens;
    }

    // Turns clipboard data into a list of locations.
    public static List<NAVLocation> NAVRawStringToLocations(string rawData)
    {
        var headDict = Constants.NAVLocationColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var locations = NAVStreamToLocations(stream, headDict);

        return locations;
    }

    // Convert a memory stream into a Location list.
    private static List<NAVLocation> NAVStreamToLocations(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVLocation> locations = new();

        using StreamReader reader = new(stream)
            ;
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Locations");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                var loc = new NAVLocation
                {
                    Code = row[headDict["Code"]],
                    Name = row[headDict["Name"]],
                    CompanyCode = row[headDict["Company Code"]],
                    IsWarehouse = row[headDict["Location Is Warehouse"]] == "Yes",
                    IsStore = row[headDict["Location Is A Store"]] == "Yes",
                    ActiveForReplenishment = row[headDict["Active for Replenishment"]] == "Yes"
                };
                locations.Add(loc);
            }

            line = reader.ReadLine();
        }

        return locations;
    }

    // Turns clipboard data into a list of zones.
    public static List<NAVZone> NAVRawStringToZones(string rawData)
    {
        var headDict = Constants.NAVZoneColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var zones = NAVStreamToZones(stream, headDict);

        return zones;
    }

    // Convert a memory stream into a Zone list.
    private static List<NAVZone> NAVStreamToZones(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVZone> zones = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Zones");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                var locCode = row[headDict["Location Code"]];
                var code = row[headDict["Code"]];
                if (!int.TryParse(row[headDict["Zone Ranking"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var rank)) rank = 0;

                var zone = new NAVZone
                {
                    ID = String.Join(":", locCode, code),
                    Code = code,
                    LocationCode = locCode,
                    Description = row[headDict["Description"]],
                    Ranking = rank
                };
                zones.Add(zone);
            }

            line = reader.ReadLine();
        }

        return zones;
    }

    // Turns clipboard data into a list of bins.
    public static List<NAVBin> NAVRawStringToBins(string rawData)
    {
        var headDict = Constants.NAVBinColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var bins = NAVStreamToBins(stream, headDict);

        return bins;
    }

    // Convert a memory stream into a Bin list.
    private static List<NAVBin> NAVStreamToBins(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVBin> bins = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Bins");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                var locCode = row[headDict["Location Code"]];
                var zoneCode = row[headDict["Zone Code"]];
                var code = row[headDict["Code"]];
                var zoneID = string.Join(":", locCode, zoneCode);
                if (!int.TryParse(row[headDict["Bin Ranking"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var rank)) rank = 0;
                if (!double.TryParse(row[headDict["Used Cubage"]], NumberStyles.AllowThousands, provider, out var usedCube)) usedCube = 0;
                if (!double.TryParse(row[headDict["Maximum Cubage"]], NumberStyles.AllowThousands, provider, out var maxCube)) maxCube = 0;
                if (!DateTime.TryParse(row[headDict["CC Last Count Date"]], provider, DateTimeStyles.None, out var ccCount)) ccCount = new DateTime();
                if (!DateTime.TryParse(row[headDict["PI - Last Count Date"]], provider, DateTimeStyles.None, out var piCount)) piCount = new DateTime();

                var bin = new NAVBin
                {
                    ID = string.Join(":", zoneID, code),
                    ZoneID = zoneID,
                    LocationCode = locCode,
                    ZoneCode = zoneCode,
                    Code = code,
                    Description = row[headDict["Description"]],
                    Empty = row[headDict["Empty"]] == "Yes",
                    Assigned = row[headDict["Bin Assigned"]] == "Yes",
                    Ranking = rank,
                    UsedCube = usedCube,
                    MaxCube = maxCube,
                    LastCCDate = ccCount,
                    LastPIDate = piCount
                };
                bins.Add(bin);
            }

            line = reader.ReadLine();
        }

        return bins;
    }

    // Turns external data from predetermined CSV into a list of items.
    public static List<NAVItem> NAV_CSVToItems(string itemCSVLocation)
    {
        List<NAVItem> items = new();
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
        var headDict = Constants.NAVItemColumns;
        using StreamReader reader = new(File.OpenRead(itemCSVLocation));
        var headArr = reader.ReadLine()?.Trim('"').Split(',', '"') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Items");
        var line = reader.ReadLine();

        while (line != null)
        {
            var row = line.Trim('"').Split(',', '"');

            if (row[0] == "AU")
            {
                if (!int.TryParse(row[headDict["ItemCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var iNum)) iNum = 0;
                if (!double.TryParse(row[headDict["Length"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var length)) length = 0;
                if (!double.TryParse(row[headDict["Width"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var width)) width = 0;
                if (!double.TryParse(row[headDict["Height"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var height)) height = 0;
                if (!double.TryParse(row[headDict["Cubage"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var cube)) cube = 0;
                if (!double.TryParse(row[headDict["Weight"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var weight)) weight = 0;
                if (!int.TryParse(row[headDict["DivisionCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var div)) div = 0;
                if (!int.TryParse(row[headDict["CategoryCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var cat)) cat = 0;
                if (!int.TryParse(row[headDict["PlatformCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var pf)) pf = 0;
                if (!int.TryParse(row[headDict["GenreCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var gen)) gen = 0;

                var item = new NAVItem(iNum)
                {
                    Description = row[headDict["ItemName"]],
                    Barcode = row[headDict["PrimaryBarcode"]],
                    CategoryCode = cat,
                    DivisionCode = div,
                    PlatformCode = pf,
                    GenreCode = gen,
                    Length = length,
                    Width = width,
                    Height = height,
                    Cube = cube,
                    Weight = weight,
                    PreOwned = row[headDict["NewUsed"]] == "Used"
                };
                items.Add(item);
            }
            line = reader.ReadLine();
        }

        return items;
    }

    // Turns clipboard data into a list of UoMs.
    public static List<NAVUoM> NAVRawStringToUoMs(string rawData)
    {
        var headDict = Constants.NAV_UoMColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var uomList = NAVStreamToUoMs(stream, headDict);

        return uomList;
    }

    // Convert a memory stream into a UoM list.
    private static List<NAVUoM> NAVStreamToUoMs(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVUoM> uomList = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Units of Measurement");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');
            if (highestCol < row.Length)
            {
                var code = row[headDict["Code"]];
                if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var iNum)) iNum = 0;
                if (!int.TryParse(row[headDict["Qty. per Unit of Measure"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qtyPerUoM)) qtyPerUoM = 0;
                if (!int.TryParse(row[headDict["Max Qty"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var max)) max = 0;
                if (!double.TryParse(row[headDict["Length (CM)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var length)) length = 0;
                if (!double.TryParse(row[headDict["Width (CM)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var width)) width = 0;
                if (!double.TryParse(row[headDict["Height (CM)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var height)) height = 0;
                if (!double.TryParse(row[headDict["CM Cubage"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var cube)) cube = 0;
                if (!double.TryParse(row[headDict["Weight (Kg)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out var weight)) weight = 0;

                var uom = new NAVUoM
                {
                    ID = string.Join(":", iNum, code),
                    Code = code,
                    ItemNumber = iNum,
                    QtyPerUoM = qtyPerUoM,
                    MaxQty = max,
                    ExcludeCartonization = row[headDict["Exclude Cartonization"]] == "Yes",
                    Length = length,
                    Width = width,
                    Height = height,
                    Cube = cube,
                    Weight = weight
                };
                uomList.Add(uom);
            }
            line = reader.ReadLine();
        }

        return uomList;
    }

    // Turns clipboard data into a list of stock.
    public static List<NAVStock> NAVRawStringToStock(string rawData)
    {
        var headDict = Constants.NAVStockColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var stockList = NAVStreamToStock(stream, headDict);

        return stockList;
    }

    // Convert a memory stream into a stock list.
    private static List<NAVStock> NAVStreamToStock(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVStock> stockList = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Stock");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                var location = row[headDict["Location Code"]];
                var zone = row[headDict["Zone Code"]];
                var bin = row[headDict["Bin Code"]];
                var uom = row[headDict["Unit of Measure Code"]];
                if (!Enum.TryParse(uom, out EUoM _))
                {
                    uom = "EACH";
                }
                var zoneID = string.Join(":", location, zone);
                var binID = string.Join(":", zoneID, bin);
                if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer, provider, out var itemNo)) itemNo = 0;
                var uomID = string.Join(":", itemNo, uom);
                if (!int.TryParse(row[headDict["Quantity"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qty)) qty = 0;
                if (!int.TryParse(row[headDict["Pick Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var pickQty)) pickQty = 0;
                if (!int.TryParse(row[headDict["Put-away Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var putQty)) putQty = 0;
                if (!int.TryParse(row[headDict["Neg. Adjmt. Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var negQty)) negQty = 0;
                if (!int.TryParse(row[headDict["Pos. Adjmt. Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var posQty)) posQty = 0;
                if (!DateTime.TryParse(row[headDict["Date Created"]], provider, DateTimeStyles.None, out var dateCreated)) dateCreated = new DateTime();
                if (!DateTime.TryParse(row[headDict["Time Created"]], provider, DateTimeStyles.NoCurrentDateDefault, out var timeCreated)) timeCreated = new DateTime();

                var stock = new NAVStock
                {
                    ID = string.Join(":", binID, uomID),
                    BinID = binID,
                    ZoneID = zoneID,
                    UoMID = uomID,
                    LocationCode = location,
                    ZoneCode = zone,
                    BinCode = bin,
                    ItemNumber = itemNo,
                    UoMCode = uom,
                    Qty = qty,
                    PickQty = pickQty,
                    PutAwayQty = putQty,
                    NegAdjQty = negQty,
                    PosAdjQty = posQty,
                    DateCreated = dateCreated,
                    TimeCreated = timeCreated,
                    Fixed = row[headDict["Fixed"]] == "Yes"
                };
                stockList.Add(stock);
            }

            line = reader.ReadLine();
        }

        return stockList;
    }

    // Turns clipboard data into a list of stock.
    public static List<NAVMoveLine> NAVRawStringToMoveLines(string rawData)
    {
        var headDict = Constants.NAVMoveColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var moveLines = NAVStreamToMoveLines(stream, headDict);

        return moveLines;
    }

    // Convert a memory stream into a stock list.
    private static List<NAVMoveLine> NAVStreamToMoveLines(Stream stream, Dictionary<string, int> headDict)
    {
        List<NAVMoveLine> moveLines = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr, "Stock");
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                var zone = row[headDict["Zone Code"]];
                var bin = row[headDict["Bin Code"]];
                var zoneID = string.Join(":", "9600", zone);
                var binID = string.Join(":", zoneID, bin);

                if (!Enum.TryParse(row[headDict["Unit of Measure Code"]], out EUoM uom)) uom = EUoM.EACH;
                if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer, provider, out var itemNo)) itemNo = 0;
                if (!int.TryParse(row[headDict["Quantity"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qty)) itemNo = 0;
                if (!Enum.TryParse(row[headDict["Action Type"]], out EAction action)) action = EAction.Take;

                var moveLine = new NAVMoveLine
                {
                    ActionType = action,
                    ZoneID = zoneID,
                    BinID = binID,
                    ItemNumber = itemNo,
                    ZoneCode = zone,
                    BinCode = bin,
                    Qty = qty,
                    UoM = uom
                };
                moveLines.Add(moveLine);
            }

            line = reader.ReadLine();
        }

        return moveLines;
    }

    // Reads data from the clipboard, assumes it is rectangular 2-dimensional data separated
    // by tabs and newlines. Converts it into an array.
    public static string[,] RawStringToArray(string rawData)
    {
        var outerArray = rawData.Split(new[] { "\r\n" }, StringSplitOptions.None);
        var maxCol = outerArray[0].Split('\t').Length;
        var maxRow = outerArray.Length;
        var fullArray = new string[maxRow, maxCol];

        for (var row = 0; row < outerArray.Length; ++row)
        {
            var innerArray = outerArray[row].Split('\t');
            for (var col = 0; col < innerArray.Length && col < maxCol; ++col)
            {
                fullArray[row, col] = innerArray[col];
            }
        }

        return fullArray;
    }

    // Takes a table-like array of data and converts it to a JSON string.
    public static string ArrayToJson(string[,] array)
    {
        var returnString = "{\n\t";
        int row, col;
        var rowMax = array.GetLength(0);
        var colMax = array.GetLength(1);
        // Set headers.
        var headers = new string[colMax];
        for (col = 0; col < colMax; ++col)
        {
            headers[col] = array[0, col];
        }

        // Set contents
        for (row = 1; row < rowMax; ++row)
        {
            var line = "{\n\t\t";

            for (col = 0; col < colMax; ++col)
            {
                var head = headers[col] + ": ";
                var val = array[row, col];
                line += head + (col == colMax - 1 ? val + "\n\t" : val + ",\n\t\t");
            }

            line += row == rowMax - 1 ? "}\n" : "},\n\t";

            returnString += line;
        }

        returnString += "}";

        return returnString;
    }

    // Takes data from the clipboard and converts it into a data table.
    // Assumes that data is separated by tabs and new lines.
    public static DataTable RawStringToTable(string rawData)
    {
        DataTable data = new();
        if (string.IsNullOrEmpty(rawData)) throw new Exception("No data on clipboard.");

        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        if (line is null) return new DataTable();

        var headers = line.Split('\t');
        foreach (var header in headers)
            _ = data.Columns.Add(header);

        line = reader.ReadLine();

        // Add row data.
        while (line != null)
        {
            var group = line.Split('\t').ToArray<object>();
            _ = data.Rows.Add(group);
            line = reader.ReadLine();
        }

        return data;
    }

    // Given a data-table and a list of columns for each conversion type, sanitizes the data to convert all values
    // in those columns to the appropriate type.
    public static void ConvertColumns(ref DataTable dataTable, List<string> dblColumns, List<string> intColumns, List<string> dtColumns, List<string> boolColumns)
    {
        string[] trueValues = { "yes", "used" };
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
        foreach (DataRow row in dataTable.Rows)
        {
            foreach (var col in dblColumns)
            {
                _ = double.TryParse(row[col].ToString(), out var dbl);
                row[col] = dbl;
            }
            foreach (var col in intColumns)
            {
                _ = int.TryParse(row[col].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var i);
                row[col] = i;
            }
            foreach (var col in dtColumns)
            {
                _ = DateTime.TryParse(row[col].ToString(), out var dateTime);
                row[col] = dateTime;
            }
            foreach (var col in boolColumns)
            {
                bool b;
                if (trueValues.Contains(row[col].ToString()?.ToLower()))
                    b = true;
                else
                    _ = bool.TryParse(row[col].ToString(), out b);
                row[col] = b;
            }
        }
    }

    /****************************************** PICK EVENTS AND MISPICK DATA *****************************************/
    private static PickEvent? ArrayToPickEvent(IReadOnlyList<string> row, PickEventIndices col, int colMax, IFormatProvider provider)
    {
        if (colMax >= row.Count) return null;

        var timeStamp = row[col.Timestamp];
        if (!DateTime.TryParse(timeStamp, out var dateTime)) dateTime = DateTime.Today;
        var demID = row[col.OperatorID];
        var rfID = row[col.OperatorName];
        if (!int.TryParse(row[col.Qty], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qty)) qty = 0;
        var container = row[col.Container];
        var tech = row[col.Tech];
        var zone = row[col.ZoneID];
        var wave = row[col.WaveID];
        var workAss = row[col.WorkAssignment];
        var store = row[col.Store];
        var deviceID = row[col.DeviceID];
        if (!int.TryParse(row[col.SkuID], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var itemNo)) itemNo = 0;
        var itemDesc = row[col.SkuDescription];
        var cluster = row[col.Cluster];

        var pickEvent = new PickEvent
        {
            ID = PickEvent.GetEventID(demID, dateTime),
            TimeStamp = timeStamp,
            DateTime = dateTime,
            Date = dateTime.Date,
            OperatorDematicID = demID,
            OperatorRF_ID = rfID,
            Qty = qty,
            ContainerID = container,
            TechString = tech,
            TechType = PickEvent.GetTechType(tech),
            ZoneID = zone,
            WaveID = wave,
            WorkAssignment = workAss,
            StoreNumber = store,
            DeviceID = deviceID,
            ItemNumber = itemNo,
            ItemDescription = itemDesc,
            ClusterReference = cluster,
        };
        return pickEvent;
    }

    private static PickEvent DataRowToPickEvent(DataRow row, PickEventIndices col, IFormatProvider provider)
    {
        var timeStamp = row[col.Timestamp].ToString()!;
        if (!DateTime.TryParse(timeStamp, out var dateTime)) dateTime = DateTime.Today;
        var demID = row[col.OperatorID].ToString()!;
        var rfID = row[col.OperatorName].ToString()!;
        if (!int.TryParse(row[col.Qty].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qty)) qty = 0;
        var container = row[col.Container].ToString()!;
        var tech = row[col.Tech].ToString()!;
        var zone = row[col.ZoneID].ToString()!;
        var wave = row[col.WaveID].ToString()!;
        var workAss = row[col.WorkAssignment].ToString()!;
        var store = row[col.Store].ToString()!;
        var deviceID = row[col.DeviceID].ToString()!;
        if (!int.TryParse(row[col.SkuID].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var itemNo)) itemNo = 0;
        var itemDesc = row[col.SkuDescription].ToString()!;
        var cluster = row[col.Cluster].ToString()!;

        var pickEvent = new PickEvent
        {
            ID = PickEvent.GetEventID(demID, dateTime),
            TimeStamp = timeStamp,
            DateTime = dateTime,
            Date = dateTime.Date,
            OperatorDematicID = demID,
            OperatorRF_ID = rfID,
            Qty = qty,
            ContainerID = container,
            TechString = tech,
            TechType = PickEvent.GetTechType(tech),
            ZoneID = zone,
            WaveID = wave,
            WorkAssignment = workAss,
            StoreNumber = store,
            DeviceID = deviceID,
            ItemNumber = itemNo,
            ItemDescription = itemDesc,
            ClusterReference = cluster,
        };
        return pickEvent;
    }

    // Turns clipboard data into a list of Pick Events.
    public static List<PickEvent> RawStringToPickEvents(string rawData)
    {
        var headDict = Constants.DematicPickEventColumns;

        if (string.IsNullOrEmpty(rawData)) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var events = StreamToPickEvents(stream);

        return events;
    }

    private static List<PickEvent> StreamToPickEvents(Stream stream)
    {
        List<PickEvent> events = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        var col = new PickEventIndices(headArr);

        // Get highest column value to make sure that any given data line isn't cut short.
        var colMax = col.Max();

        line = reader.ReadLine();

        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            var pickEvent = ArrayToPickEvent(row, col, colMax, provider);

            if (pickEvent is not null) events.Add(pickEvent);

            line = reader.ReadLine();
        }

        return events;
    }

    public static async Task<List<PickEvent>> FileToPickEventsAsync(string filePath)
    {
        if (Path.GetExtension(filePath) == ".csv") return await CSVToPickEventsAsync(filePath);

        return Regex.IsMatch(Path.GetExtension(filePath), "\\.xls\\w?") ?
            await ExcelToPickEventsAsync(filePath) :
            new List<PickEvent>();
    }

    public static async Task<List<PickEvent>> CSVToPickEventsAsync(string csvPath)
    {
        List<PickEvent> events = new();
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(File.OpenRead(csvPath));

        var headArr = (await reader.ReadLineAsync())?.Trim('"').Split(',', '"') ?? Array.Empty<string>();
        var col = new PickEventIndices(headArr);
        var colMax = col.Max();

        var line = await reader.ReadLineAsync();

        while (line != null)
        {
            var row = line.Trim('"').Split(',', '"');

            var pickEvent = ArrayToPickEvent(row, col, colMax, provider);

            if (pickEvent is not null) events.Add(pickEvent);

            line = await reader.ReadLineAsync();
        }

        return events;
    }

    public static async Task<List<PickEvent>> ExcelToPickEventsAsync(string excelPath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        await using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            UseColumnDataType = true,
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                EmptyColumnNamePrefix = "Col",
                UseHeaderRow = true
            }
        });

        var events = new List<PickEvent>();

        foreach (DataTable table in dataSet.Tables)
            events.AddRange(DataTableToPickEvents(table));

        // If no mispicks, throw invalid data error.
        if (events.Count == 0)
            throw new InvalidDataException("No valid pick event data found from file.", new List<string>());

        return events;
    }

    public static List<PickEvent> DataTableToPickEvents(DataTable dataTable)
    {
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        PickEventIndices col;
        // Check headers.
        try
        {
            col = new PickEventIndices(GetTableHeaders(dataTable), true);
        }
        catch (InvalidDataException)
        {
            // This may represent a single page across many in a workbook. Do not throw the error.
            return new List<PickEvent>();
        }
        catch (Exception e)
        {
            Log.Error(e, "Unknown error when reading excel data.");
            throw;
        }

        // Iterate through rows.
        return (from DataRow row in dataTable.Rows select DataRowToPickEvent(row, col, provider)).ToList();
    }

    public static string[] GetTableHeaders(DataTable table)
    {
        var headers = new string[table.Columns.Count];
        var i = 0;
        foreach (DataColumn column in table.Columns)
        {
            headers[i] = column.ColumnName;
            i++;
        }
        return headers;
    }

    public static Dictionary<string, DataColumn> GetColumnDictionary(DataTable table) =>
        table.Columns.Cast<DataColumn>().ToDictionary(tableColumn => tableColumn.ColumnName);

    private static Mispick? ArrayToMispick(IReadOnlyList<string> row, MispickIndices col, int colMax,
        IFormatProvider provider)
    {
        if (colMax >= row.Count) return null;

        if (!DateTime.TryParse(row[col.ShipDate], out var shipDate)) shipDate = DateTime.Today;
        if (!DateTime.TryParse(row[col.ReceiveDate], out var recDate)) recDate = DateTime.Today;
        var cartonID = row[col.CartonID];
        if (!int.TryParse(row[col.ItemNumber], NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var itemNo)) itemNo = 0;
        var itemDesc = row[col.ItemDesc];
        var actionNotes = row[col.ActionNotes];
        if (!int.TryParse(row[col.OriginalQty], NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var originalQty)) originalQty = 0;
        if (!int.TryParse(row[col.ReceiveQty], NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var receivedQty)) receivedQty = 0;
        if (!int.TryParse(row[col.VarianceQty], NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var varianceQty)) varianceQty = 0;
        if (!DateTime.TryParse(row[col.PostDate], out var postedDate)) postedDate = DateTime.Today;

        var mispick = new Mispick
        {
            ID = Mispick.GetMispickID(cartonID, itemNo),
            ShipmentDate = shipDate,
            ReceivedDate = recDate,
            CartonID = cartonID,
            ItemNumber = itemNo,
            ItemDescription = itemDesc,
            ActionNotes = actionNotes,
            OriginalQty = originalQty,
            ReceivedQty = receivedQty,
            VarianceQty = varianceQty,
            PostedDate = postedDate,
            ErrorDate = shipDate,
        };
        return mispick;
    }

    private static Mispick DataRowToMispick(DataRow row, MispickIndices col, IFormatProvider provider)
    {
        if (!DateTime.TryParse(row[col.ShipDate].ToString(), out var shipDate)) shipDate = DateTime.Today;
        if (!DateTime.TryParse(row[col.ReceiveDate].ToString(), out var recDate)) recDate = DateTime.Today;
        var cartonID = row[col.CartonID].ToString()!;
        if (!int.TryParse(row[col.ItemNumber].ToString()!, NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var itemNo)) itemNo = 0;
        var itemDesc = row[col.ItemDesc].ToString()!;
        var actionNotes = row[col.ActionNotes].ToString()!;
        if (!int.TryParse(row[col.OriginalQty].ToString()!, NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var originalQty)) originalQty = 0;
        if (!int.TryParse(row[col.ReceiveQty].ToString()!, NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var receivedQty)) receivedQty = 0;
        if (!int.TryParse(row[col.VarianceQty].ToString()!, NumberStyles.Integer | NumberStyles.AllowThousands, provider,
                out var varianceQty)) varianceQty = 0;
        if (!DateTime.TryParse(row[col.PostDate].ToString()!, out var postedDate)) postedDate = DateTime.Today;

        var mispick = new Mispick
        {
            ID = Mispick.GetMispickID(cartonID, itemNo),
            ShipmentDate = shipDate,
            ReceivedDate = recDate,
            CartonID = cartonID,
            ItemNumber = itemNo,
            ItemDescription = itemDesc,
            ActionNotes = actionNotes,
            OriginalQty = originalQty,
            ReceivedQty = receivedQty,
            VarianceQty = varianceQty,
            PostedDate = postedDate,
            ErrorDate = shipDate,
        };
        return mispick;
    }

    public static List<Mispick> RawStringToMispicks(string rawData)
    {
        if (string.IsNullOrEmpty(rawData)) _ = new MispickIndices(new string[] { });
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var mispicks = StreamToMispicks(stream);

        return mispicks;
    }

    private static List<Mispick> StreamToMispicks(Stream stream)
    {
        List<Mispick> mispicks = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        var col = new MispickIndices(headArr);

        // Get highest column value to make sure that any given data line isn't cut short.
        var colMax = col.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            var mispick = ArrayToMispick(row, col, colMax, provider);

            if (mispick is not null) mispicks.Add(mispick);

            line = reader.ReadLine();
        }

        return mispicks;
    }

    public static List<Mispick> FileToMispicks(string filePath)
    {
        if (Path.GetExtension(filePath) == ".csv") return CSVToMispicks(filePath);

        return Regex.IsMatch(Path.GetExtension(filePath), "\\.xls\\w?") ?
            ExcelToMispicks(filePath) :
            new List<Mispick>();
    }

    public static async Task<List<Mispick>> FileToMispicksAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        if (extension == ".csv")
            return await CSVToMispicksAsync(filePath).ConfigureAwait(false);
        if (Regex.IsMatch(extension, "\\.xls\\w?"))
            return await ExcelToMispicksAsync(filePath).ConfigureAwait(false);
        return new List<Mispick>();
    }

    public static List<Mispick> CSVToMispicks(string csvPath)
    {
        List<Mispick> mispicks = new();
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(File.OpenRead(csvPath));

        var headArr = reader.ReadLine()?.Trim('"').Split(',', '"') ?? Array.Empty<string>();
        var col = new MispickIndices(headArr, true);
        var colMax = col.Max();

        var line = reader.ReadLine();

        while (line != null)
        {
            var row = line.Trim('"').Split(',', '"');

            var mispick = ArrayToMispick(row, col, colMax, provider);

            if (mispick is not null) mispicks.Add(mispick);

            line = reader.ReadLine();
        }

        return mispicks;
    }

    public static async Task<List<Mispick>> CSVToMispicksAsync(string csvPath)
    {
        List<Mispick> mispicks = new();

        using var reader = new StreamReader(File.OpenRead(csvPath));
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        await csvReader.ReadAsync();
        csvReader.ReadHeader();

        while (await csvReader.ReadAsync())
        {
            var mispick = csvReader.GetRecord<Mispick>();
            if (mispick != null)
            {
                mispicks.Add(mispick);
            }
        }

        return mispicks;
    }

    public static List<Mispick> ExcelToMispicks(string excelPath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            UseColumnDataType = true,
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                EmptyColumnNamePrefix = "Col",
                UseHeaderRow = true
            }
        });

        var mispicks = new List<Mispick>();

        foreach (DataTable table in dataSet.Tables)
            mispicks.AddRange(DataTableToMispicks(table));

        // If no mispicks, throw invalid data error.
        if (mispicks.Count > 0)
            throw new InvalidDataException("No valid mispick data found from file.", new List<string>());

        // Check for duplicates.
        mispicks = mispicks.Distinct().ToList();

        return mispicks;
    }

    public static async Task<List<Mispick>> ExcelToMispicksAsync(string excelPath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        await using var stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            UseColumnDataType = true,
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                EmptyColumnNamePrefix = "Col",
                UseHeaderRow = true
            }
        });

        var mispicks = dataSet.Tables.Cast<DataTable>().SelectMany(DataTableToMispicks).ToList();

        // If there is no data, throw invalid data exception.
        if (mispicks.Count == 0)
            throw new InvalidDataException($"Failed to pull valid mispick data from {excelPath}.", new List<string>());

        // Check for duplicates.
        mispicks = mispicks.Distinct().ToList();

        return mispicks;
    }

    public static List<Mispick> DataTableToMispicks(DataTable dataTable)
    {
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        MispickIndices col;
        // Check headers.
        try
        {
            col = new MispickIndices(GetTableHeaders(dataTable), true);
        }
        catch (InvalidDataException)
        {
            // This may represent a single page across many in a workbook. Do not throw the error.
            return new List<Mispick>();
        }
        catch (Exception e)
        {
            Log.Error(e, "Unknown error when reading excel data.");
            throw;
        }

        // Iterate through rows.
        return (from DataRow row in dataTable.Rows select DataRowToMispick(row, col, provider)).ToList();
    }

    /******************************** TOs AND BATCHES ******************************************/
    private static BatchTOLine? ArrayToBatchTOLine(IReadOnlyList<string> row, BatchTOLineIndices col, int colMax, IFormatProvider provider)
    {
        if (colMax >= row.Count) return null;

        var storeNo = row[col.StoreNo];
        if (!int.TryParse(row[col.Ctns], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var ctns)) ctns = 0;
        if (!double.TryParse(row[col.Weight], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var weight)) weight = 0;
        if (!double.TryParse(row[col.Cube], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var cube)) cube = 0;
        var ccn = row[col.CCN];
        var ctnType = row[col.CtnType];
        var startZone = row[col.StartZone];
        var endZone = row[col.EndZone];
        var startBin = row[col.StartBin];
        var endBin = row[col.EndBin];
        var batchNo = row[col.BatchNo];
        var dateString = row[col.Date];
        if (!DateTime.TryParse(dateString, out var date)) date = DateTime.Today;
        if (!int.TryParse(row[col.BaseUnits], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var baseUnits)) baseUnits = 0;
        var wave = row[col.WaveNumber];

        var batchTOLine = new BatchTOLine
        {
            StoreNo = storeNo,
            Cartons = ctns,
            Weight = weight,
            Cube = cube,
            CCN = ccn,
            CartonType = ctnType,
            StartZone = startZone,
            EndZone = endZone,
            StartBin = startBin,
            EndBin = endBin,
            BatchID = batchNo,
            Date = date,
            UnitsBase = baseUnits,
            WaveNo = wave,
        };

        return batchTOLine;
    }

    public static async Task<List<BatchTOLine>> FileToBatchTOLinesAsync(string filePath)
    {
        List<BatchTOLine> lines;
        if (Path.GetExtension(filePath) == ".csv")
            lines = await CSVToBatchTOLinesAsync(filePath);
        else
            lines = Regex.IsMatch(Path.GetExtension(filePath), "\\.xls\\w?")
                ? await ExcelToBatchTOLinesAsync(filePath)
                : new List<BatchTOLine>();

        var fileName = Path.GetFileName(filePath);
        foreach (var batchTOLine in lines)
        {
            batchTOLine.OriginalFileDirectory = Path.GetDirectoryName(filePath) ?? string.Empty;
            batchTOLine.OriginalFileName = fileName;
            batchTOLine.SetBays();
        }

        return lines;
    }

    public static async Task<List<BatchTOLine>> CSVToBatchTOLinesAsync(string csvPath)
    {
        List<BatchTOLine> lines = new();
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(File.OpenRead(csvPath));

        var headArr = (await reader.ReadLineAsync())?.Trim('"').Split(',', '"') ?? Array.Empty<string>();
        var col = new BatchTOLineIndices(headArr);
        var colMax = col.Max();

        var line = await reader.ReadLineAsync();

        while (line != null)
        {
            var row = line.Trim('"').Split(',', '"');

            var batchLine = ArrayToBatchTOLine(row, col, colMax, provider);

            if (batchLine is not null) lines.Add(batchLine);

            line = await reader.ReadLineAsync();
        }

        return lines;
    }

    public static async Task<List<BatchTOLine>> ExcelToBatchTOLinesAsync(string excelPath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        await using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            UseColumnDataType = true,
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                EmptyColumnNamePrefix = "Col",
                UseHeaderRow = true
            }
        });

        var lines = new List<BatchTOLine>();

        foreach (DataTable table in dataSet.Tables)
            lines.AddRange(DataTableToBatchTOLines(table));

        // If no mispicks, throw invalid data error.
        if (lines.Count == 0)
            throw new InvalidDataException("No valid pick event data found from file.", new List<string>());

        return lines;
    }

    public static List<BatchTOLine> DataTableToBatchTOLines(DataTable dataTable)
    {
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        BatchTOLineIndices col;
        // Check headers.
        try
        {
            col = new BatchTOLineIndices(GetTableHeaders(dataTable));
        }
        catch (InvalidDataException)
        {
            // This may represent a single page across many in a workbook. Do not throw the error.
            return new List<BatchTOLine>();
        }
        catch (Exception e)
        {
            Log.Error(e, "Unknown error when reading excel data.");
            throw;
        }

        // Iterate through rows.
        return (from DataRow row in dataTable.Rows select DataRowToBatchTOLine(row, col, provider)).ToList();
    }

    private static BatchTOLine DataRowToBatchTOLine(DataRow row, BatchTOLineIndices col, IFormatProvider provider)
    {
        var storeNo = row[col.StoreNo].ToString()!;
        if (!int.TryParse(row[col.Ctns].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var ctns)) ctns = 0;
        if (!double.TryParse(row[col.Weight].ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider, out var weight)) weight = 0;
        if (!double.TryParse(row[col.Cube].ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider, out var cube)) cube = 0;
        var ccn = row[col.CCN].ToString()!;
        var ctnType = row[col.CtnType].ToString()!;
        var startZone = row[col.StartZone].ToString()!;
        var endZone = row[col.EndZone].ToString()!;
        var startBin = row[col.StartBin].ToString()!;
        var endBin = row[col.EndBin].ToString()!;
        var batchNo = row[col.BatchNo].ToString()!;
        var dateString = row[col.Date].ToString()!;
        if (!DateTime.TryParse(dateString, out var date)) date = DateTime.Today;
        if (!int.TryParse(row[col.BaseUnits].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var baseUnits)) baseUnits = 0;
        var wave = row[col.WaveNumber].ToString()!;

        var batchTOLine = new BatchTOLine
        {
            StoreNo = storeNo,
            Cartons = ctns,
            Weight = weight,
            Cube = cube,
            CCN = ccn,
            CartonType = ctnType,
            StartZone = startZone,
            EndZone = endZone,
            StartBin = startBin,
            EndBin = endBin,
            BatchID = batchNo,
            Date = date,
            UnitsBase = baseUnits,
            WaveNo = wave,
        };

        return batchTOLine;
    }
    
    public static List<Batch> RawStringToBatches(string rawData)
    {
        if (string.IsNullOrEmpty(rawData)) _ = new BatchIndices(new string[] { });
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var batches = StreamToBatches(stream);

        return batches;
    }
    
    private static List<Batch> StreamToBatches(Stream stream)
    {
        List<Batch> batches = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        var col = new BatchIndices(headArr);

        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = col.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                var batchNo = row[col.BatchNo];
                if (!DateTime.TryParse(row[col.CreatedOn], out var createdOn)) createdOn = DateTime.Today;
                var createdBy = row[col.CreatedBy];
                var desc = row[col.Description];
                if (!DateTime.TryParse(row[col.LastTimeCartonizedDate], out var ctnDate)) ctnDate = DateTime.Today;
                if (!DateTime.TryParse(row[col.LastTimeCartonizedTime], out var ctnTime)) ctnTime = DateTime.Now;
                if (!int.TryParse(row[col.Cartons], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var cartons)) cartons = 0;
                if (!int.TryParse(row[col.Units], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var units)) units = 0;
                var ptlCreated = row[col.PTLFileCreated].ToUpper() == "YES";
                var cartonized = row[col.Cartonized].ToUpper() == "YES";
                var shipmentCreated = row[col.ShipmentCreated].ToUpper() == "YES";
                var fullyShipped = row[col.FullyShipped].ToUpper() == "YES";

                var batch = new Batch
                {
                    ID = batchNo,
                    CreatedOn = createdOn,
                    CreatedBy = createdBy,
                    Description = desc,
                    LastTimeCartonizedDate = ctnDate,
                    LastTimeCartonizedTime = ctnTime,
                    Cartons = cartons,
                    Units = units,
                    Priority = Batch.DetectPriority(desc),
                    TagString = string.Join(',', Batch.DetectTags(desc, batchNo).OrderBy(s => s)),
                    Progress = fullyShipped ? EBatchProgress.Completed : 
                        ptlCreated ? EBatchProgress.SentToPick : 
                        shipmentCreated ? EBatchProgress.AutoRun :
                        cartonized ? EBatchProgress.Cartonized : 
                        EBatchProgress.Created,
                };
                batches.Add(batch);
            }

            line = reader.ReadLine();
        }

        return batches;
    }
    
    public static List<PickLine> RawStringToPickLines(string rawData)
    {
        if (string.IsNullOrEmpty(rawData)) _ = new PickLineIndices(new string[] { });
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var pickLines = StreamToPickLines(stream);

        return pickLines;
    }
    
    private static List<PickLine> StreamToPickLines(Stream stream)
    {
        List<PickLine> pickLines = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        var col = new PickLineIndices(headArr, true);

        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = col.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                if (!Enum.TryParse(row[col.Action], out EAction action)) action = EAction.Take;
                var locationCode = row[col.LocationCode];
                var zoneCode = row[col.ZoneCode];
                var number = row[col.Number];
                if (!int.TryParse(row[col.LineNo], out var lineNo)) lineNo = 0;
                var cartonID = row[col.CartonID];
                var batchID = row[col.BatchNo];
                var pickerID = row[col.Description];
                var sourceNo = row[col.SourceNo];
                var sourceLineNo = row[col.SourceLineNo];
                var binCode = row[col.BinCode];
                if (!int.TryParse(row[col.ItemNumber], out var itemNumber)) itemNumber = 0;
                var description = row[col.Description];
                if (!int.TryParse(row[col.Quantity], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var quantity)) quantity = 0;
                if (!int.TryParse(row[col.QtyBase], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qtyBase)) qtyBase = 0;
                if (!int.TryParse(row[col.QtyPerUoM], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qtyPerUoM)) qtyPerUoM = 0;
                if (!Enum.TryParse(row[col.UoMCode], out EUoM uom)) uom = EUoM.EACH;
                if (!int.TryParse(row[col.QtyOutstanding], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qtyOutstanding)) qtyOutstanding = 0;
                if (!int.TryParse(row[col.QtyToHandle], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qtyToHandle)) qtyToHandle = 0;
                if (!int.TryParse(row[col.QtyHandled], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qtyHandled)) qtyHandled = 0;
                if (!DateTime.TryParse(row[col.DueDate], out var dueDate)) dueDate = DateTime.Today;

                var pickLine = new PickLine
                {
                    ActionType = action,
                    LocationCode = locationCode,
                    ZoneCode = zoneCode,
                    Number = number,
                    LineNumber = lineNo,
                    CartonID = cartonID,
                    BatchID = batchID,
                    PickerID = pickerID,
                    SourceNo = sourceNo,
                    SourceLineNo = sourceLineNo,
                    BinCode = binCode,
                    ItemNumber = itemNumber,
                    Description = description,
                    Qty = quantity,
                    BaseQty = qtyBase,
                    QtyPerUoM = qtyPerUoM,
                    UoM = uom,
                    QtyOutstanding = qtyOutstanding,
                    QtyToHandle = qtyToHandle,
                    QtyHandled = qtyHandled,
                    DueDate = dueDate,
                };
                pickLine.InitializeData();
                pickLines.Add(pickLine);
            }

            line = reader.ReadLine();
        }

        return pickLines;
    }

    /*********************************** STORES ***************************************/
    private static Store? ArrayToStore(IReadOnlyList<string> row, StoreIndices col, int colMax, IFormatProvider provider)
    {
        if (colMax >= row.Count) return null;

        var number = row[col.Store];
        var waveString = row[col.Wave];
        var waveNo = Regex.Match(waveString, "\\d+").Value;
        if (!int.TryParse(waveNo, NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var wave)) wave = 0;
        var ccnRegion = row[col.CCNRegion];
        var roadCCN = row[col.RoadCCN];
        if (!int.TryParse(row[col.ShippingDays], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var transitDays)) transitDays = 0;
        var mbRegion = row[col.MBRegion];
        var roadRegion = row[col.RoadRegion];
        if (!int.TryParse(row[col.SortingLane], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var sortingLane)) sortingLane = 0;
        var state = row[col.State];
        var region = row[col.Region];
        var storeTypeString = row[col.StoreType];
        var groups = Regex.Matches(storeTypeString, "(?i)(EBGAMES|ZING)");
        if (!Enum.TryParse(groups[0].Value, out EStoreType storeType)) storeType = EStoreType.EBGames;

        var store = new Store
        {
            Number = number,
            WaveNumber = wave,
            CCNRegion = ccnRegion,
            RoadCCN = roadCCN,
            TransitDays = transitDays,
            MBRegion = mbRegion,
            RoadRegion = roadRegion,
            SortingLane = sortingLane,
            State = state,
            Region = region,
            Type = storeType,
        };
        return store;
    }

    private static Store DataRowToStore(DataRow row, StoreIndices col, IFormatProvider provider)
    {
        var number = row[col.Store].ToString()!;
        var waveString = row[col.Wave].ToString()!;
        var waveNo = Regex.Match(waveString, "\\d+").Value;
        if (!int.TryParse(waveNo, NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var wave)) wave = 0;
        var ccnRegion = row[col.CCNRegion].ToString()!;
        var roadCCN = row[col.RoadCCN].ToString()!;
        if (!int.TryParse(row[col.ShippingDays].ToString()!, NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var transitDays)) transitDays = 0;
        var mbRegion = row[col.MBRegion].ToString()!;
        var roadRegion = row[col.RoadRegion].ToString()!;
        if (!int.TryParse(row[col.SortingLane].ToString()!, NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var sortingLane)) sortingLane = 0;
        var state = row[col.State].ToString()!;
        var region = row[col.Region].ToString()!;
        var storeTypeString = row[col.StoreType].ToString()!;
        var groups = Regex.Matches(storeTypeString, "(?i)(EBGAMES|ZING)");
        if (!Enum.TryParse(groups[0].Value, out EStoreType storeType)) storeType = EStoreType.EBGames;

        var store = new Store
        {
            Number = number,
            WaveNumber = wave,
            CCNRegion = ccnRegion,
            RoadCCN = roadCCN,
            TransitDays = transitDays,
            MBRegion = mbRegion,
            RoadRegion = roadRegion,
            SortingLane = sortingLane,
            State = state,
            Region = region,
            Type = storeType,
        };
        return store;
    }
    
    public static List<Store> RawStringToStores(string rawData)
    {
        if (string.IsNullOrEmpty(rawData)) _ = new StoreIndices(new string[] { });
        // Start memory stream from which to read.
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        MemoryStream stream = new(byteArray);
        // Start Reading from stream.
        var stores = StreamToStores(stream);

        return stores;
    }

    private static List<Store> StreamToStores(Stream stream)
    {
        List<Store> stores = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        var col = new StoreIndices(headArr);

        // Get highest column value to make sure that any given data line isn't cut short.
        var colMax = col.Max();

        line = reader.ReadLine();

        // Add row data.
        while (line != null)
        {
            var row = line.Split('\t');

            var store = ArrayToStore(row, col, colMax, provider);

            if (store is not null) stores.Add(store);

            line = reader.ReadLine();
        }

        return stores;
    }

    public static async Task<List<Store>> FileToStoresAsync(string filePath)
    {
        if (Path.GetExtension(filePath) == ".csv") return await CSVToStoresAsync(filePath);

        return Regex.IsMatch(Path.GetExtension(filePath), "\\.xls\\w?") ?
            await ExcelToStoresAsync(filePath) :
            new List<Store>();
    }

    public static async Task<List<Store>> CSVToStoresAsync(string csvPath)
    {
        List<Store> stores = new();
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(File.OpenRead(csvPath));

        var headArr = (await reader.ReadLineAsync())?.Trim('"').Split(',', '"') ?? Array.Empty<string>();
        var col = new StoreIndices(headArr);
        var colMax = col.Max();

        var line = await reader.ReadLineAsync();

        while (line != null)
        {
            var row = line.Trim('"').Split(',', '"');

            var store = ArrayToStore(row, col, colMax, provider);

            if (store is not null) stores.Add(store);

            line = await reader.ReadLineAsync();
        }

        return stores;
    }

    public static async Task<List<Store>> ExcelToStoresAsync(string excelPath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        await using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            UseColumnDataType = true,
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                EmptyColumnNamePrefix = "Col",
                UseHeaderRow = true
            }
        });

        var stores = new List<Store>();

        foreach (DataTable table in dataSet.Tables)
            stores.AddRange(DataTableToStores(table));

        // If no mispicks, throw invalid data error.
        if (stores.Count == 0)
            throw new InvalidDataException("No valid store data found from file.", new List<string>());

        return stores;
    }

    public static List<Store> DataTableToStores(DataTable dataTable)
    {
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        StoreIndices col;
        // Check headers.
        try
        {
            col = new StoreIndices(GetTableHeaders(dataTable), true);
        }
        catch (InvalidDataException)
        {
            // This may represent a single page across many in a workbook. Do not throw the error.
            return new List<Store>();
        }
        catch (Exception e)
        {
            Log.Error(e, "Unknown error when reading excel data.");
            throw;
        }

        // Iterate through rows.
        return (from DataRow row in dataTable.Rows select DataRowToStore(row, col, provider)).ToList();
    }
}