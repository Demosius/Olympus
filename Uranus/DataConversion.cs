using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Uranus;

public static class DataConversion
{
    // Set dictionary head positions, based on a string array from the head of the assumed data.
    public static void SetHeadPosFromArray(ref Dictionary<string, int> headDict, string[] headArr)
    {
        List<string> missingHeads = new();

        foreach (var key in headDict.Keys.ToList())
        {
            headDict[key] = Array.IndexOf(headArr, key);
            if (headDict[key] == -1) missingHeads.Add(key);
        }
        if (missingHeads.Count > 0) throw new InvalidDataException(missingHeads);
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
        SetHeadPosFromArray(ref headDict, headArr);
        // Get highest column value to make sure that any given data line isn't cut short.
        var highestCol = headDict.Values.Max();

        line = reader.ReadLine();
        // Add row data.
        while (line is not null)
        {
            var row = line.Split('\t');

            if (highestCol < row.Length)
            {
                if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var item)) item = 0;
                if (!int.TryParse(row[headDict["Quantity"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qty)) qty = 0;
                if (!int.TryParse(row[headDict["Avail. UOM Fulfillment Qty"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var availQty)) availQty = 0;
                if (!DateTime.TryParse(row[headDict["Created On Date"]], provider, DateTimeStyles.None, out var date)) date = new DateTime();
                if (!DateTime.TryParse(row[headDict["Created On Time"]], provider, DateTimeStyles.NoCurrentDateDefault, out var time)) time = new DateTime();

                var to = new NAVTransferOrder
                {
                    ID = row[headDict["Document No."]],
                    StoreNumber = row[headDict["Transfer-to Code"]],
                    ItemNumber = item,
                    Qty = qty,
                    UoM = EnumConverter.StringToUoM(row[headDict["Unit of Measure"]]),
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
        SetHeadPosFromArray(ref headDict, headArr);
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
    private static List<NAVCategory> NAVStreamToCategories(MemoryStream stream, Dictionary<string, int> headDict)
    {
        List<NAVCategory> cats = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr);
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
    private static List<NAVPlatform> NAVStreamToPlatforms(MemoryStream stream, Dictionary<string, int> headDict)
    {
        List<NAVPlatform> pfList = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr);
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
        SetHeadPosFromArray(ref headDict, headArr);
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
    private static List<NAVLocation> NAVStreamToLocations(MemoryStream stream, Dictionary<string, int> headDict)
    {
        List<NAVLocation> locations = new();

        using StreamReader reader = new(stream)
            ;
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr);
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
    private static List<NAVZone> NAVStreamToZones(MemoryStream stream, Dictionary<string, int> headDict)
    {
        List<NAVZone> zones = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr);
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
    private static List<NAVBin> NAVStreamToBins(MemoryStream stream, Dictionary<string, int> headDict)
    {
        List<NAVBin> bins = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr);
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
        SetHeadPosFromArray(ref headDict, headArr);
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
    private static List<NAVUoM> NAVStreamToUoMs(MemoryStream stream, Dictionary<string, int> headDict)
    {
        List<NAVUoM> uomList = new();

        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
        using StreamReader reader = new(stream);
        // First set the headers.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();
        SetHeadPosFromArray(ref headDict, headArr);
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
        SetHeadPosFromArray(ref headDict, headArr);
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
                if (!int.TryParse(row[headDict["Quantity"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var qty)) itemNo = 0;
                if (!int.TryParse(row[headDict["Pick Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var pickQty)) itemNo = 0;
                if (!int.TryParse(row[headDict["Put-away Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var putQty)) itemNo = 0;
                if (!int.TryParse(row[headDict["Neg. Adjmt. Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var negQty)) itemNo = 0;
                if (!int.TryParse(row[headDict["Pos. Adjmt. Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out var posQty)) itemNo = 0;
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
}