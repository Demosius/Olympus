using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable StringLiteralTypo

namespace Uranus.Inventory;

/// <summary>
///  Used for holding constants relevant to the Inventory namespace.
///  Common use is for holding Column names for data.
/// </summary>
public static class Constants
{
    #region const string

    public const string Action = "Action Type";
    public const string ActionNotes = "Action Notes";
    public const string AvailFillQty = "Avail. UOM Fulfilment Qty";
    public const string BaseUnits = "Total Units (Base)";
    public const string BatchDescription = "Batch Description";
    public const string BatchNo = "Batch No.";
    public const string BinAssigned = "Bin Assigned";
    public const string BinCode = "Bin Code";
    public const string BinRanking = "Bin Ranking";
    public const string CartonID = "Carton ID";
    public const string Cartonized = "Cartonized";
    public const string Cartons = "No. of Cartons";
    public const string CatCode = "CategoryCode";
    public const string CCN = "CCN";
    public const string CCNRegion = "CCN Region";
    public const string Cluster = "Cluster Ref";
    public const string Code = "Code";
    public const string Container = "Container";
    public const string CreatedBy = "Created By";
    public const string CreatedDate = "Created On Date";
    public const string CreatedOn = "Created On";
    public const string CreatedTime = "Created On Time";
    public const string Ctns = "CTNS";
    public const string CtnType = "Carton Type";
    public const string Cube = "Cubage";
    public const string CubeCM = "CM Cubage";
    public const string CubeM = "Cube m3";
    public const string Date = "Date";
    public const string DateCreated = "Date Created";
    public const string Description = "Description";
    public const string DeviceID = "Device ID";
    public const string DivCode = "DivisionCode";
    public const string DocNo = "Document No.";
    public const string DueDate = "Due Date";
    public const string Empty = "Empty";
    public const string EndBin = "Ending Pick Bin";
    public const string EndZone = "Ending Pick Zone";
    public const string ExcludeCtn = "Exclude Cartonization";
    public const string Fixed = "Fixed";
    public const string FullyShipped = "Fully Shipped";
    public const string GenCode = "GenreCode";
    public const string Height = "Height";
    public const string HeightCM = "Height (CM)";
    public const string InnerPack = "Inner Pack";
    public const string ItemBarcode = "ItemBarcode";
    public const string ItemCode = "ItemCode";
    public const string ItemDesc = "Item Description";
    public const string ItemDivCode = "Item Division Code";
    public const string ItemName = "ItemName";
    public const string ItemNumber = "Item No.";
    public const string LastCycleCount = "CC Last Count Date";
    public const string LastPhysicalCount = "PI - Last Count Date";
    public const string LastTimeCartonizedDate = "Last Time Cartonized Date";
    public const string LastTimeCartonizedTime = "Last Time Cartonized time";
    public const string Length = "Length";
    public const string LengthCM = "Length (CM)";
    public const string LineNo = "Line No.";
    public const string LocationCode = "Location Code";
    public const string MaxCube = "Maximum Cubage";
    public const string MaxQty = "Max Qty";
    public const string MBRegion = "MB Region";
    public const string Name = "Name";
    public const string NegQty = "Neg. Adjmt. Qty.";
    public const string NewUsed = "NewUsed";
    public const string Number = "No.";
    public const string OperatorID = "Operator ID";
    public const string OperatorName = "Operator Name";
    public const string OriginalQty = "Original Qty.";
    public const string PFCode = "PlatformCode";
    public const string PickerID = "Picker ID";
    public const string PickQty = "Pick Qty.";
    public const string PosQty = "Pos. Adjmt. Qty.";
    public const string PostDate = "Posted Date";
    public const string PrimaryBarcode = "PrimaryBarcode";
    public const string PTLFileCreated = "PTL File Created";
    public const string PutAwayQty = "Put-away Qty.";
    public const string Qty = "Qty";
    public const string QtyBase = "Qty. (Base)";
    public const string QtyHandled = "Qty. Handled";
    public const string QtyOutstanding = "Qty. Outstanding";
    public const string QtyPerUoM = "Qty. per Unit of Measure";
    public const string QtyToHandle = "Qty. to Handle";
    public const string Quantity = "Quantity";
    public const string ReceiveDate = "Actual Received Date";
    public const string ReceiveQty = "Received Qty.";
    public const string Region = "Region";
    public const string RoadCCN = "Prm > Road CCN";
    public const string RoadRegion = "Road Regions";
    public const string ShipDate = "Actual Shipment Date";
    public const string ShipmentCreated = "Shipment Created";
    public const string ShippingDays = "Shipping Days";
    public const string SkuDesc = "SKU Description";
    public const string SkuID = "SKU ID";
    public const string SortingLane = "Sorting Lane Regions";
    public const string SourceLineNo = "Source Line No.";
    public const string SourceNo = "Source No.";
    public const string StartBin = "Starting Pick Bin";
    public const string StartZone = "Starting Pick Zone";
    public const string State = "State";
    public const string Store = "Store";
    public const string StoreNo = "Store No.";
    public const string StoreType = "Store Type";
    public const string Tech = "Tech Type";
    public const string TimeCreated = "Time Created";
    public const string Timestamp = "Timestamp";
    public const string TOBatchNo = "TO Batch No.";
    public const string TransferCode = "Transfer-to Code";
    public const string Units = "Total Qty. (Base)";
    public const string UoM = "Unit of Measure";
    public const string UoMCode = "Unit of Measure Code";
    public const string UsedCube = "Used Cubage";
    public const string VarianceQty = "Variance Qty.";
    public const string Wave = "Wave";
    public const string WaveID = "Wave ID";
    public const string WaveNumber = "Wave Number";
    public const string Weight = "Weight";
    public const string Weight_Kg = "Weight kg";
    public const string WeightKG = "Weight (Kg)";
    public const string Width = "Width";
    public const string WidthCM = "Width (CM)";
    public const string WorkAssignment = "Work Assignment";
    public const string ZoneCode = "Zone Code";
    public const string ZoneID = "Zone ID";
    public const string ZoneRank = "Zone Ranking";

    #endregion

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

    public static Dictionary<string, int> DematicMispickColumns = new()
    {
        {"Actual Shipment Date", -1},
        {"Actual Received Date", -1},
        {"Carton ID", -1},
        {"Item No.", -1},
        {"Item Description", -1},
        {"Action Notes", -1},
        {"Original Qty.", -1},
        {"Received Qty.", -1},
        {"Variance Qty.", -1},
        {"Posted Date", -1},
    };

}

public interface IColumnIndexer
{
    public void SetIndices(string[] headers);
    public void CheckMissingHeaders();
    public int Max();
}

public class NAVStockIndices : IColumnIndexer
{
    public int LocationCode { get; set; }
    public int ZoneCode { get; set; }
    public int BinCode { get; set; }
    public int ItemNumber { get; set; }
    public int ItemBarcode { get; set; }
    public int UoM { get; set; }
    public int Qty { get; set; }
    public int PickQty { get; set; }
    public int PutAwayQty { get; set; }
    public int NegQty { get; set; }
    public int PosQty { get; set; }
    public int DateCreated { get; set; }
    public int TimeCreated { get; set; }
    public int Fixed { get; set; }

    public NAVStockIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        LocationCode = Array.IndexOf(headers, Constants.LocationCode);
        BinCode = Array.IndexOf(headers, Constants.BinCode);
        ItemNumber = Array.IndexOf(headers, Constants.ItemNumber);
        ItemBarcode = Array.IndexOf(headers, Constants.ItemBarcode);
        UoM = Array.IndexOf(headers, Constants.UoMCode);
        Qty = Array.IndexOf(headers, Constants.Quantity);
        PickQty = Array.IndexOf(headers, Constants.PickQty);
        PutAwayQty = Array.IndexOf(headers, Constants.PutAwayQty);
        NegQty = Array.IndexOf(headers, Constants.NegQty);
        PosQty = Array.IndexOf(headers, Constants.PosQty);
        DateCreated = Array.IndexOf(headers, Constants.DateCreated);
        TimeCreated = Array.IndexOf(headers, Constants.TimeCreated);
        Fixed = Array.IndexOf(headers, Constants.Fixed);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (LocationCode == -1) missingHeaders.Add(Constants.LocationCode);
        if (BinCode == -1) missingHeaders.Add(Constants.BinCode);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (ItemBarcode == -1) missingHeaders.Add(Constants.ItemBarcode);
        if (UoM == -1) missingHeaders.Add(Constants.UoMCode);
        if (Qty == -1) missingHeaders.Add(Constants.Quantity);
        if (PickQty == -1) missingHeaders.Add(Constants.PickQty);
        if (PutAwayQty == -1) missingHeaders.Add(Constants.PutAwayQty);
        if (NegQty == -1) missingHeaders.Add(Constants.NegQty);
        if (PosQty == -1) missingHeaders.Add(Constants.PosQty);
        if (DateCreated == -1) missingHeaders.Add(Constants.DateCreated);
        if (TimeCreated == -1) missingHeaders.Add(Constants.TimeCreated);
        if (Fixed == -1) missingHeaders.Add(Constants.Fixed);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        LocationCode, BinCode, ItemNumber, ItemBarcode, UoM, Qty, PickQty, PutAwayQty, NegQty, PosQty, DateCreated,
        TimeCreated, Fixed,
    }.Max();
}

public class NAVDivPFGenIndices : IColumnIndexer
{
    public int Code { get; set; }
    public int Description { get; set; }

    public NAVDivPFGenIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Code = Array.IndexOf(headers, Constants.Code);
        Description = Array.IndexOf(headers, Constants.Description);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Code == -1) missingHeaders.Add(Constants.Code);
        if (Description == -1) missingHeaders.Add(Constants.Description);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int> { Code, Description }.Max();
}

public class NAVCategoryIndices : IColumnIndexer
{
    public int Code { get; set; }
    public int Description { get; set; }
    public int ItemDivCode { get; set; }

    public void SetIndices(string[] headers)
    {
        Code = Array.IndexOf(headers, Constants.Code);
        Description = Array.IndexOf(headers, Constants.Description);
        ItemDivCode = Array.IndexOf(headers, Constants.ItemDivCode);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Code == -1) missingHeaders.Add(Constants.Code);
        if (Description == -1) missingHeaders.Add(Constants.Description);
        if (ItemDivCode == -1) missingHeaders.Add(Constants.ItemDivCode);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int> { Code, Description, ItemDivCode }.Max();
}

public class NAVLocationIndices : IColumnIndexer
{
    public int Code { get; set; }
    public int Name { get; set; }

    public NAVLocationIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Code = Array.IndexOf(headers, Constants.Code);
        Name = Array.IndexOf(headers, Constants.Name);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Code == -1) missingHeaders.Add(Constants.Code);
        if (Name == -1) missingHeaders.Add(Constants.Name);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int> { Code, Name }.Max();
}

public class NAVZoneIndices : IColumnIndexer
{
    public int Location { get; set; }
    public int Code { get; set; }
    public int Description { get; set; }
    public int Rank { get; set; }

    public NAVZoneIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Location = Array.IndexOf(headers, Constants.LocationCode);
        Code = Array.IndexOf(headers, Constants.Code);
        Description = Array.IndexOf(headers, Constants.Description);
        Rank = Array.IndexOf(headers, Constants.ZoneRank);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Location == -1) missingHeaders.Add(Constants.LocationCode);
        if (Code == -1) missingHeaders.Add(Constants.Code);
        if (Description == -1) missingHeaders.Add(Constants.Description);
        if (Rank == -1) missingHeaders.Add(Constants.ZoneRank);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }
    public int Max() => new List<int> { Location, Code, Description, Rank }.Max();
}

public class NAVItemIndices : IColumnIndexer
{
    public int ItemCode { get; set; }
    public int ItemName { get; set; }
    public int Barcode { get; set; }
    public int NewUsed { get; set; }
    public int CatCode { get; set; }
    public int PFCode { get; set; }
    public int DivCode { get; set; }
    public int GenCode { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Cube { get; set; }
    public int Weight { get; set; }

    public NAVItemIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        ItemCode = Array.IndexOf(headers, Constants.ItemCode);
        ItemName = Array.IndexOf(headers, Constants.ItemName);
        Barcode = Array.IndexOf(headers, Constants.PrimaryBarcode);
        NewUsed = Array.IndexOf(headers, Constants.NewUsed);
        CatCode = Array.IndexOf(headers, Constants.CatCode);
        PFCode = Array.IndexOf(headers, Constants.PFCode);
        DivCode = Array.IndexOf(headers, Constants.DivCode);
        GenCode = Array.IndexOf(headers, Constants.GenCode);
        Length = Array.IndexOf(headers, Constants.Length);
        Width = Array.IndexOf(headers, Constants.Width);
        Height = Array.IndexOf(headers, Constants.Height);
        Cube = Array.IndexOf(headers, Constants.Cube);
        Weight = Array.IndexOf(headers, Constants.Weight);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (ItemCode == -1) missingHeaders.Add(Constants.ItemCode);
        if (ItemName == -1) missingHeaders.Add(Constants.ItemName);
        if (Barcode == -1) missingHeaders.Add(Constants.PrimaryBarcode);
        if (NewUsed == -1) missingHeaders.Add(Constants.NewUsed);
        if (CatCode == -1) missingHeaders.Add(Constants.CatCode);
        if (PFCode == -1) missingHeaders.Add(Constants.PFCode);
        if (DivCode == -1) missingHeaders.Add(Constants.DivCode);
        if (Length == -1) missingHeaders.Add(Constants.Length);
        if (GenCode == -1) missingHeaders.Add(Constants.GenCode);
        if (Width == -1) missingHeaders.Add(Constants.Width);
        if (Height == -1) missingHeaders.Add(Constants.Height);
        if (Cube == -1) missingHeaders.Add(Constants.Cube);
        if (Weight == -1) missingHeaders.Add(Constants.Weight);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int>
        {
            ItemCode, ItemName, Barcode, NewUsed, CatCode, PFCode, DivCode, Length, GenCode, Width, Height, Cube, Weight
        }
        .Max();
}

public class NAV_UoMIndices : IColumnIndexer
{
    public int Code { get; set; }
    public int ItemNumber { get; set; }
    public int QtyPerUoM { get; set; }
    public int MaxQty { get; set; }
    public int InnerPack { get; set; }
    public int ExcludeCarton { get; set; }
    public int LengthCM { get; set; }
    public int WidthCM { get; set; }
    public int HeightCM { get; set; }
    public int CubeCM { get; set; }
    public int WeightKG { get; set; }

    public NAV_UoMIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Code = Array.IndexOf(headers, Constants.Code);
        ItemNumber = Array.IndexOf(headers, Constants.ItemNumber);
        QtyPerUoM = Array.IndexOf(headers, Constants.QtyPerUoM);
        MaxQty = Array.IndexOf(headers, Constants.MaxQty);
        InnerPack = Array.IndexOf(headers, Constants.InnerPack);
        ExcludeCarton = Array.IndexOf(headers, Constants.ExcludeCtn);
        LengthCM = Array.IndexOf(headers, Constants.LengthCM);
        WidthCM = Array.IndexOf(headers, Constants.WidthCM);
        HeightCM = Array.IndexOf(headers, Constants.HeightCM);
        CubeCM = Array.IndexOf(headers, Constants.CubeCM);
        WeightKG = Array.IndexOf(headers, Constants.WeightKG);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Code == -1) missingHeaders.Add(Constants.Code);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (QtyPerUoM == -1) missingHeaders.Add(Constants.QtyPerUoM);
        if (MaxQty == -1) missingHeaders.Add(Constants.MaxQty);
        if (InnerPack == -1) missingHeaders.Add(Constants.InnerPack);
        if (ExcludeCarton == -1) missingHeaders.Add(Constants.ExcludeCtn);
        if (LengthCM == -1) missingHeaders.Add(Constants.LengthCM);
        if (WidthCM == -1) missingHeaders.Add(Constants.WidthCM);
        if (HeightCM == -1) missingHeaders.Add(Constants.HeightCM);
        if (CubeCM == -1) missingHeaders.Add(Constants.CubeCM);
        if (WeightKG == -1) missingHeaders.Add(Constants.WeightKG);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int>
        {
            Code, ItemNumber, QtyPerUoM, MaxQty, InnerPack, ExcludeCarton, LengthCM, WidthCM, HeightCM, CubeCM, WeightKG
        }
        .Max();
}

public class NAVBinIndices : IColumnIndexer
{
    public int LocationCode { get; set; }
    public int ZoneCode { get; set; }
    public int Code { get; set; }
    public int Description { get; set; }
    public int Empty { get; set; }
    public int BinAssigned { get; set; }
    public int BinRank { get; set; }
    public int UsedCube { get; set; }
    public int MaxCube { get; set; }
    public int LastCycleCount { get; set; }
    public int LastPhysicalCount { get; set; }

    public NAVBinIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        LocationCode = Array.IndexOf(headers, Constants.LocationCode);
        ZoneCode = Array.IndexOf(headers, Constants.ZoneCode);
        Code = Array.IndexOf(headers, Constants.Code);
        Description = Array.IndexOf(headers, Constants.Description);
        Empty = Array.IndexOf(headers, Constants.Empty);
        BinAssigned = Array.IndexOf(headers, Constants.BinAssigned);
        BinRank = Array.IndexOf(headers, Constants.BinRanking);
        UsedCube = Array.IndexOf(headers, Constants.UsedCube);
        MaxCube = Array.IndexOf(headers, Constants.MaxCube);
        LastCycleCount = Array.IndexOf(headers, Constants.LastCycleCount);
        LastPhysicalCount = Array.IndexOf(headers, Constants.LastPhysicalCount);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (LocationCode == -1) missingHeaders.Add(Constants.LocationCode);
        if (ZoneCode == -1) missingHeaders.Add(Constants.ZoneCode);
        if (Code == -1) missingHeaders.Add(Constants.Code);
        if (Description == -1) missingHeaders.Add(Constants.Description);
        if (Empty == -1) missingHeaders.Add(Constants.Empty);
        if (BinAssigned == -1) missingHeaders.Add(Constants.BinAssigned);
        if (BinRank == -1) missingHeaders.Add(Constants.BinRanking);
        if (UsedCube == -1) missingHeaders.Add(Constants.UsedCube);
        if (MaxCube == -1) missingHeaders.Add(Constants.MaxCube);
        if (LastCycleCount == -1) missingHeaders.Add(Constants.LastCycleCount);
        if (LastPhysicalCount == -1) missingHeaders.Add(Constants.LastPhysicalCount);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        LocationCode, ZoneCode, Code, Description, Empty, BinAssigned, BinRank, UsedCube, MaxCube, LastCycleCount,
        LastPhysicalCount
    }.Max();
}

public class NAV_TOLineBatchIndices : IColumnIndexer
{
    public int DocNo { get; set; }
    public int TransferCode { get; set; }
    public int ItemNumber { get; set; }
    public int Qty { get; set; }
    public int UoM { get; set; }
    public int AvailFillQty { get; set; }
    public int DateCreated { get; set; }
    public int TimeCreated { get; set; }

    public NAV_TOLineBatchIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        DocNo = Array.IndexOf(headers, Constants.DocNo);
        TransferCode = Array.IndexOf(headers, Constants.TransferCode);
        ItemNumber = Array.IndexOf(headers, Constants.ItemNumber);
        Qty = Array.IndexOf(headers, Constants.Quantity);
        UoM = Array.IndexOf(headers, Constants.UoM);
        AvailFillQty = Array.IndexOf(headers, Constants.AvailFillQty);
        DateCreated = Array.IndexOf(headers, Constants.DateCreated);
        TimeCreated = Array.IndexOf(headers, Constants.TimeCreated);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (DocNo == -1) missingHeaders.Add(Constants.DocNo);
        if (TransferCode == -1) missingHeaders.Add(Constants.TransferCode);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (Qty == -1) missingHeaders.Add(Constants.Quantity);
        if (UoM == -1) missingHeaders.Add(Constants.UoM);
        if (AvailFillQty == -1) missingHeaders.Add(Constants.AvailFillQty);
        if (DateCreated == -1) missingHeaders.Add(Constants.DateCreated);
        if (TimeCreated == -1) missingHeaders.Add(Constants.TimeCreated);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int>
        {DocNo, TransferCode, ItemNumber, Qty, UoM, AvailFillQty, DateCreated, TimeCreated}.Max();
}

public class NAVMoveIndices : IColumnIndexer
{
    public int Action { get; set; }
    public int ItemNumber { get; set; }
    public int ZoneCode { get; set; }
    public int BinCode { get; set; }
    public int Qty { get; set; }
    public int UoMCode { get; set; }

    public NAVMoveIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Action = Array.IndexOf(headers, Constants.Action);
        ItemNumber = Array.IndexOf(headers, Constants.ItemNumber);
        ZoneCode = Array.IndexOf(headers, Constants.ZoneCode);
        BinCode = Array.IndexOf(headers, Constants.BinCode);
        Qty = Array.IndexOf(headers, Constants.Quantity);
        UoMCode = Array.IndexOf(headers, Constants.UoMCode);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Action == -1) missingHeaders.Add(Constants.Action);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (ZoneCode == -1) missingHeaders.Add(Constants.ZoneCode);
        if (BinCode == -1) missingHeaders.Add(Constants.BinCode);
        if (Qty == -1) missingHeaders.Add(Constants.Quantity);
        if (UoMCode == -1) missingHeaders.Add(Constants.UoMCode);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int> { Action, ItemNumber, ZoneCode, BinCode, Qty, UoMCode }.Max();
}

public class PickEventIndices : IColumnIndexer
{
    public int Timestamp { get; set; }
    public int OperatorID { get; set; }
    public int OperatorName { get; set; }
    public int Qty { get; set; }
    public int Container { get; set; }
    public int Tech { get; set; }
    public int ZoneID { get; set; }
    public int WaveID { get; set; }
    public int WorkAssignment { get; set; }
    public int Store { get; set; }
    public int DeviceID { get; set; }
    public int SkuID { get; set; }
    public int SkuDescription { get; set; }
    public int Cluster { get; set; }

    public PickEventIndices(string[] headers, bool softCheck = false)
    {
        SetIndices(headers);
        if (softCheck)
            CheckMissingHeadersSoft();
        else
            CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Timestamp = Array.IndexOf(headers, Constants.Timestamp);
        OperatorID = Array.IndexOf(headers, Constants.OperatorID);
        OperatorName = Array.IndexOf(headers, Constants.OperatorName);
        if (OperatorName == -1) OperatorName = Array.IndexOf(headers, "Picker");
        Qty = Array.IndexOf(headers, Constants.Qty);
        Container = Array.IndexOf(headers, Constants.Container);
        if (Container == -1) Container = Array.IndexOf(headers, "CCN");
        Tech = Array.IndexOf(headers, Constants.Tech);
        if (Tech == -1) Tech = Array.IndexOf(headers, "Tech");
        ZoneID = Array.IndexOf(headers, Constants.ZoneID);
        if (ZoneID == -1) ZoneID = Array.IndexOf(headers, "Zone");
        WaveID = Array.IndexOf(headers, Constants.WaveID);
        WorkAssignment = Array.IndexOf(headers, Constants.WorkAssignment);
        Store = Array.IndexOf(headers, Constants.Store);
        DeviceID = Array.IndexOf(headers, Constants.DeviceID);
        SkuID = Array.IndexOf(headers, Constants.SkuID);
        if (SkuID == -1) SkuID = Array.IndexOf(headers, "SKU");
        SkuDescription = Array.IndexOf(headers, Constants.SkuDesc);
        Cluster = Array.IndexOf(headers, Constants.Cluster);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Timestamp == -1) missingHeaders.Add(Constants.Timestamp);
        if (OperatorID == -1) missingHeaders.Add(Constants.OperatorID);
        if (OperatorName == -1) missingHeaders.Add(Constants.OperatorName);
        if (Qty == -1) missingHeaders.Add(Constants.Qty);
        if (Container == -1) missingHeaders.Add(Constants.Container);
        if (Tech == -1) missingHeaders.Add(Constants.Tech);
        if (ZoneID == -1) missingHeaders.Add(Constants.ZoneID);
        if (WaveID == -1) missingHeaders.Add(Constants.WaveID);
        if (WorkAssignment == -1) missingHeaders.Add(Constants.WorkAssignment);
        if (Store == -1) missingHeaders.Add(Constants.Store);
        if (DeviceID == -1) missingHeaders.Add(Constants.DeviceID);
        if (SkuID == -1) missingHeaders.Add(Constants.SkuID);
        if (SkuDescription == -1) missingHeaders.Add(Constants.SkuDesc);
        if (Cluster == -1) missingHeaders.Add(Constants.Cluster);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Pick Event conversion.", missingHeaders);
    }

    public void CheckMissingHeadersSoft()
    {
        var missingHeaders = new List<string>();

        if (Cluster == -1) Cluster = Container;
        if (WaveID == -1) WaveID = Cluster;
        if (WorkAssignment == -1) WorkAssignment = Container;
        if (Store == -1) Store = Container;
        if (DeviceID == -1) DeviceID = OperatorID;

        if (Timestamp == -1) missingHeaders.Add(Constants.Timestamp);
        if (OperatorID == -1) missingHeaders.Add(Constants.OperatorID);
        if (OperatorName == -1) missingHeaders.Add(Constants.OperatorName);
        if (Qty == -1) missingHeaders.Add(Constants.Qty);
        if (Container == -1) missingHeaders.Add(Constants.Container);
        if (Tech == -1) missingHeaders.Add(Constants.Tech);
        if (ZoneID == -1) missingHeaders.Add(Constants.ZoneID);
        if (WaveID == -1) missingHeaders.Add(Constants.WaveID);
        if (WorkAssignment == -1) missingHeaders.Add(Constants.WorkAssignment);
        if (Store == -1) missingHeaders.Add(Constants.Store);
        if (DeviceID == -1) missingHeaders.Add(Constants.DeviceID);
        if (SkuID == -1) missingHeaders.Add(Constants.SkuID);
        if (SkuDescription == -1) missingHeaders.Add(Constants.SkuDesc);
        if (Cluster == -1) missingHeaders.Add(Constants.Cluster);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Pick Event conversion.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        Timestamp, OperatorID, OperatorName, Qty, Container, Tech, ZoneID, WaveID, WorkAssignment, Store, DeviceID,
        SkuID, SkuDescription, Cluster
    }.Max();
}

public class MispickIndices : IColumnIndexer
{
    public int ShipDate { get; set; }
    public int ReceiveDate { get; set; }
    public int CartonID { get; set; }
    public int ItemNumber { get; set; }
    public int ItemDesc { get; set; }
    public int ActionNotes { get; set; }
    public int OriginalQty { get; set; }
    public int ReceiveQty { get; set; }
    public int VarianceQty { get; set; }
    public int PostDate { get; set; }

    // Use as potential place holder.
    public int Date { get; set; }

    public MispickIndices(string[] headers, bool softCheck = false)
    {
        SetIndices(headers);

        if (softCheck)
            CheckMissingHeadersSoft();
        else

            CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        ShipDate = Array.IndexOf(headers, Constants.ShipDate);
        if (ShipDate == -1) ShipDate = Array.IndexOf(headers, "Ship Date");
        ReceiveDate = Array.IndexOf(headers, Constants.ReceiveDate);
        CartonID = Array.IndexOf(headers, Constants.CartonID);
        ItemNumber = Array.IndexOf(headers, Constants.ItemNumber);
        ItemDesc = Array.IndexOf(headers, Constants.ItemDesc);
        ActionNotes = Array.IndexOf(headers, Constants.ActionNotes);
        OriginalQty = Array.IndexOf(headers, Constants.OriginalQty);
        if (OriginalQty == -1) OriginalQty = Array.IndexOf(headers, "Orig");
        ReceiveQty = Array.IndexOf(headers, Constants.ReceiveQty);
        VarianceQty = Array.IndexOf(headers, Constants.VarianceQty);
        if (VarianceQty == -1) VarianceQty = Array.IndexOf(headers, "Adj");
        PostDate = Array.IndexOf(headers, Constants.PostDate);

        Date = Array.IndexOf(headers, "Date");
        if (Date == -1) Date = Array.IndexOf(headers, "DATE");
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (ShipDate == -1) missingHeaders.Add(Constants.ShipDate);
        if (ReceiveDate == -1) missingHeaders.Add(Constants.ReceiveDate);
        if (CartonID == -1) missingHeaders.Add(Constants.CartonID);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (ItemDesc == -1) missingHeaders.Add(Constants.ItemDesc);
        if (ActionNotes == -1) missingHeaders.Add(Constants.ActionNotes);
        if (OriginalQty == -1) missingHeaders.Add(Constants.OriginalQty);
        if (ReceiveQty == -1) missingHeaders.Add(Constants.ReceiveQty);
        if (VarianceQty == -1) missingHeaders.Add(Constants.VarianceQty);
        if (PostDate == -1) missingHeaders.Add(Constants.PostDate);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Mispick conversion.", missingHeaders);
    }

    public void CheckMissingHeadersSoft()
    {
        var missingHeaders = new List<string>();

        if (ShipDate == -1) ShipDate = Date;
        if (ReceiveDate == -1) ReceiveDate = ShipDate;
        if (PostDate == -1) PostDate = ShipDate;

        // Some old data only has a variance qty. Use that to fill in missing original and receive.
        if (OriginalQty == -1) OriginalQty = VarianceQty;
        if (ReceiveQty == -1) ReceiveQty = VarianceQty;

        // Don't worry about not having action notes column. Use variance as fill in.
        if (ActionNotes == -1) ActionNotes = VarianceQty;

        if (ShipDate == -1) missingHeaders.Add(Constants.ShipDate);
        if (ReceiveDate == -1) missingHeaders.Add(Constants.ReceiveDate);
        if (CartonID == -1) missingHeaders.Add(Constants.CartonID);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (ItemDesc == -1) missingHeaders.Add(Constants.ItemDesc);
        if (ActionNotes == -1) missingHeaders.Add(Constants.ActionNotes);
        if (OriginalQty == -1) missingHeaders.Add(Constants.OriginalQty);
        if (ReceiveQty == -1) missingHeaders.Add(Constants.ReceiveQty);
        if (VarianceQty == -1) missingHeaders.Add(Constants.VarianceQty);
        if (PostDate == -1) missingHeaders.Add(Constants.PostDate);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Mispick conversion.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        ShipDate, ReceiveDate, CartonID, ItemNumber, ItemDesc, ActionNotes, OriginalQty, ReceiveQty, VarianceQty,
        PostDate
    }.Max();
}

public class BatchTOLineIndices : IColumnIndexer
{
    public int StoreNo { get; set; }
    public int Ctns { get; set; }
    public int Weight { get; set; }
    public int Cube { get; set; }
    public int CCN { get; set; }
    public int CtnType { get; set; }
    public int StartZone { get; set; }
    public int EndZone { get; set; }
    public int StartBin { get; set; }
    public int EndBin { get; set; }
    public int BatchNo { get; set; }
    public int Date { get; set; }
    public int BaseUnits { get; set; }
    public int WaveNumber { get; set; }

    public BatchTOLineIndices(string[] headers)
    {
        SetIndices(headers);

        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        StoreNo = Array.IndexOf(headers, Constants.StoreNo);
        Ctns = Array.IndexOf(headers, Constants.Ctns);
        Weight = Array.IndexOf(headers, Constants.Weight_Kg);
        Cube = Array.IndexOf(headers, Constants.CubeM);
        CCN = Array.IndexOf(headers, Constants.CCN);
        CtnType = Array.IndexOf(headers, Constants.CtnType);
        StartZone = Array.IndexOf(headers, Constants.StartZone);
        EndZone = Array.IndexOf(headers, Constants.EndZone);
        StartBin = Array.IndexOf(headers, Constants.StartBin);
        EndBin = Array.IndexOf(headers, Constants.EndBin);
        BatchNo = Array.IndexOf(headers, Constants.TOBatchNo);
        Date = Array.IndexOf(headers, Constants.Date);
        BaseUnits = Array.IndexOf(headers, Constants.BaseUnits);
        WaveNumber = Array.IndexOf(headers, Constants.WaveNumber);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (StoreNo == -1) missingHeaders.Add(Constants.StoreNo);
        if (Ctns == -1) missingHeaders.Add(Constants.Ctns);
        if (Weight == -1) missingHeaders.Add(Constants.Weight_Kg);
        if (Cube == -1) missingHeaders.Add(Constants.CubeM);
        if (CCN == -1) missingHeaders.Add(Constants.CCN);
        if (CtnType == -1) missingHeaders.Add(Constants.CtnType);
        if (StartZone == -1) missingHeaders.Add(Constants.StartZone);
        if (EndZone == -1) missingHeaders.Add(Constants.EndZone);
        if (StartBin == -1) missingHeaders.Add(Constants.StartBin);
        if (EndBin == -1) missingHeaders.Add(Constants.EndBin);
        if (BatchNo == -1) missingHeaders.Add(Constants.TOBatchNo);
        if (Date == -1) missingHeaders.Add(Constants.Date);
        if (BaseUnits == -1) missingHeaders.Add(Constants.BaseUnits);
        if (WaveNumber == -1) missingHeaders.Add(Constants.WaveNumber);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Batch TO Lines.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        StoreNo, Ctns, Weight, Cube, CCN, CtnType, StartZone, EndZone, StartBin, EndBin, BatchNo, Date, BaseUnits, WaveNumber
    }.Max();
}

public class BatchIndices : IColumnIndexer
{
    public int BatchNo { get; set; }
    public int Description { get; set; }
    public int CreatedOn { get; set; }
    public int CreatedBy { get; set; }
    public int LastTimeCartonizedDate { get; set; }
    public int LastTimeCartonizedTime { get; set; }
    public int Cartons { get; set; }
    public int Units { get; set; }
    public int PTLFileCreated { get; set; }
    public int Cartonized { get; set; }
    public int ShipmentCreated { get; set; }
    public int FullyShipped { get; set; }

    public BatchIndices(string[] headers)
    {
        SetIndices(headers);

        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        BatchNo = Array.IndexOf(headers, Constants.BatchNo);
        Description = Array.IndexOf(headers, Constants.BatchDescription);
        CreatedOn = Array.IndexOf(headers, Constants.CreatedOn);
        CreatedBy = Array.IndexOf(headers, Constants.CreatedBy);
        LastTimeCartonizedDate = Array.IndexOf(headers, Constants.LastTimeCartonizedDate);
        LastTimeCartonizedTime = Array.IndexOf(headers, Constants.LastTimeCartonizedTime);
        Cartons = Array.IndexOf(headers, Constants.Cartons);
        Units = Array.IndexOf(headers, Constants.Units);
        PTLFileCreated = Array.IndexOf(headers, Constants.PTLFileCreated);
        Cartonized = Array.IndexOf(headers, Constants.Cartonized);
        ShipmentCreated = Array.IndexOf(headers, Constants.ShipmentCreated);
        FullyShipped = Array.IndexOf(headers, Constants.FullyShipped);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (BatchNo == -1) missingHeaders.Add(Constants.BatchNo);
        if (Description == -1) missingHeaders.Add(Constants.BatchDescription);
        if (CreatedOn == -1) missingHeaders.Add(Constants.CreatedOn);
        if (CreatedBy == -1) missingHeaders.Add(Constants.CreatedBy);
        if (LastTimeCartonizedDate == -1) missingHeaders.Add(Constants.LastTimeCartonizedDate);
        if (LastTimeCartonizedTime == -1) missingHeaders.Add(Constants.LastTimeCartonizedTime);
        if (Cartons == -1) missingHeaders.Add(Constants.Cartons);
        if (Units == -1) missingHeaders.Add(Constants.Units);
        if (PTLFileCreated == -1) missingHeaders.Add(Constants.PTLFileCreated);
        if (Cartonized == -1) missingHeaders.Add(Constants.Cartonized);
        if (ShipmentCreated == -1) missingHeaders.Add(Constants.ShipmentCreated);
        if (FullyShipped == -1) missingHeaders.Add(Constants.FullyShipped);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Batch TO Lines.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        BatchNo, Description, CreatedOn, CreatedBy, LastTimeCartonizedDate, LastTimeCartonizedTime, Cartons, Units, PTLFileCreated
    }.Max();
}

public class PickLineIndices : IColumnIndexer
{
    // Required
    public int Action { get; set; }
    public int LocationCode { get; set; }
    public int ZoneCode { get; set; }
    public int Number { get; set; }
    public int LineNo { get; set; }
    public int CartonID { get; set; }
    public int BatchNo { get; set; }
    public int PickerID { get; set; }
    public int BinCode { get; set; }
    public int ItemNumber { get; set; }
    public int Quantity { get; set; }
    public int QtyBase { get; set; }
    public int QtyPerUoM { get; set; }
    public int UoMCode { get; set; }
    public int DueDate { get; set; }

    // Optional
    public int SourceNo { get; set; }
    public int SourceLineNo { get; set; }
    public int Description { get; set; }
    public int QtyOutstanding { get; set; }
    public int QtyHandled { get; set; }
    public int QtyToHandle { get; set; }

    public PickLineIndices(string[] headers, bool softCheck = false)
    {
        SetIndices(headers);

        if (softCheck)
            CheckMissingHeadersSoft();
        else
            CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Action = Array.IndexOf(headers, Constants.Action);
        LocationCode = Array.IndexOf(headers, Constants.LocationCode);
        ZoneCode = Array.IndexOf(headers, Constants.ZoneCode);
        Number = Array.IndexOf(headers, Constants.Number);
        LineNo = Array.IndexOf(headers, Constants.LineNo);
        CartonID = Array.IndexOf(headers, Constants.CartonID);
        BatchNo = Array.IndexOf(headers, Constants.BatchNo);
        PickerID = Array.IndexOf(headers, Constants.PickerID);
        BinCode = Array.IndexOf(headers, Constants.BinCode);
        ItemNumber = Array.IndexOf(headers, Constants.ItemNumber);
        Quantity = Array.IndexOf(headers, Constants.Quantity);
        QtyBase = Array.IndexOf(headers, Constants.QtyBase);
        QtyPerUoM = Array.IndexOf(headers, Constants.QtyPerUoM);
        UoMCode = Array.IndexOf(headers, Constants.UoMCode);
        DueDate = Array.IndexOf(headers, Constants.DueDate);
        SourceNo = Array.IndexOf(headers, Constants.SourceNo);
        SourceLineNo = Array.IndexOf(headers, Constants.SourceLineNo);
        Description = Array.IndexOf(headers, Constants.Description);
        QtyOutstanding = Array.IndexOf(headers, Constants.QtyOutstanding);
        QtyHandled = Array.IndexOf(headers, Constants.QtyHandled);
        QtyToHandle = Array.IndexOf(headers, Constants.QtyToHandle);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Action == -1) missingHeaders.Add(Constants.Action);
        if (LocationCode == -1) missingHeaders.Add(Constants.LocationCode);
        if (ZoneCode == -1) missingHeaders.Add(Constants.ZoneCode);
        if (Number == -1) missingHeaders.Add(Constants.Number);
        if (LineNo == -1) missingHeaders.Add(Constants.LineNo);
        if (CartonID == -1) missingHeaders.Add(Constants.CartonID);
        if (BatchNo == -1) missingHeaders.Add(Constants.BatchNo);
        if (PickerID == -1) missingHeaders.Add(Constants.PickerID);
        if (BinCode == -1) missingHeaders.Add(Constants.BinCode);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (Quantity == -1) missingHeaders.Add(Constants.Quantity);
        if (QtyBase == -1) missingHeaders.Add(Constants.QtyBase);
        if (QtyPerUoM == -1) missingHeaders.Add(Constants.QtyPerUoM);
        if (UoMCode == -1) missingHeaders.Add(Constants.UoMCode);
        if (DueDate == -1) missingHeaders.Add(Constants.DueDate);
        if (SourceNo == -1) missingHeaders.Add(Constants.SourceNo);
        if (SourceLineNo == -1) missingHeaders.Add(Constants.SourceLineNo);
        if (Description == -1) missingHeaders.Add(Constants.Description);
        if (QtyOutstanding == -1) missingHeaders.Add(Constants.QtyOutstanding);
        if (QtyHandled == -1) missingHeaders.Add(Constants.QtyHandled);
        if (QtyToHandle == -1) missingHeaders.Add(Constants.QtyToHandle);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Batch TO Lines.", missingHeaders);
    }

    public void CheckMissingHeadersSoft()
    {
        var missingHeaders = new List<string>();

        if (SourceNo == -1) SourceNo = Number;
        if (SourceLineNo == -1) SourceLineNo = LineNo;
        if (Description == -1) Description = ItemNumber;
        if (QtyOutstanding == -1) QtyOutstanding = Quantity;
        if (QtyToHandle == -1) QtyToHandle = Quantity;
        if (QtyHandled == -1) QtyHandled = Quantity;

        if (Action == -1) missingHeaders.Add(Constants.Action);
        if (LocationCode == -1) missingHeaders.Add(Constants.LocationCode);
        if (ZoneCode == -1) missingHeaders.Add(Constants.ZoneCode);
        if (Number == -1) missingHeaders.Add(Constants.Number);
        if (LineNo == -1) missingHeaders.Add(Constants.LineNo);
        if (CartonID == -1) missingHeaders.Add(Constants.CartonID);
        if (BatchNo == -1) missingHeaders.Add(Constants.BatchNo);
        if (PickerID == -1) missingHeaders.Add(Constants.PickerID);
        if (BinCode == -1) missingHeaders.Add(Constants.BinCode);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (Quantity == -1) missingHeaders.Add(Constants.Quantity);
        if (QtyBase == -1) missingHeaders.Add(Constants.QtyBase);
        if (QtyPerUoM == -1) missingHeaders.Add(Constants.QtyPerUoM);
        if (UoMCode == -1) missingHeaders.Add(Constants.UoMCode);
        if (DueDate == -1) missingHeaders.Add(Constants.DueDate);
        if (SourceNo == -1) missingHeaders.Add(Constants.SourceNo);
        if (SourceLineNo == -1) missingHeaders.Add(Constants.SourceLineNo);
        if (Description == -1) missingHeaders.Add(Constants.Description);
        if (QtyOutstanding == -1) missingHeaders.Add(Constants.QtyOutstanding);
        if (QtyHandled == -1) missingHeaders.Add(Constants.QtyHandled);
        if (QtyToHandle == -1) missingHeaders.Add(Constants.QtyToHandle);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Batch TO Lines.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        Action, LocationCode, ZoneCode, Number, LineNo, CartonID, BatchNo, PickerID, BinCode, ItemNumber, Quantity,
        QtyBase, QtyPerUoM, UoMCode, DueDate, SourceNo, SourceLineNo, Description, QtyOutstanding, QtyToHandle,
        QtyHandled
    }.Max();
}

public class StoreIndices : IColumnIndexer
{
    public int Store { get; set; }
    public int CCNRegion { get; set; }
    public int Wave { get; set; }
    public int RoadCCN { get; set; }
    public int ShippingDays { get; set; }
    public int MBRegion { get; set; }
    public int RoadRegion { get; set; }
    public int SortingLane { get; set; }
    public int State { get; set; }
    public int Region { get; set; }
    public int StoreType { get; set; }

    public StoreIndices(string[] headers, bool softCheck = false)
    {
        SetIndices(headers);
        if (softCheck)
            CheckMissingHeadersSoft();
        else
            CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Store = Array.IndexOf(headers, Constants.Store);
        CCNRegion = Array.IndexOf(headers, Constants.CCNRegion);
        Wave = Array.IndexOf(headers, Constants.Wave);
        RoadCCN = Array.IndexOf(headers, Constants.RoadCCN);
        ShippingDays = Array.IndexOf(headers, Constants.ShippingDays);
        MBRegion = Array.IndexOf(headers, Constants.MBRegion);
        RoadRegion = Array.IndexOf(headers, Constants.RoadRegion);
        SortingLane = Array.IndexOf(headers, Constants.SortingLane);
        if (SortingLane == -1) SortingLane = Array.IndexOf(headers, "Sorting Lane Region");
        State = Array.IndexOf(headers, Constants.State);
        Region = Array.IndexOf(headers, Constants.Region);
        StoreType = Array.IndexOf(headers, Constants.StoreType);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Store == -1) missingHeaders.Add(Constants.Store);
        if (CCNRegion == -1) missingHeaders.Add(Constants.CCNRegion);
        if (Wave == -1) missingHeaders.Add(Constants.Wave);
        if (RoadCCN == -1) missingHeaders.Add(Constants.RoadCCN);
        if (ShippingDays == -1) missingHeaders.Add(Constants.ShippingDays);
        if (MBRegion == -1) missingHeaders.Add(Constants.MBRegion);
        if (RoadRegion == -1) missingHeaders.Add(Constants.RoadRegion);
        if (SortingLane == -1) missingHeaders.Add(Constants.SortingLane);
        if (State == -1) missingHeaders.Add(Constants.State);
        if (Region == -1) missingHeaders.Add(Constants.Region);
        if (StoreType == -1) missingHeaders.Add(Constants.StoreType);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Store conversion.", missingHeaders);
    }

    public void CheckMissingHeadersSoft()
    {
        var missingHeaders = new List<string>();

        if (RoadCCN == -1) RoadCCN = CCNRegion;
        if (MBRegion == -1) MBRegion = Region;
        if (RoadRegion == -1) RoadRegion = MBRegion;
        if (SortingLane == -1) SortingLane = ShippingDays;
        if (State == -1) State = CCNRegion;
        if (StoreType == -1) StoreType = Wave;

        if (Store == -1) missingHeaders.Add(Constants.Store);
        if (CCNRegion == -1) missingHeaders.Add(Constants.CCNRegion);
        if (Wave == -1) missingHeaders.Add(Constants.Wave);
        if (RoadCCN == -1) missingHeaders.Add(Constants.RoadCCN);
        if (ShippingDays == -1) missingHeaders.Add(Constants.ShippingDays);
        if (MBRegion == -1) missingHeaders.Add(Constants.MBRegion);
        if (RoadRegion == -1) missingHeaders.Add(Constants.RoadRegion);
        if (SortingLane == -1) missingHeaders.Add(Constants.SortingLane);
        if (State == -1) missingHeaders.Add(Constants.State);
        if (Region == -1) missingHeaders.Add(Constants.Region);
        if (StoreType == -1) missingHeaders.Add(Constants.StoreType);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Store conversion.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        Store, CCNRegion, Wave, RoadCCN, ShippingDays, MBRegion, RoadRegion, SortingLane, State, Region, StoreType
    }.Max();
}