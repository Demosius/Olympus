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

namespace Titan
{
    class Program
    {
        static void Main()
        {
            TestSets();

            _ = Console.ReadLine();
        }

        public static void TestSets()
        {
            Console.WriteLine("Press enter to begin: ...");
            Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"\nSTAFF:");
            MasterChariot chariot = new StaffChariot(Toolbox.GetSol());
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
            chariot = new InventoryChariot(Toolbox.GetSol());
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
            chariot = new EquipmentChariot(Toolbox.GetSol());
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
            chariot = new UserChariot(Toolbox.GetSol());
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
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
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
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            Console.WriteLine(chariot.LastTableUpdate("bin"));
        }

        public static void CheckStockDate()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            Console.WriteLine(chariot.LastStockUpdate(new List<string> { "PR", "PK" }));
        }

        public static void TestDBRepair()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            chariot.RepairDatabase();
        }

        public static void TestDBValidation()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
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

            DataTable data = GetInventory.Bins();

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
            Console.WriteLine(Toolbox.GetSol());
            Toolbox.SetSol(@"Cdsa:/Users/aarop");
            Console.WriteLine(Toolbox.GetSol());
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
