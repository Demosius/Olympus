using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Olympus.Helios.Inventory
{
    public class Bin
    {
        public Zone Zone { get; set; }
        public string Code { get; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool Empty { get; set; }
        public bool Assigned { get; set; }
        public int Ranking { get; set; }
        public float UsedCube { get; set; }
        public float MaxCube { get; set; }
        public DateTime LastCCDate { get; set; }
        public DateTime LastPIDate { get; set; }

        public Bay Bay { get; set; }
        public Dictionary<int, Stock> Stock { get; }

        /// <summary>
        /// Default constructor used to make empty temp bins.
        /// </summary>
        public Bin() { }

        /// <summary>
        ///  Primary constructor for creating Bins.
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="code"></param>
        /// <param name="location"></param>
        /// <param name="description"></param>
        /// <param name="empty"></param>
        /// <param name="assigned"></param>
        /// <param name="ranking"></param>
        /// <param name="usedCube"></param>
        /// <param name="maxCube"></param>
        /// <param name="lastCCDate"></param>
        /// <param name="lastPIDate"></param>
        public Bin(Zone zone, string code, string location, string description,
                    bool empty, bool assigned, int ranking, float usedCube,
                    float maxCube, DateTime lastCCDate, DateTime lastPIDate)
        {
            Zone = zone;
            Code = code;
            Location = location;
            Description = description;
            Empty = empty;
            Assigned = assigned;
            Ranking = ranking;
            UsedCube = usedCube;
            MaxCube = maxCube;
            LastCCDate = lastCCDate;
            LastPIDate = lastPIDate;
            Bay = null;

            Stock = new Dictionary<int, Stock> { };
        }

        private string GetPKBay()
        {
            Regex regex = new Regex(@"^(\w)(\d{3})$");
            Match match = regex.Match(Code);

            if (!match.Success) throw new Exception("PK bin has unexpected bin name.");

            string letter = match.Groups[1].Value;
            int number = int.Parse(match.Groups[2].Value);

            int[] bayRanges;

            switch (letter)
            {
                case "B":
                    bayRanges = new int[4] { 84, 168, 252, 272 };
                    break;
                case "C":
                    bayRanges = new int[4] { 20, 40, 60, 80 };
                    break;
                case "E":
                case "F":
                case "G":
                    bayRanges = new int[4] { 40, 80, 120, 160 };
                    break;
                default:
                    bayRanges = new int[4] { 48, 96, 144, 192 };
                    break;
            }
            for (int i = 0; i < bayRanges.Length; ++i)
            {
                if (number <= bayRanges[i])
                {
                    number = i + 1;
                    break;
                }
            }
            return letter + number;
        }

        public string GetBayName()
        {
            if (Bay != null)  return Bay.Name;

            switch (Zone.Code)
            {
                case "PK":
                    return GetPKBay();
                case "SP PK":
                case "PO PK":
                case "SUP PK":
                    return Code.Substring(0, 2);
                case "BLK PK":
                    return Description;
                case "OS": 
                case "PR":
                case "HR":
                    return Code.Split(' ')[0];
                default:
                    return Zone.Code + "-" + Code[0];
            }
        }

        public void AddStock(Stock stock)
        {
            Stock.Add(stock.Item.Number, stock);
            stock.Bin = this;
        }

        public void RemoveStock(Stock stock)
        {
            Stock.Remove(stock.Item.Number);
        }
    }

    public class SimpleBin
    {
        public string Location { get; set; }
        public string ZoneCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Empty { get; set; }
        public bool Assigned { get; set; } 
        public int Ranking { get; set; }
        public double UsedCube { get; set; }
        public double MaxCube { get; set; }
        public DateTime LastCCDate { get; set; }
        public DateTime LastPIDate { get; set; }

        public SimpleBin() { }

        public SimpleBin(string location, string zoneCode, string code, string description, bool empty, bool assigned,
                            int ranking, double usedCube, double maxCube, DateTime lastCCDate, DateTime lastPIDate)
        {
            Location = location;
            ZoneCode = zoneCode;
            Code = code;
            Description = description;
            Empty = empty;
            Assigned = assigned;
            Ranking = ranking;
            UsedCube = usedCube;
            MaxCube = maxCube;
            LastCCDate = lastCCDate;
            LastPIDate = lastPIDate;
        }
    }
}
