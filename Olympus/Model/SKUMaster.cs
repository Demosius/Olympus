using Olympus.Uranus.Inventory;
using Olympus.Uranus.Inventory.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Model
{
    public enum EProductType
    {
        CORE,
        LOOT,
        POP
    }

    public enum EDeptType
    {
        Frontline,
        PreOwned,
        Supplies
    }

    [DataContract]
    public class SKUMaster
    {
        [PrimaryKey, DataMember]
        public int SKU { get; set; }
        [DataMember]
        public int DivisionCode { get; set; }
        [DataMember]
        public string DivisionName { get; set; }
        [DataMember]
        public int CategoryCode { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public int PlatformCode { get; set; }
        [DataMember]
        public string PlatformName { get; set; }
        [DataMember]
        public int GenreCode { get; set; }
        [DataMember]
        public string GenreName { get; set; }
        [DataMember]
        public EProductType ProductTypeCode { get; set; }
        [DataMember]
        public string ProductType { get; set; }
        [DataMember]
        public EDeptType DeptTypeCode { get; set; }
        [DataMember]
        public string DeptType { get; set; }
        [DataMember]
        public string ItemDescription { get; set; }
        [DataMember]
        public bool CasePick { get; set; }
        [DataMember]
        public bool SplitCase { get; set; }
        [DataMember]
        public int? UnitsPerCase { get; set; }
        [DataMember]
        public int? UnitsPerPack { get; set; }
        [DataMember]
        public int? UnitsPerCarton { get; set; }    // Use packs if no cases - null if no packs or cases. 
        [DataMember]
        public int? CartonsPerPallet { get; set; }   // Use packs if no cases - units if no packs or cases.
        [DataMember]
        public double EachLength { get; set; }
        [DataMember]
        public double EachWidth { get; set; }
        [DataMember]
        public double EachHeight { get; set; }
        [DataMember]
        public double EachWeight { get; set; }
        [DataMember]
        public double PackLength { get; set; }
        [DataMember]
        public double PackWidth { get; set; }
        [DataMember]
        public double PackHeight { get; set; }
        [DataMember]
        public double PackWeight { get; set; }
        [DataMember]
        public double CaseLength { get; set; }
        [DataMember]
        public double CaseWidth { get; set; }
        [DataMember]
        public double CaseHeight { get; set; }
        [DataMember]
        public double CaseWeight { get; set; }
        [DataMember]
        public double CartonLength { get; set; }
        [DataMember]
        public double CartonWidth { get; set; }
        [DataMember]
        public double CartonHeight { get; set; }
        [DataMember]
        public double CartonWeight { get; set; }
        [DataMember]
        public int? TotalEachesOnHand { get; set; } 
        [DataMember]
        public int? TotalPacksOnHand { get; set; } 
        [DataMember]
        public int? TotalCasesOnHand { get; set; } 
        [DataMember]
        public int? TotalCartonsOnHand { get; set; } // Equal to highest UoM
        [DataMember]
        public double TotalWeight { get; set; }
        [DataMember]
        public int BaseUnitsOnHand { get; set; }
        [DataMember]
        public string CurrentPickZones { get; set; }
        [DataMember]
        public string CurrentPickBins { get; set; }
        [DataMember]
        public string CurrentOverstockBins { get; set; }
        [DataMember]
        public string CurrentVirtualBins { get; set; }
        
        /// <summary>
        /// Constructor assuming fully filled item.
        /// </summary>
        public SKUMaster(NAVItem item)
        {
            item.SetUoMs();

            SKU = item.Number;
            DivisionCode = item.DivisionCode;
            DivisionName = item.Division.Description;
            CategoryCode = item.CategoryCode;
            CategoryName = item.Category.Description;
            PlatformCode = item.PlatformCode;
            PlatformName = item.Platform.Description;
            GenreCode = item.GenreCode;
            GenreName = item.Genre.Description;
            if (PlatformCode == 516)
                ProductTypeCode = EProductType.POP;
            else if (DivisionCode == 550)
                ProductTypeCode = EProductType.LOOT;
            else
                ProductTypeCode = EProductType.CORE;
            ProductType = ProductTypeCode.ToString();
            ItemDescription = item.Description;
            if (PlatformCode == 125)
                DeptTypeCode = EDeptType.Supplies;
            else if (item.PreOwned)
                DeptTypeCode = EDeptType.PreOwned;
            else
                DeptTypeCode = EDeptType.Frontline;
            DeptType = DeptTypeCode.ToString();

            // Units per Case/Pack/Carton(largest thereof)
            if (item.Case is null)
                UnitsPerCase = null;
            else
                UnitsPerCase = item.Case.QtyPerUoM;
            if (item.Pack is null || item.Pack.QtyPerUoM == 0)
                UnitsPerPack = null;
            else
                UnitsPerPack = item.Pack.QtyPerUoM;
            if (UnitsPerCase is null)
                UnitsPerCarton = UnitsPerPack;
            else
                UnitsPerCarton = UnitsPerCase;

            //// Basic dimensions.
            //Length = item.Length;
            //Width = item.Width;
            //Height = item.Height;
            //Weight = item.Weight;

            // Total Cartons/Units on hand, check for current pick bin,
            // gather list of pallet sizes, and verify if item picks in cases and/or split cases.
            List<string> osZones = new List<string> { "OS", "PR", "HR" };
            List<string> cpZones = new List<string> { "CP", "TM" };
            List<string> primaryZones = new List<string>
            {
                "BLK PK",
                "BLK PO",
                "PO PK",
                "PK",
                "SP PK",
                "SUP PK"
            };
            List<string> zones = new List<string>();
            List<string> bins = new List<string>();
            List<int> palletSizes = new List<int>();
            if (UnitsPerCarton is null)
                TotalCartonsOnHand = null;
            else
                TotalCartonsOnHand = 0;
            EUoM uomCheck;
            if (UnitsPerCase != null)
                uomCheck = EUoM.CASE;
            else if (UnitsPerPack != null)
                uomCheck = EUoM.PACK;
            else
                uomCheck = EUoM.EACH;
            foreach (NAVStock stock in item.NAVStock)
            {
                BaseUnitsOnHand += stock.GetBaseQty();
                if (stock.UoM.UoM == uomCheck)
                    TotalCartonsOnHand += stock.Qty;
                if (primaryZones.Contains(stock.ZoneCode))
                {
                    zones.Add(stock.ZoneCode);
                    bins.Add(stock.BinCode);
                    if (stock.UoM.UoM < uomCheck)
                        SplitCase = true;
                }
                else if (osZones.Contains(stock.ZoneCode) && stock.UoM.UoM == uomCheck)
                {
                    palletSizes.Add(stock.Qty);
                }
                else if (cpZones.Contains(stock.ZoneCode))
                {
                    if (stock.UoM.UoM == uomCheck)
                        CasePick = true;
                    else
                        SplitCase = true;
                }
            }
            //CurrentPickZone = string.Join(", ", zones);
            //CurrentPickBin = string.Join(", ", bins);
            CartonsPerPallet = palletSizes
                                .GroupBy(x => x)
                                .OrderByDescending(x => x.Count()).ThenBy(x => x.Key)
                                .Select(x => (int?)x.Key)
                                .FirstOrDefault();
        }

        /// <summary>
        /// Constructor taking basic objects with multiple lists/dicts to cross-reference.
        /// </summary>
        public SKUMaster(NAVItem item,
                        Dictionary<int, List<NAVStock>> stock,
                        Dictionary<int, Dictionary<string, NAVUoM>> uoms,
                        Dictionary<string, NAVBin> bins,
                        Dictionary<int, string> divisions, Dictionary<int, string> categories,
                        Dictionary<int, string> platforms, Dictionary<int, string> genres)
        {
            SKU = item.Number;
            DivisionCode = item.DivisionCode;
            divisions.TryGetValue(DivisionCode, out string divName);
            DivisionName = divName;
            CategoryCode = item.CategoryCode;
            categories.TryGetValue(CategoryCode, out string catName);
            CategoryName = catName;
            PlatformCode = item.PlatformCode;
            platforms.TryGetValue(PlatformCode, out string pfName);
            PlatformName = pfName;
            GenreCode = item.GenreCode;
            genres.TryGetValue(GenreCode, out string genName);
            GenreName = genName;
            if (PlatformCode == 516)
                ProductTypeCode = EProductType.POP;
            else if (DivisionCode == 550)
                ProductTypeCode = EProductType.LOOT;
            else
                ProductTypeCode = EProductType.CORE;
            ProductType = ProductTypeCode.ToString();
            ItemDescription = item.Description;
            if (PlatformCode == 125)
                DeptTypeCode = EDeptType.Supplies;
            else if (item.PreOwned)
                DeptTypeCode = EDeptType.PreOwned;
            else
                DeptTypeCode = EDeptType.Frontline;
            DeptType = DeptTypeCode.ToString();

            // Units and Dims per Case/Pack/Each/Carton(largest thereof)
            uoms.TryGetValue(SKU, out Dictionary<string, NAVUoM> itemUoMs);
            NAVUoM caseUoM = null, packUoM = null, eachUoM = null;
            itemUoMs?.TryGetValue("CASE", out caseUoM);
            itemUoMs?.TryGetValue("PACK", out packUoM);
            itemUoMs?.TryGetValue("EACH", out eachUoM);
            NAVUoM ctnUoM = caseUoM ?? packUoM ?? eachUoM;

            if (caseUoM is null)
            {
                UnitsPerCase = null;
                TotalCasesOnHand = null;
            }
            else
            {
                UnitsPerCase = caseUoM.QtyPerUoM;
                TotalCasesOnHand = 0;
                CaseLength = caseUoM.Length;
                CaseHeight = caseUoM.Height;
                CaseWidth = caseUoM.Weight;
                CaseWeight = caseUoM.Weight;
            }

            if (packUoM is null)
            {
                UnitsPerPack = null;
                TotalPacksOnHand = null;
            }
            else
            {
                UnitsPerPack = packUoM.QtyPerUoM;
                TotalPacksOnHand = 0;
                PackLength = packUoM.Length;
                PackHeight = packUoM.Height;
                PackWidth = packUoM.Width;
                PackWeight = packUoM.Weight;
            }

            if (ctnUoM is null)
            {
                UnitsPerCarton = null;
                TotalCartonsOnHand = null;
            }
            else
            {
                UnitsPerCarton = ctnUoM.QtyPerUoM;
                TotalCartonsOnHand = 0;
                CartonLength = ctnUoM.Length;
                CartonHeight = ctnUoM.Height;
                CartonWidth = ctnUoM.Width;
                CartonWeight = ctnUoM.Weight;
            }

            if (eachUoM != null)
            {
                TotalEachesOnHand = 0;
                EachLength = eachUoM.Length;
                EachHeight = eachUoM.Height;
                EachWidth = eachUoM.Width;
                EachWeight = eachUoM.Weight;
            }

            // Total Cartons/Units on hand, check for current pick bin,
            // gather list of pallet sizes, and verify if item picks in cases and/or split cases.
            List<string> overstockZones = new List<string> { "OS", "PR", "HR", "OZ" };
            List<string> virtualZones = new List<string> { "CP", "DCP", "SUP CP", "TM", "HZ", "PARK", "WEB TP" };
            List<string> vPickZones = new List<string> { "CP", "DCP", "SUP CP", "TM", "WEB TP" };
            List<string> casePickZones = new List<string> { "CP", "DCP"};
            List<string> primaryZones = new List<string>
            {
                "BLK PK",
                "BLK SS",
                "PO PK",
                "PK",
                "SP PK",
                "SUP PK"
            };
            List<string> zones = new List<string>();
            List<string> pickBins = new List<string>();
            List<string> osBins = new List<string>();
            List<string> vBins = new List<string>();
            List<int> palletSizes = new List<int>();
            
            EUoM uomCheck;
            if (UnitsPerCase != null)
                uomCheck = EUoM.CASE;
            else if (UnitsPerPack != null)
                uomCheck = EUoM.PACK;
            else
                uomCheck = EUoM.EACH;

            stock.TryGetValue(SKU, out List<NAVStock> skuStock);
            if (skuStock is null) skuStock = new List<NAVStock>();
            foreach (NAVStock s in skuStock)
            {
                string zone = s.ZoneCode;
                EUoM uom = s.GetEUoM();
                switch (uom)
                {
                    case EUoM.CASE:
                        s.UoM = caseUoM;
                        TotalCasesOnHand += s.Qty;
                        break;
                    case EUoM.PACK:
                        s.UoM = packUoM;
                        TotalPacksOnHand += s.Qty;
                        break;
                    case EUoM.EACH:
                        s.UoM = eachUoM;
                        TotalEachesOnHand += s.Qty;
                        break;
                }
                BaseUnitsOnHand += s.GetBaseQty();
                TotalWeight += s.GetWeight();
                if (uom == uomCheck)
                    TotalCartonsOnHand += s.Qty;
                if (primaryZones.Contains(zone))
                {
                    zones.Add(s.ZoneCode);
                    pickBins.Add(s.BinCode);
                    if (uom < uomCheck)
                        SplitCase = true;
                }
                else if (overstockZones.Contains(zone))
                {
                    osBins.Add(s.BinCode);
                    if (zone != "OS")
                    {
                        int qty = s.Qty;
                        bins.TryGetValue(s.BinID, out NAVBin bin);
                        if (bin != null && bin.Description.Contains("Double"))
                            palletSizes.Add(qty / 2);
                        palletSizes.Add(qty);
                    }

                }
                else if (virtualZones.Contains(zone))
                {
                    vBins.Add(s.BinCode);
                    if (casePickZones.Contains(zone))
                        CasePick = true;
                    if (vPickZones.Contains(zone) && uom < uomCheck)
                        SplitCase = true;
                }
            }
            CurrentPickZones = string.Join("|", zones.Distinct().ToList());
            CurrentPickBins = string.Join("|", pickBins.Distinct().ToList());
            CurrentOverstockBins = string.Join("|", osBins.Distinct().ToList());
            CurrentVirtualBins = string.Join("|", vBins.Distinct().ToList());
            CartonsPerPallet = palletSizes
                                .GroupBy(x => x)
                                .OrderByDescending(x => x.Count()).ThenBy(x => x.Key)
                                .Select(x => (int?)x.Key)
                                .FirstOrDefault();
        }

        //private string ProductTypeToString(EProductType productType)
        //{
        //    switch (productType)
        //    {
        //        case EProductType.CORE:
        //            return "CORE";
        //        case EProductType.LOOT:
        //            return "LOOT";
        //        case EProductType.POP:
        //            return "POP";
        //        default:
        //            return "CORE";
        //    }
        //}

        //private string DeptTypeToString(EDeptType deptType)
        //{
        //    switch (deptType)
        //    {
        //        case EDeptType.Frontline:
        //            return "Frontline";
        //        case EDeptType.PreOwned:
        //            return "PreOwned";
        //        case EDeptType.Supplies:
        //            return "Supplies";
        //        default:
        //            return "CORE";
        //    }
        //}
    }
}
