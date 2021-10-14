using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Text.Json;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using Olympus.Uranus.Inventory;
using Olympus.Uranus.Inventory.Model;
using System.Diagnostics;

namespace Olympus.Uranus
{
    public static class DataConversion
    {
        // Gets raw string data from the clipboard.
        public static string ClipboardToString()
        {
            string rawData = "";
            Thread thread = new Thread(delegate ()
            {
                rawData = Clipboard.GetText(TextDataFormat.Text);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return rawData;
        }

        // Set dictionary head positions, based on a string array from the head of the assumed data.
        public static void SetHeadPosFromArray(ref Dictionary<string, int> headDict, string[] headArr)
        {
            List<string> missingHeads = new List<string> { };

            foreach (string key in headDict.Keys.ToList())
            {
                headDict[key] = Array.IndexOf(headArr, key);
                if (headDict[key] == -1) missingHeads.Add(key);
            }
            if (missingHeads.Count > 0) throw new InvalidDataException(missingHeads);
        }

        // Turns clipboard data into a list of divisions.
        public static List<NAVTransferOrder> NAVClipToTransferOrders()
        {
            List<NAVTransferOrder> transferOrders = new List<NAVTransferOrder> { };

            Dictionary<string, int> headDict = Constants.NAV_TO_LINE_BATCH_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                transferOrders = NAVStreamToTransferOrders(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return transferOrders;
        }

        // Convert a memory stream into a Division list.
        private static List<NAVTransferOrder> NAVStreamToTransferOrders(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVTransferOrder> tos = new List<NAVTransferOrder> { };

            string[] row;
            int highestCol;
            NAVTransferOrder to;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int item)) item = 0;
                        if (!int.TryParse(row[headDict["Quantity"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int qty)) qty = 0;
                        if (!int.TryParse(row[headDict["Avail. UOM Fulfilment Qty"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int availQty)) availQty = 0;
                        if (!DateTime.TryParse(row[headDict["Created On Date"]], provider, DateTimeStyles.None, out DateTime date)) date = new DateTime();
                        if (!DateTime.TryParse(row[headDict["Created On Time"]], provider, DateTimeStyles.NoCurrentDateDefault, out DateTime time)) time = new DateTime();

                        to = new NAVTransferOrder
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
            }

            return tos;
        }

        // Turns clipboard data into a list of divisions.
        public static List<NAVDivision> NAVClipToDivisions()
        {
            List<NAVDivision> divisions = new List<NAVDivision> { };

            Dictionary<string, int> headDict = Constants.NAV_DIV_PF_GEN_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                divisions = NAVStreamToDivisions(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return divisions;
        }

        // Convert a memory stream into a Division list.
        private static List<NAVDivision> NAVStreamToDivisions(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVDivision> divs = new List<NAVDivision> { };

            string[] row;
            int highestCol;
            NAVDivision div;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int code)) code = 0;

                        div = new NAVDivision
                        {
                            Code = code,
                            Description = row[headDict["Description"]]
                        };
                        divs.Add(div);
                    }

                    line = reader.ReadLine();
                }
            }

            return divs;
        }

        // Turns clipboard data into a list of categories.
        public static List<NAVCategory> NAVClipToCategories()
        {
            List<NAVCategory> categories = new List<NAVCategory> { };

            Dictionary<string, int> headDict = Constants.NAV_CATEGORY_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                categories = NAVStreamToCategories(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return categories;
        }

        // Convert a memory stream into a Category list.
        private static List<NAVCategory> NAVStreamToCategories(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVCategory> cats = new List<NAVCategory> { };

            string[] row;
            int highestCol;
            NAVCategory cat;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int code)) code = 0;
                        if (!int.TryParse(row[headDict["Item Division Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int div)) div = 0;

                        cat = new NAVCategory
                        {
                            Code = code,
                            Description = row[headDict["Description"]],
                            DivisionCode = div
                        };
                        cats.Add(cat);
                    }

                    line = reader.ReadLine();
                }
            }

            return cats;
        }

        // Turns clipboard data into a list of platforms.
        public static List<NAVPlatform> NAVClipToPlatform()
        {
            List<NAVPlatform> platforms = new List<NAVPlatform> { };

            Dictionary<string, int> headDict = Constants.NAV_DIV_PF_GEN_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                platforms = NAVStreamToPlatforms(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return platforms;
        }

        // Convert a memory stream into a Platform list.
        private static List<NAVPlatform> NAVStreamToPlatforms(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVPlatform> pfList = new List<NAVPlatform> { };

            string[] row;
            int highestCol;
            NAVPlatform pf;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int code)) code = 0;

                        pf = new NAVPlatform
                        {
                            Code = code,
                            Description = row[headDict["Description"]]
                        };
                        pfList.Add(pf);
                    }

                    line = reader.ReadLine();
                }
            }

            return pfList;
        }

        // Turns clipboard data into a list of genres.
        public static List<NAVGenre> NAVClipToGenres()
        {
            List<NAVGenre> genres = new List<NAVGenre> { };

            Dictionary<string, int> headDict = Constants.NAV_DIV_PF_GEN_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                genres = NAVStreamToGenres(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return genres;
        }

        // Convert a memory stream into a Genre list.
        private static List<NAVGenre> NAVStreamToGenres(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVGenre> gens = new List<NAVGenre> { };

            string[] row;
            int highestCol;
            NAVGenre gen;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        if (!int.TryParse(row[headDict["Code"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int code)) code = 0;

                        gen = new NAVGenre
                        {
                            Code = code,
                            Description = row[headDict["Description"]]
                        };
                        gens.Add(gen);
                    }

                    line = reader.ReadLine();
                }
            }

            return gens;
        }

        // Turns clipboard data into a list of locations.
        public static List<NAVLocation> NAVClipToLocations()
        {
            List<NAVLocation> locations = new List<NAVLocation> { };

            Dictionary<string, int> headDict = Constants.NAV_LOCATION_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                locations = NAVStreamToLocations(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return locations;
        }

        // Convert a memory stream into a Location list.
        private static List<NAVLocation> NAVStreamToLocations(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVLocation> locs = new List<NAVLocation> { };

            string[] row;
            int highestCol;
            NAVLocation loc;

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        loc = new NAVLocation
                        {
                            Code = row[headDict["Code"]],
                            Name = row[headDict["Name"]],
                            CompanyCode = row[headDict["Company Code"]],
                            IsWarehouse = row[headDict["Location Is Warehouse"]] == "Yes",
                            IsStore = row[headDict["Location Is A Store"]] == "Yes",
                            ActiveForRepenishment = row[headDict["Active for Replenishment"]] == "Yes"
                        };
                        locs.Add(loc);
                    }

                    line = reader.ReadLine();
                }
            }

            return locs;
        }

        // Turns clipboard data into a list of zones.
        public static List<NAVZone> NAVClipToZones()
        {
            List<NAVZone> zones = new List<NAVZone> { };

            Dictionary<string, int> headDict = Constants.NAV_ZONE_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                zones = NAVStreamToZones(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return zones;
        }

        // Convert a memory stream into a Zone list.
        private static List<NAVZone> NAVStreamToZones(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVZone> zones = new List<NAVZone> { };

            string[] row;
            int highestCol;
            NAVZone zone;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        string locCode = row[headDict["Location Code"]];
                        string code = row[headDict["Code"]];
                        if (!int.TryParse(row[headDict["Zone Ranking"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int rank)) rank = 0;

                        zone = new NAVZone
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
            }

            return zones;
        }

        // Turns clipboard data into a list of bins.
        public static List<NAVBin> NAVClipToBins()
        {
            List<NAVBin> bins = new List<NAVBin> { };

            Dictionary<string, int> headDict = Constants.NAV_BIN_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                bins = NAVStreamToBins(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return bins;
        }

        // Convert a memory stream into a Bin list.
        private static List<NAVBin> NAVStreamToBins(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVBin> bins = new List<NAVBin> { };

            string[] row;
            int highestCol;
            NAVBin bin;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        string locCode = row[headDict["Location Code"]];
                        string zoneCode = row[headDict["Zone Code"]];
                        string code = row[headDict["Code"]];
                        string zoneID = string.Join(":", locCode, zoneCode);
                        if (!int.TryParse(row[headDict["Bin Ranking"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int rank)) rank = 0;
                        if (!double.TryParse(row[headDict["Used Cubage"]], NumberStyles.AllowThousands, provider, out double usedCube)) usedCube = 0;
                        if (!double.TryParse(row[headDict["Maximum Cubage"]], NumberStyles.AllowThousands, provider, out double maxCube)) maxCube = 0;
                        if (!DateTime.TryParse(row[headDict["CC Last Count Date"]], provider, DateTimeStyles.None, out DateTime ccCount)) ccCount = new DateTime();
                        if (!DateTime.TryParse(row[headDict["PI - Last Count Date"]], provider, DateTimeStyles.None, out DateTime piCount)) piCount = new DateTime();

                        bin = new NAVBin
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
            }

            return bins;
        }

        // Turns external data from predetermined CSV into a list of items.
        public static List<NAVItem> NAVCSVToItems()
        {
            List<NAVItem> items = new List<NAVItem> { };
            try
            {
                NAVItem item;
                IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
                Dictionary<string, int> headDict = Constants.NAV_ITEM_COLUMNS;
                using (StreamReader reader = new StreamReader(File.OpenRead(App.Settings.ItemCSVLocation)))
                {
                    string[] headArr = reader.ReadLine().Trim('"').Split(',', '"');
                    DataConversion.SetHeadPosFromArray(ref headDict, headArr);
                    string[] row;
                    string line = reader.ReadLine();

                    while (line != null)
                    {
                        row = line.Trim('"').Split(',', '"');

                        if (row[0] == "AU")
                        {
                            if (!int.TryParse(row[headDict["ItemCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int iNum)) iNum = 0;
                            if (!double.TryParse(row[headDict["Length"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double length)) length = 0;
                            if (!double.TryParse(row[headDict["Width"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double width)) width = 0;
                            if (!double.TryParse(row[headDict["Height"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double height)) height = 0;
                            if (!double.TryParse(row[headDict["Cubage"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double cube)) cube = 0;
                            if (!double.TryParse(row[headDict["Weight"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double weight)) weight = 0;
                            if (!int.TryParse(row[headDict["DivisionCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int div)) div = 0;
                            if (!int.TryParse(row[headDict["CategoryCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int cat)) cat = 0;
                            if (!int.TryParse(row[headDict["PlatformCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int pf)) pf = 0;
                            if (!int.TryParse(row[headDict["GenreCode"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int gen)) gen = 0;

                            item = new NAVItem
                            {
                                Number = iNum,
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
                }
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return items;
        }

        // Turns clipboard data into a list of uoms.
        public static List<NAVUoM> NAVClipToUoMs()
        {
            List<NAVUoM> uoms = new List<NAVUoM> { };

            Dictionary<string, int> headDict = Constants.NAV_UOM_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                uoms = NAVStreamToUoMs(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return uoms;
        }

        // Convert a memory stream into a UoM list.
        private static List<NAVUoM> NAVStreamToUoMs(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVUoM> uoms = new List<NAVUoM> { };

            string[] row;
            int highestCol;
            NAVUoM uom;
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');
                    if (highestCol < row.Length)
                    {
                        string code = row[headDict["Code"]];
                        if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int iNum)) iNum = 0;
                        if (!int.TryParse(row[headDict["Qty. per Unit of Measure"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int qtyPU)) qtyPU = 0;
                        if (!int.TryParse(row[headDict["Max Qty"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int max)) max = 0;
                        if (!double.TryParse(row[headDict["Length (CM)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double length)) length = 0;
                        if (!double.TryParse(row[headDict["Width (CM)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double width)) width = 0;
                        if (!double.TryParse(row[headDict["Height (CM)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double height)) height = 0;
                        if (!double.TryParse(row[headDict["CM Cubage"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double cube)) cube = 0;
                        if (!double.TryParse(row[headDict["Weight (Kg)"]], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double weight)) weight = 0;

                        uom = new NAVUoM
                        {
                            ID = string.Join(":", iNum, code),
                            Code = code,
                            ItemNumber = iNum,
                            QtyPerUoM = qtyPU,
                            MaxQty = max,
                            ExcludCartonization = row[headDict["Exclude Cartonization"]] == "Yes",
                            Length = length,
                            Width = width,
                            Height = height,
                            Cube = cube,
                            Weight = weight
                        };
                        uoms.Add(uom);
                    }
                    line = reader.ReadLine();
                }
            }
            return uoms;
        }

        // Turns clipboard data into a list of stock.
        public static List<NAVStock> NAVClipToStock()
        {
            List<NAVStock> stockList = new List<NAVStock> { };

            Dictionary<string, int> headDict = Constants.NAV_STOCK_COLUMNS;

            try
            {
                // Get raw data from clipboard and check that it has data.
                string rawData = ClipboardToString();

                if (rawData == "" || rawData == null) throw new InvalidDataException("No data on clipboard.", headDict.Keys.ToList());
                // Start memory stream from which to read.
                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);
                // Start Reading from stream.
                stockList = NAVStreamToStock(stream, headDict);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return stockList;
        }

        // Convert a memory stream into a stock list.
        private static List<NAVStock> NAVStreamToStock(MemoryStream stream, Dictionary<string, int> headDict)
        {
            List<NAVStock> stockList = new List<NAVStock> { };

            string[] row;
            int highestCol;
            NAVStock stock;

            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headArr = line.Split('\t');
                SetHeadPosFromArray(ref headDict, headArr);
                // Get highest column value to make sure that any given data line isn't cut short.
                highestCol = headDict.Values.Max();

                line = reader.ReadLine();
                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        string location = row[headDict["Location Code"]];
                        string zone = row[headDict["Zone Code"]];
                        string bin = row[headDict["Bin Code"]];
                        string uom = row[headDict["Unit of Measure Code"]];
                        string zoneID = string.Join(":", location, zone);
                        string binID = string.Join(":", zoneID, bin);
                        if (!int.TryParse(row[headDict["Item No."]], NumberStyles.Integer, provider, out int itemNo)) itemNo = 0;
                        string uomID = string.Join(":", itemNo, uom);
                        if (!int.TryParse(row[headDict["Quantity"]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int qty)) itemNo = 0;
                        if (!int.TryParse(row[headDict["Pick Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int pickQty)) itemNo = 0;
                        if (!int.TryParse(row[headDict["Put-away Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int putQty)) itemNo = 0;
                        if (!int.TryParse(row[headDict["Neg. Adjmt. Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int negQty)) itemNo = 0;
                        if (!int.TryParse(row[headDict["Pos. Adjmt. Qty."]], NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int posQty)) itemNo = 0;
                        if (!DateTime.TryParse(row[headDict["Date Created"]], provider, DateTimeStyles.None, out DateTime dateCreated)) dateCreated = new DateTime();
                        if (!DateTime.TryParse(row[headDict["Time Created"]], provider, DateTimeStyles.NoCurrentDateDefault, out DateTime timeCreated)) timeCreated = new DateTime();

                        stock = new NAVStock
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
            }

            return stockList;
        }

        // Reads data from the clipboard, assumes it is rectangular 2-dimensional data separated
        // by tabs and newlines. Converts it into an array.
        public static string[,] ClipboardToArray()
        {
            string rawData = ClipboardToString();

            string[] outerArray = rawData.Split(new[] { "\r\n" }, StringSplitOptions.None);
            int maxCol = outerArray[0].Split('\t').Length;
            int maxRow = outerArray.Length;
            string[,] fullArray = new string[maxRow, maxCol];
            string[] innerArray;

            for (int row = 0; row < outerArray.Length; ++row)
            {
                innerArray = outerArray[row].Split('\t');
                for (int col = 0; col < innerArray.Length && col < maxCol; ++col)
                {
                    fullArray[row, col] = innerArray[col];
                }
            }

            return fullArray;
        }

        // Takes a table-like array of data and converts it to a JSON string.
        public static string ArrayToJSON(string[,] array)
        {
            string returnString = "{\n\t";
            string line;
            int row, col, rowMax, colMax;
            rowMax = array.GetLength(0);
            colMax = array.GetLength(1);
            // Set headers.
            string[] headers = new string[colMax];
            for (col = 0; col < colMax; ++col)
            {
                headers[col] = array[0, col];
            }

            string head, val;

            // Set contents
            for (row = 1; row < rowMax; ++row)
            {
                line = "{\n\t\t";

                for (col = 0; col < colMax; ++col)
                {
                    head = headers[col] + ": ";
                    val = array[row, col];
                    line += head + ((col == colMax - 1) ? val + "\n\t" : val + ",\n\t\t");
                }

                line += (row == rowMax - 1) ? "}\n" : "},\n\t";

                returnString += line;
            }

            returnString += "}";

            return returnString;
        }

        // Takes data from the clipboard and converts it into a data table.
        // Assumes that data is separated by tabs and new lines.
        public static DataTable ClipboardToTable()
        {
            DataTable data = new DataTable();
            try
            {
                string rawData = ClipboardToString();
                if (rawData == "" || rawData == null) throw new Exception("No data on clipboard.");

                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);

                using (StreamReader reader = new StreamReader(stream))
                {
                    // First set the headers.
                    string line = reader.ReadLine();
                    string[] headers = line.Split('\t');
                    foreach (string header in headers)
                        data.Columns.Add(header);

                    line = reader.ReadLine();

                    // Add row data.
                    while (line != null)
                    {
                        data.Rows.Add(line.Split('\t'));
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }

            return data;
        }

        // Pulls data from a .csv file into a DataTable
        public static DataTable CSVToTable(string csvPath, List<string> columns, string conditions = "", bool includesHeaders = true)
        {
            string header = includesHeaders ? "Yes" : "No";

            DataTable data = new DataTable();
            // Check path validity.
            if (!(File.Exists(csvPath) && Path.GetExtension(csvPath) == ".csv")) return data;

            string csvDir = Path.GetDirectoryName(csvPath);
            string csvFile = Path.GetFileName(csvPath);
            string colStr = ((columns.Count > 0) ? string.Join(", ", columns) : "*");

            // Build query.
            string query = $"SELECT {colStr} FROM [{csvFile}] {((conditions == "") ? "" : $"WHERE {conditions}")};";

            // Connect and pull data.
            using (OleDbConnection connection = new OleDbConnection(
              @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + csvDir +
              ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(query, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                adapter.Fill(data);
                return data;
            }
        }

        // Given a datatable and a list of columns for each conversion type, sanitizes the data to convert all values
        // in those columns to the appropriate type.
        public static void ConvertColumns(ref DataTable dataTable, List<string> dblColumns, List<string> intColumns, List<string> dtColumns, List<string> boolColumns)
        {
            string[] trueVals = { "yes", "used" };
            bool b;
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (string col in dblColumns)
                {
                    Double.TryParse(row[col].ToString(), out double dbl);
                    row[col] = dbl;
                }
                foreach (string col in intColumns)
                {
                    int.TryParse(row[col].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int i);
                    row[col] = i;
                }
                foreach (string col in dtColumns)
                {
                    DateTime.TryParse(row[col].ToString(), out DateTime dateTime);
                    row[col] = dateTime;
                }
                foreach (string col in boolColumns)
                {
                    if (trueVals.Contains(row[col].ToString().ToLower()))
                        b = true;
                    else
                        Boolean.TryParse(row[col].ToString(), out b);
                    row[col] = b;
                }
            }
        }
    }
}
