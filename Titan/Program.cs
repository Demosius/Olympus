using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympus.Helios.Inventory;
using Olympus.Helios;
using Olympus.Helios.Users;
using Olympus.Helios.Equipment;
using Olympus.Helios.Staff;
using Olympus;
using System.Text.Json;
using System.Data;
using System.Data.SQLite.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.CodeDom;
using Olympus.Model;
using System.Windows;
using SQLite;
using System.Globalization;
using System.Data.OleDb;
using Olympus.Helios.Inventory.Model;

namespace Titan
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(App.Settings.SolLocation);

            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();



            _ = Console.ReadLine();
        }

        public static List<NAVItem> CSVToDTTimer()
        {
            Console.WriteLine("*********************** CSV to Datatable ***********************");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            DataTable dataTable = DataConversion.CSVToTable(App.Settings.ItemCSVLocation, Constants.ITEM_COLUMNS.Values.ToList(), "CompanyCode = 'AU'");

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to read CSV to DataTable.");
            stopwatch.Restart();

            DataConversion.ConvertColumns(
                dataTable: ref dataTable,
                dblColumns: new List<string> { "Length", "Width", "Height", "Cubage", "Weight" },
                intColumns: new List<string> { "ItemCode", "DivisionCode", "CategoryCode", "PlatformCode", "GenreCode" },
                dtColumns: new List<string> { },
                boolColumns: new List<string> { "NewUsed" }
                );

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for table to convert data.");
            stopwatch.Restart();
            
            List<NAVItem> items = new List<NAVItem> { };
            NAVItem item;
            foreach (DataRow row in dataTable.Rows)
            {
                item = new NAVItem
                {
                    Number = int.Parse(row["ItemCode"].ToString()),
                    Description = row["ItemName"].ToString(),
                    Barcode = row["PrimaryBarcode"].ToString(),
                    CategoryCode = int.Parse(row["CategoryCode"].ToString()),
                    DivisionCode = int.Parse(row["DivisionCode"].ToString()),
                    PlatformCode = int.Parse(row["PlatformCode"].ToString()),
                    GenreCode = int.Parse(row["GenreCode"].ToString()),
                    Length = double.Parse(row["Length"].ToString()),
                    Width = double.Parse(row["Width"].ToString()),
                    Height = double.Parse(row["Height"].ToString()),
                    Cube = double.Parse(row["Cubage"].ToString()),
                    Weight = double.Parse(row["Weight"].ToString()),
                    PreOwned = bool.Parse(row["NewUsed"].ToString())
                };
                items.Add(item);
            };

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to convert datatable to Object List ({items.Count}).\n");

            return items;
        }

        public static List<NAVItem> CSVToFileStreamTimer()
        {
            Console.WriteLine("*********************** CSV to FileStream ***********************");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<NAVItem> items = new List<NAVItem> { };
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
            
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to read CSV to FileStream then convert to Object list ({items.Count}).\n");

            return items;
        }

        public static List<NAVItem> CSVOLEDBReaderToOListTimer()
        {
            Console.WriteLine("*********************** OleDB Reader - CSV to Object List ***********************");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string csvPath = App.Settings.ItemCSVLocation;
            List<string> columns = Constants.ITEM_COLUMNS.Values.ToList();
            List<NAVItem> items = new List<NAVItem> { };
            NAVItem item;
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");

            // Check path validity.
            if (!(File.Exists(csvPath) && Path.GetExtension(csvPath) == ".csv")) return items;

            string csvDir = Path.GetDirectoryName(csvPath);
            string csvFile = Path.GetFileName(csvPath);
            string colStr = ((columns.Count > 0) ? string.Join(", ", columns) : "*");

            // Build query.
            string query = $"SELECT {colStr} FROM [{csvFile}] WHERE CompanyCode = 'AU';";

            // Connect and pull data.
            using (OleDbConnection connection = new OleDbConnection(
              @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + csvDir +
              ";Extended Properties=\"Text;HDR=Yes\""))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                connection.Open();
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!int.TryParse(reader["ItemCode"].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int iNum)) iNum = 0;
                        if (!double.TryParse(reader["Length"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double length)) length = 0;
                        if (!double.TryParse(reader["Width"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double width)) width = 0;
                        if (!double.TryParse(reader["Height"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double height)) height = 0;
                        if (!double.TryParse(reader["Cubage"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double cube)) cube = 0;
                        if (!double.TryParse(reader["Weight"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider, out double weight)) weight = 0;
                        if (!int.TryParse(reader["DivisionCode"].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int div)) div = 0;
                        if (!int.TryParse(reader["CategoryCode"].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int cat)) cat = 0;
                        if (!int.TryParse(reader["PlatformCode"].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int pf)) pf = 0;
                        if (!int.TryParse(reader["GenreCode"].ToString(), NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int gen)) gen = 0;


                        item = new NAVItem
                        {
                            Number = iNum,
                            Description = reader["ItemName"].ToString(),
                            Barcode = reader["PrimaryBarcode"].ToString(),
                            CategoryCode = cat,
                            DivisionCode = div,
                            PlatformCode = pf,
                            GenreCode = gen,
                            Length = length,
                            Width = width,
                            Height = height,
                            Cube = cube,
                            Weight = weight,
                            PreOwned = reader["NewUsed"].ToString() == "Used"
                        };
                        items.Add(item);
                    }
                }

            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to read CSV directly to object list ({items.Count}).\n");

            return items;
        }

        public static List<NAVItem> CSVFileReadToOListTimer()
        {
            Console.WriteLine("*********************** CSV File Read into string then to Object List ***********************");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string[] lines = File.ReadAllLines(App.Settings.ItemCSVLocation);

            string[] headArr = lines[0].Trim('"').Split(',', '"');
            string[] row;

            List<NAVItem> items = new List<NAVItem> { };
            NAVItem item;
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
            Dictionary<string, int> headDict = Constants.NAV_ITEM_COLUMNS;
            DataConversion.SetHeadPosFromArray(ref headDict, headArr);

            for (int r = 1; r<lines.Length; ++r)
            {
                row = lines[r].Trim('"').Split(',','"');

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
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for CSV fileRead to get string and convert to Object List ({items.Count}).\n");
            return items;
        }

        public static void SysDatSqlReadTest()
        {
            Console.WriteLine("*******************System.Data.SQLite**********************");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _ = GetInventory.StockTable();
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to read stock to DataTable.");
        }

        public static void SQLNetReadTest()
        {
            SQLiteConnection conn = null;
            conn = SQLTesting.InitializeLocalDatabase(conn, typeof(Stock));
            Console.WriteLine("*******************sqlite-net-pcl**********************");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Stock> stock = conn.Table<Stock>().ToList();
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to read stock to Stock List.");

            _ = Console.ReadLine();

            foreach (var s in stock)
            {
                Console.WriteLine($"{s.Location}, {s.ZoneCode}, {s.BinCode}, {s.ItemNumber}, {s.DateCreated}, {s.TimeCreated}");
            }
        }

        public static void ClipToDBTimeTest()
        {
            SQLiteConnection conn = null;
            conn = SQLTesting.InitializeLocalDatabase(conn, typeof(Stock));
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();

            Console.WriteLine("*******************System.Data.SQLite**********************");

            Stopwatch stopwatch = new Stopwatch();

            // Time for Clip to Datatable
            stopwatch.Start();

            DataTable dataTable = DataConversion.ClipboardToTable();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for clip to table.");

            // Time to convert columns to correct format.
            stopwatch.Restart();

            DataConversion.ConvertColumns(
                dataTable: ref dataTable,
                dblColumns: new List<string> { },
                intColumns: new List<string> { "Item No.", "Quantity", "Pick Qty.", "Put-away Qty.", "Neg. Adjmt. Qty.", "Pos. Adjmt. Qty." },
                dtColumns: new List<string> { "Date Created", "Time Created" },
                boolColumns: new List<string> { }
                );

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for table to convert data.");

            // Time to convert Datatable to list<object>
            stopwatch.Restart();
            List<Stock> stocks = new List<Stock> { };
            Stock stock;
            foreach (DataRow row in dataTable.Rows)
            {
                stock = new Stock
                {
                    Location = row["Location Code"].ToString(),
                    ZoneCode = row["Zone Code"].ToString(),
                    BinCode = row["Bin Code"].ToString(),
                    ItemNumber = int.Parse(row["Item No."].ToString()),
                    Barcode = row["ItemBarcode"].ToString(),
                    UoMCode = row["Unit of Measure Code"].ToString(),
                    Qty = int.Parse(row["Quantity"].ToString()),
                    PickQty = int.Parse(row["Pick Qty."].ToString()),
                    PutAwayQty = int.Parse(row["Put-away Qty."].ToString()),
                    NegAdjQty = int.Parse(row["Neg. Adjmt. Qty."].ToString()),
                    PosAdjQty = int.Parse(row["Pos. Adjmt. Qty."].ToString()),
                    DateCreated = DateTime.Parse(row["Date Created"].ToString()),
                    TimeCreated = DateTime.Parse(row["Time Created"].ToString()),
                    Fixed = row["Fixed"].ToString() == "Yes"
                };
                stocks.Add(stock);
            };

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for table to Object.");

            // Time to clear Main DB (System.Data.SQLite)
            stopwatch.Restart();
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            chariot.EmptyTable("Stock");
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to clear main DB Stock. (System.Data.SQLite)");

            stocks = new List<Stock> { };

            // Time to get object list into db. (System.Data.SQLite)
            stopwatch.Restart();
            chariot.StockTableUpdate(dataTable);
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to load Stock into main DB. (System.Data.SQLite) (Includes other table, and 'delete')\n");

            Console.WriteLine("*******************sqlite-net-pcl**********************");
            // Time to get Object list from clipboard directly.
            stopwatch.Restart();

            stocks = StockFromClip();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for Clipboard to Object.");

            // Time to clear new DB (sqlite-net-pcl)
            stopwatch.Restart();
            conn.RunInTransaction(() =>
            {
                conn.DeleteAll<Stock>();
            });
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to clear new DB stock. (sqlite-net-pcl)");


            // Time to get DataTable into DB. (sqlite-net-pcl)
            stopwatch.Restart();
            conn.RunInTransaction(() =>
            {
                conn.InsertAll(stocks);
            });
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to load Stock into new DB. (sqlite-net-pcl)");

        }

        public static List<Stock> StockFromClip()
        {
            List<Stock> stocks = new List<Stock> { };
            Dictionary<string, int> head = new Dictionary<string, int>
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
            string[] row;
            int highestCol;
            Stock stock;
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-AU");
            //try
            //{
            string rawData = DataConversion.ClipboardToString();
                if (rawData == "" || rawData == null) throw new Exception("No data on clipboard.");

                byte[] byteArray = Encoding.UTF8.GetBytes(rawData);
                MemoryStream stream = new MemoryStream(byteArray);

            using (StreamReader reader = new StreamReader(stream))
            {
                // First set the headers.
                string line = reader.ReadLine();
                string[] headers = line.Split('\t');

                foreach (string key in head.Keys.ToList())
                {
                    head[key] = Array.IndexOf(headers, key);
                    if (head[key] == -1) throw new Exception($"Missing header: {key}.");
                }

                highestCol = head.Values.Max();

                line = reader.ReadLine();

                // Add row data.
                while (line != null)
                {
                    row = line.Split('\t');

                    if (highestCol < row.Length)
                    {
                        if (!int.TryParse(row[head["Item No."]], NumberStyles.Integer, provider, out int itemNo)) itemNo = 0;
                        if (!int.TryParse(row[head["Quantity"]], NumberStyles.Integer, provider, out int qty)) itemNo = 0;
                        if (!int.TryParse(row[head["Pick Qty."]], NumberStyles.Integer, provider, out int pickQty)) itemNo = 0;
                        if (!int.TryParse(row[head["Put-away Qty."]], NumberStyles.Integer, provider, out int putQty)) itemNo = 0;
                        if (!int.TryParse(row[head["Neg. Adjmt. Qty."]], NumberStyles.Integer, provider, out int negQty)) itemNo = 0;
                        if (!int.TryParse(row[head["Pos. Adjmt. Qty."]], NumberStyles.Integer, provider, out int posQty)) itemNo = 0;
                        if (!DateTime.TryParse(row[head["Date Created"]], provider, DateTimeStyles.None, out DateTime dateCreated)) dateCreated = new DateTime();
                        if (!DateTime.TryParse(row[head["Time Created"]], provider, DateTimeStyles.NoCurrentDateDefault, out DateTime timeCreated)) timeCreated = new DateTime();
                        
                        stock = new Stock
                        {
                            Location = row[head["Location Code"]],
                            ZoneCode = row[head["Zone Code"]],
                            BinCode = row[head["Bin Code"]],
                            ItemNumber = itemNo,
                            Barcode = row[head["ItemBarcode"]],
                            UoMCode = row[head["Unit of Measure Code"]],
                            Qty = qty,
                            PickQty = pickQty,
                            PutAwayQty = putQty,
                            NegAdjQty = negQty,
                            PosAdjQty = posQty,
                            DateCreated = dateCreated,
                            TimeCreated = timeCreated,
                            Fixed = row[head["Fixed"]] == "Yes"
                        };

                        stocks.Add(stock);
                    }

                    line = reader.ReadLine();
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    Toolbox.ShowUnexpectedException(ex);
            //}

            return stocks;
        }

        public static void SQLiteNetPCL()
        {
            SQLiteConnection db = null;
            db = SQLTesting.InitializeLocalDatabase(db, typeof(Stock), typeof(Bin), typeof(Item), typeof(UoM));

            Console.WriteLine(db.GetMapping(typeof(Item)).ToString());
        }

        public static void MessageIconCheck()
        {
            MessageBox.Show("Asterisk", "Asterisk", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show("Exclamation", "Exclamation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            MessageBox.Show("Hand", "Hand", MessageBoxButton.OK, MessageBoxImage.Hand);
            MessageBox.Show("Information", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            MessageBox.Show("None", "None", MessageBoxButton.OK, MessageBoxImage.None);
            MessageBox.Show("Question", "Question", MessageBoxButton.OK, MessageBoxImage.Question);
            MessageBox.Show("Stop", "Stop", MessageBoxButton.OK, MessageBoxImage.Stop);
            MessageBox.Show("Warning", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ContainsExistsAny()
        {
            Console.WriteLine("***************************************");
            Console.WriteLine("********* ContainsExistsAny ***********");
            Console.WriteLine("***************************************");

            List<int> list = new List<int>(6000000);
            Random random = new Random();
            for (int i = 0; i < 6000000; i++)
            {
                list.Add(random.Next(6000000));
            }
            int[] arr = list.ToArray();
            HashSet<int> set = new HashSet<int>(list);

            Find(list, arr, set);

        }

        public static void ContainsExistsAnyShort()
        {
            Console.WriteLine("***************************************");
            Console.WriteLine("***** ContainsExistsAnyShortRange *****");
            Console.WriteLine("***************************************");

            List<int> list = new List<int>(2000);
            Random random = new Random();
            for (int i = 0; i < 2000; i++)
            {
                list.Add(random.Next(6000000));
            }
            int[] arr = list.ToArray();
            HashSet<int> set = new HashSet<int>(list);

            Find(list, arr, set);

        }

        public static void Find(List<int> list, int[] arr, HashSet<int> set)
        {
            Random random = new Random();
            int[] find = new int[10000];
            for (int i = 0; i < 10000; i++)
            {
                find[i] = random.Next(6000000);
            }

            Stopwatch watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                list.Contains(find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("List/Contains: {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                list.Exists(a => a == find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("List/Exists: {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                list.Any(a => a == find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("List/Any: {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                arr.Contains(find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("Array/Contains: {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                Array.Exists(arr, element => element == find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("Array/Exists: {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                arr.Any(a => a == find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("Array/Any: {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                set.Contains(find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("HashSet/Contains: {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            for (int rpt = 0; rpt < 10000; rpt++)
            {
                set.Any(a => a == find[rpt]);
            }
            watch.Stop();
            Console.WriteLine("HashSet/Any: {0}ms", watch.ElapsedMilliseconds);
        }

        public static void TestBCMethods()
        {
            int count = 20;
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();
            _ = GetInventory.BinTable();
            Stopwatch stopwatch;

            stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++) 
            {
                _ = GetInventory.SimpleBins();
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds / count}ms for Simples from datatable.");

            stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                _ = GetInventory.BinTable();
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds / count}ms for just DataTable.");

            stopwatch = new Stopwatch();
            stopwatch.Start();

            //List<Object> lo;
            //for (int i = 0; i < count; i++)
            //{
            //    lo = GetInventory.BinList();
            //}

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds / count}ms for Object List by reader.");

            stopwatch = new Stopwatch();
            stopwatch.Start();

            //List<SimpleBin> ss;
            //for (int i = 0; i < count; i++)
            //{
            //    ss = GetInventory.SimpleBinList();
            //}

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds / count}ms for SimpleBin List by reader.");

            /*ss = GetInventory.SimpleBinList();
            foreach (var item in ss)
            {
                Console.WriteLine(item.MaxCube);
            }
*/
            /*int j = 0;
            foreach (var item in lo)
            {
                Console.WriteLine($"{j} : {item.ToString()});
            }*/

            /*foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine(row["max_cube"].ToString());
            }*/
            /*
                        foreach (var item in ls)
                        {
                            Console.WriteLine(item.MaxCube);
                        }
            */
        }

        public static void TestSets()
        {
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"\nSTAFF:");
            MasterChariot chariot = new StaffChariot(App.Settings.SolLocation);
            DataSet set = chariot.PullFullDataSet();
            foreach (DataTable table in set.Tables)
            {
                Console.WriteLine(table.TableName);
            }
            DataTable dataTable = set.Tables["clan"];
            Console.WriteLine(dataTable.Columns.Count);
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for Staff.");

            stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"\n\nINVENTORY:");
            chariot = new InventoryChariot(App.Settings.SolLocation);
            set = chariot.PullFullDataSet();
            foreach (DataTable table in set.Tables)
            {
                Console.WriteLine(table.TableName);
            }
            dataTable = set.Tables["bin"];
            Console.WriteLine(dataTable.Columns.Count);
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for Inventory.");

            stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"\n\nEQUIPMENT:");
            chariot = new EquipmentChariot(App.Settings.SolLocation);
            set = chariot.PullFullDataSet();
            foreach (DataTable table in set.Tables)
            {
                Console.WriteLine(table.TableName);
            }
            dataTable = set.Tables["machine"];
            Console.WriteLine(dataTable.Columns.Count);
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for Equipment.");

            stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"\n\nUSERS:");
            chariot = new UserChariot(App.Settings.SolLocation);
            set = chariot.PullFullDataSet();
            foreach (DataTable table in set.Tables)
            {
                Console.WriteLine(table.TableName);
            }
            dataTable = set.Tables["role"];
            Console.WriteLine(dataTable.Columns.Count);
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for Users.");
        }

        public static void CheckColsAfterJoin()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            var data = chariot.GetBinsWithContents();
            foreach (DataColumn column in data.Columns)
            {
                Console.WriteLine(column.ColumnName);
            }
        }

        public static void StringBuilding(int count)
        {
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();
            Stopwatch stopwatch;
            string concat = "";
            if (count < 100000)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();

                concat = "";
                for (int i = 0; i < count; ++i)
                    concat += i.ToString();

                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms for standard concatenation.");
            }
            stopwatch = new Stopwatch();
            stopwatch.Start();

            List<string> vs = new List<string> { };
            for (int i = 0; i < count; ++i)
                vs.Add(i.ToString());
            string join = string.Join("", vs);

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms using a List Join.");
            stopwatch = new Stopwatch();
            stopwatch.Start();

            StringBuilder build = new StringBuilder();
            for (int i = 0; i < count; ++i)
                build.Append(i);

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms using a string builder.");

            Console.WriteLine(concat == build.ToString());
            Console.WriteLine(concat == join);
            Console.WriteLine(join == build.ToString());
        }

        public static void TestBCDeserial()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            BinContents bc = GetInventory.BCFromFile(@"C:\Users\aarop\BC_2ds0210601-0858.json");

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to pull and deserialize bin cointents from file.");

            Console.WriteLine(bc.DateTime);
            if (bc.Stock.Count >= 3)
            {
                Console.WriteLine(bc.Stock[0].BinCode);
                Console.WriteLine(bc.Stock[1].BinCode);
                Console.WriteLine(bc.Stock[2].BinCode);
            }
        }

        public static void TestBCSerial()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            PutInventory.BCFromDB("C:/Users/aarop");

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to pull and serialize bin cointents.");
        }

        public static void CheckTableDatePull()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            Console.WriteLine(chariot.LastTableUpdate("bin"));
        }

        public static void CheckStockDate()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            Console.WriteLine(chariot.LastStockUpdate(new List<string> { "PR", "PK" }));
        }

        public static void TestDBRepair()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            chariot.RepairDatabase();
        }

        public static void TestDBValidation()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            chariot.ValidateDatabase();
        }

        public static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        public static void TestLiteralPrinting()
        {
            string input = DataConversion.ClipboardToString();
            for (int i = 0; i<3;++i)
            {
                Console.WriteLine(input);
                input = ToLiteral(input);
            }
            Console.WriteLine(input);
        }

        public static void TestCSVPull()
        {
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            DataTable data = DataConversion.CSVToTable(@"\\ausefpdfs01ns\Shares\Public\IMR\Australia\Pricebook\IMR_PriceBookSalesRanking.csv", Constants.ITEM_COLUMNS.Values.ToList(),"CompanyCode = 'AU'");

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to pull Item Data.");
            stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (DataRow row in data.Rows)
            {
                row["NewUsed"] = (row["NewUsed"].ToString() == "Used");
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to convert to boolean preowned.");
            Console.WriteLine($"{data} || COLS: {data.Columns.Count} || ROWS: {data.Rows.Count}");
            Console.WriteLine(data.Rows[1]["NewUsed"].ToString());
        }

        public static void InvPullTest()
        {
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            DataTable data = GetInventory.BinTable();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to pull Bin data.");
            Console.WriteLine($"{data} || COLS: {data.Columns.Count} || ROWS: {data.Rows.Count}");
        }

        public static void InvPushTest()
        {
            string entry;
            char choice;
            do
            {
                Console.Write($"[I] - Items\n[B] - Bins\n[S] - Stock\n[U] - UoM\n[Q] - Quit\nChoose: ... ");

                entry = Console.ReadLine();
                choice = (entry != "") ? entry.ToLower()[0] : 'q';

                switch (choice)
                {
                    case 'i':
                        PushItems();
                        break;
                    case 'b':
                        PushBins();
                        break;
                    case 's':
                        PushStock();
                        break;
                    case 'u':
                        PushUoM();
                        break;
                }
                    
            } while (choice != 'q');
        }

        public static void PushBins()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            PutInventory.BinsFromClipboard();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to update Bin data.");
        }

        public static void PushItems()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            PutInventory.ItemsFromCSV();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to update Item data.");
        }

        public static void PushStock()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            PutInventory.StockFromClipboard();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to update Stock data.");
        }

        public static void PushUoM()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            PutInventory.UoMFromClipboard();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to update UoM data.");
        }

        public static void PathTesting(string path = @"\\ausefpdfs01ns\Shares\Public\IMR\Australia\Pricebook\IMR_PriceBookSalesRanking.csv")
        {
            Console.WriteLine(Path.GetFullPath(path));
            Console.WriteLine(Path.GetDirectoryName(path));
            Console.WriteLine(Path.GetFileName(path));
            Console.WriteLine(Path.GetExtension(path));
            Console.WriteLine((Directory.Exists(Path.GetDirectoryName(path))) ? "Direcotry Exists" : "Directory Does Not Exist");
            Console.WriteLine((File.Exists(path)) ? "File Exists" : "File Does Not Exist");
        }

        public static void TestingSettings()
        {
            Console.WriteLine(App.Settings.SolLocation);
            App.Settings.SolLocation = @"Cdsa:/Users/aarop";
            Console.WriteLine(App.Settings.SolLocation);
        }

        public static void JSONTesting()
        {
            Thing one = new Thing();
            Thing two = new Thing();
            Thing three = new Thing();

            one.First = "ONE";
            one.Second = 1;
            one.Third = false;

            two.First = "TWO";
            two.Second = 2;
            two.Third = false;

            three.First = "THREE";
            three.Second = 3;
            three.Third = true;

            string jsonStr = JsonSerializer.Serialize(new List<Thing>() { one, two, three }, new JsonSerializerOptions() { WriteIndented = true });

            Console.WriteLine(jsonStr);

            Console.WriteLine("Press any key to continue...");
        }

        public static void TestDataConverter()
        {
            string[,] printOut = DataConversion.ClipboardToArray();
            Console.WriteLine(printOut.ToString());
            Console.WriteLine(printOut.GetLength(0));
            Console.WriteLine(printOut.GetLength(1));
            Console.WriteLine("Press any key to continue...");
            _ = Console.ReadLine();

            Console.WriteLine(DataConversion.ArrayToJSON(printOut));
            Console.WriteLine("Press any key to continue...");
            _ = Console.ReadLine();
        }

        public static void TestPKRegex()
        {
            Regex regex = new Regex(@"^(\w)(\d{3})$");

            string bin = "B13";

            //Console.WriteLine(regex.IsMatch(bin));

            Match omatch = regex.Match(bin);

            Console.WriteLine(omatch.Success);

            MatchCollection matches = regex.Matches(bin);

            Console.WriteLine(matches.Count > 0);

            if (matches.Count > 0)
            {
                string letter = matches[0].Groups[1].Value;
                int number = int.Parse(matches[0].Groups[2].Value);

                Console.WriteLine($"Letter: {letter}\nNumber: {number}");

                foreach (Match match in matches)
                {
                    foreach (Group group in match.Groups)
                    {
                        Console.WriteLine(group.Value);
                    }
                }
            }
        }

        public static void TestBinUpdate()
        {
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            var chariot = new InventoryChariot();

            DataTable bins = DataConversion.ClipboardToTable();

            chariot.BinTableUpdate(bins);

            stopwatch.Stop();

            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms to update Bin data.");
        }

        public static void SimpleJSONParse()
        {
            string data = " [ {\"name\": \"John Doe\", \"occupation\": \"gardener\"}, {\"name\": \"Peter Novak\", \"occupation\": \"driver\"} ]";

            JsonDocument doc = JsonDocument.Parse(data);
            JsonElement root = doc.RootElement;

            // Simple Parse
            Console.WriteLine(root);

            JsonElement u1 = root[0];
            JsonElement u2 = root[1];

            Console.WriteLine(u1);
            Console.WriteLine(u2);

            Console.WriteLine(u1.GetProperty("name"));
            Console.WriteLine(u1.GetProperty("occupation"));

            Console.WriteLine(u2.GetProperty("name"));
            Console.WriteLine(u2.GetProperty("occupation"));
        }

        public static void EnumerateJSONDoc()
        {
            string data = " [ {\"name\": \"John Doe\", \"occupation\": \"gardener\"}, {\"name\": \"Peter Novak\", \"occupation\": \"driver\"} ]";

            JsonDocument doc = JsonDocument.Parse(data);
            JsonElement root = doc.RootElement;

            // Enumerate
            var users = root.EnumerateArray();

            while (users.MoveNext())
            {
                var user = users.Current;
                Console.WriteLine(user);

                var props = user.EnumerateObject();

                while (props.MoveNext())
                {
                    var prop = props.Current;
                    Console.WriteLine($"{prop.Name}: {prop.Value}");
                }
            }
        }

        public static void SerializeJSON()
        {
            // Serialize
            var date = new MyDate(1987, 12, 10);

            var json1 = JsonSerializer.Serialize(date);
            Console.WriteLine(json1);

            var user1 = new User("John Doe", "gardener", date);

            var json2 = JsonSerializer.Serialize(user1);
            Console.WriteLine(json2);

            var thing = new Thing("John Doe", 5, true);

            var json3 = JsonSerializer.Serialize(thing);
            Console.WriteLine(json3);
        }

        public static void DeserializeJSON()
        {
            // Deserialize
            string json = "{\"Name\":\"John Doe\",\"Occupation\":\"gardener\",\"DateOfBirth\":{\"Year\":1995,\"Month\":11,\"Day\":30}}";

            var user = JsonSerializer.Deserialize<User>(json);

            Console.WriteLine(user);

            Console.WriteLine(user.Name);
            Console.WriteLine(user.Occupation);
            Console.WriteLine(user.DateOfBirth);
        }

        public static void CreateJSON()
        {
            // Create JSON Object
            var ms = new MemoryStream();
            var writer = new Utf8JsonWriter(ms);

            writer.WriteStartObject();
            writer.WriteString("name", "John Doe");
            writer.WriteString("occupation", "gardener");
            writer.WriteNumber("age", 34);
            writer.WriteEndObject();
            writer.Flush();

            string json = Encoding.UTF8.GetString(ms.ToArray());

            Console.WriteLine(json);
        }

        public static void WriteJSONToFile()
        {
            // Beautify And write to file JSON
            string data = " [ {\"name\": \"John Doe\", \"occupation\": \"gardener\"}, " +
    "{\"name\": \"Peter Novak\", \"occupation\": \"driver\"} ]";

            JsonDocument jdoc = JsonDocument.Parse(data);

            string fileName = "C:/users/aarop/source/data.json";

            string freddy = JsonSerializer.Serialize(jdoc, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(fileName, freddy);

            FileStream fs = File.OpenWrite(fileName);

            Utf8JsonWriter writer = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true });
            jdoc.WriteTo(writer);
        }

        public static void CheckCliptoTableConversion()
        {
            // Checking CliptoTable conversion.
            DataTable data = DataConversion.ClipboardToTable();

            foreach (DataRow row in data.Rows)
            {
                Console.WriteLine(row[0]);
            }

            Console.WriteLine(data.ToString());
        }

    }

    public class MyDate
    {

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public MyDate() { }

        public MyDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public override string ToString()
        {
            return $"{Year} - {Month} - {Day}";
        }

    }

    public class User
    {
        public string Name { get; set; }
        public string Occupation { get; set; }
        public MyDate DateOfBirth { get; set; }

        public User() { }

        public User(string name, string occupation, MyDate dateOfBirth)
        {
            Name = name;
            Occupation = occupation;
            DateOfBirth = dateOfBirth;
        }

        public override string ToString()
        {
            return $"{{\n\tName: {Name}\n\tOccupation: {Occupation}\n\tDateOfBirth: {DateOfBirth}\n}}";
        }

    }

    public class Thing
    {
        public string First { get; set; }
        public int Second { get; set; }
        public bool Third { get; set; }

        public Thing() { }

        public Thing(string first, int second, bool third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }
}
