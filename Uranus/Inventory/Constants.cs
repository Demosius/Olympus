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
    #region Strings

    public const string LocationCode = "Location Code";
    public const string BinCode = "Bin Code";
    public const string ItemNumber = "Item No.";
    public const string ItemBarcode = "ItemBarcode";
    public const string UoMCode = "Unit of Measure Code";
    public const string Qty = "Quantity";
    public const string PickQty = "Pick Qty.";
    public const string PutAwayQty = "Put-away Qty.";
    public const string NegQty = "Neg. Adjmt. Qty.";
    public const string PosQty = "Pos. Adjmt. Qty.";
    public const string DateCreated = "Date Created";
    public const string TimeCreated = "Time Created";
    public const string Fixed = "Fixed";
    public const string Code = "Code";
    public const string Description = "Description";
    public const string ItemDivCode = "Item Division Code";
    public const string Name = "Name";
    public const string ZoneRank = "Zone Ranking";
    public const string ItemCode = "ItemCode";
    public const string ItemName = "ItemName";
    public const string PrimaryBarcode = "PrimaryBarcode";
    public const string NewUsed = "NewUsed";
    public const string CatCode = "CategoryCode";
    public const string PFCode = "PlatformCode";
    public const string DivCode = "DivisionCode";
    public const string GenCode = "GenreCode";
    public const string Length = "Length";
    public const string Width = "Width";
    public const string Height = "Height";
    public const string Cube = "Cubage";
    public const string Weight = "Weight";
    public const string QtyPerUoM = "Qty. per Unit of Measure";
    public const string MaxQty = "Max Qty";
    public const string InnerPack = "Inner Pack";
    public const string ExcludeCtn = "Exclude Cartonization";
    public const string LengthCM = "Length (CM)";
    public const string WidthCM = "Width (CM)";
    public const string HeightCM = "Height (CM)";
    public const string CubeCM = "CM Cubage";
    public const string WeightKG = "Weight (Kg)";
    public const string ZoneCode = "Zone Code";
    public const string Empty = "Empty";
    public const string BinAssigned = "Bin Assigned";
    public const string BinRanking = "Bin Ranking";
    public const string UsedCube = "Used Cubage";
    public const string MaxCube = "Maximum Cubage";
    public const string LastCycleCount = "CC Last Count Date";
    public const string LastPhysicalCount = "PI - Last Count Date";
    public const string DocNo = "Document No.";
    public const string TransferCode = "Transfer-to Code";
    public const string UoM = "Unit of Measure";
    public const string AvailFillQty = "Avail. UOM Fulfilment Qty";
    public const string CreatedDate = "Created On Date";
    public const string CreatedTime = "Created On Time";
    public const string Action = "Action Type";
    public const string Timestamp = "Timestamp";
    public const string OperatorID = "Operator ID";
    public const string OperatorName = "Operator Name";
    public const string Container = "Container";
    public const string Tech = "Tech Type";
    public const string ZoneID = "Zone ID";
    public const string WaveID = "Wave ID";
    public const string WorkAssignment = "Work Assignment";
    public const string Store = "Store";
    public const string DeviceID = "Device ID";
    public const string SkuID = "SKU ID";
    public const string SkuDesc = "SKU Description";
    public const string Cluster = "Cluster Ref";
    public const string ShipDate = "Actual Shipment Date";
    public const string ReceiveDate = "Actual Received Date";
    public const string CartonID = "Carton ID";
    public const string ActionNotes = "Action Notes";
    public const string OriginalQty = "Original Qty.";
    public const string ReceiveQty = "Received Qty.";
    public const string VarianceQty = "Variance Qty.";
    public const string PostDate = "Posted Date";
    public const string Q = "Qty";
    public const string ItemDesc = "Item Description";
    
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

    public static Dictionary<string, int> DematicMissPickColumns = new()
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
        Qty = Array.IndexOf(headers, Constants.Qty);
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
        if (Qty == -1) missingHeaders.Add(Constants.Qty);
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

    public int Max() => new List<int> {Code, Description}.Max();
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

    public int Max() => new List<int>{ Code, Description, ItemDivCode}.Max();
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
    public int Max() => new List<int>{ Location, Code, Description, Rank}.Max();
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
        Qty = Array.IndexOf(headers, Constants.Qty);
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
        if (Qty == -1) missingHeaders.Add(Constants.Qty);
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
        Qty = Array.IndexOf(headers, Constants.Qty);
        UoMCode = Array.IndexOf(headers, Constants.UoMCode);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Action == -1) missingHeaders.Add(Constants.Action);
        if (ItemNumber == -1) missingHeaders.Add(Constants.ItemNumber);
        if (ZoneCode == -1) missingHeaders.Add(Constants.ZoneCode);
        if (BinCode == -1) missingHeaders.Add(Constants.BinCode);
        if (Qty == -1) missingHeaders.Add(Constants.Qty);
        if (UoMCode == -1) missingHeaders.Add(Constants.UoMCode);

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int> {Action, ItemNumber, ZoneCode, BinCode, Qty, UoMCode}.Max();
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

    public PickEventIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        Timestamp = Array.IndexOf(headers, Constants.Timestamp);
        OperatorID = Array.IndexOf(headers, Constants.OperatorID);
        OperatorName = Array.IndexOf(headers, Constants.OperatorName);
        Qty = Array.IndexOf(headers, Constants.Q);
        Container = Array.IndexOf(headers, Constants.Container);
        Tech = Array.IndexOf(headers, Constants.Tech);
        ZoneID = Array.IndexOf(headers, Constants.ZoneID);
        WaveID = Array.IndexOf(headers, Constants.WaveID);
        WorkAssignment = Array.IndexOf(headers, Constants.WorkAssignment);
        Store = Array.IndexOf(headers, Constants.Store);
        DeviceID = Array.IndexOf(headers, Constants.DeviceID);
        SkuID = Array.IndexOf(headers, Constants.SkuID);
        SkuDescription = Array.IndexOf(headers, Constants.SkuDesc);
        Cluster = Array.IndexOf(headers, Constants.Cluster);
    }

    public void CheckMissingHeaders()
    {
        var missingHeaders = new List<string>();

        if (Timestamp == -1) missingHeaders.Add(Constants.Timestamp);
        if (OperatorID == -1) missingHeaders.Add(Constants.OperatorID);
        if (OperatorName == -1) missingHeaders.Add(Constants.OperatorName);
        if (Qty == -1) missingHeaders.Add(Constants.Q);
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

        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        Timestamp, OperatorID, OperatorName, Qty, Container, Tech, ZoneID, WaveID, WorkAssignment, Store, DeviceID,
        SkuID, SkuDescription, Cluster
    }.Max();
}

public class MissPickIndices : IColumnIndexer
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

    public MissPickIndices(string[] headers)
    {
        SetIndices(headers);
        CheckMissingHeaders();
    }

    public void SetIndices(string[] headers)
    {
        ShipDate = Array.IndexOf(headers, Constants.ShipDate);
        ReceiveDate = Array.IndexOf(headers, Constants.ReceiveDate);
        CartonID = Array.IndexOf(headers, Constants.CartonID);
        ItemNumber = Array.IndexOf(headers, Constants.ItemNumber);
        ItemDesc = Array.IndexOf(headers, Constants.ItemDesc);
        ActionNotes = Array.IndexOf(headers, Constants.ActionNotes);
        OriginalQty = Array.IndexOf(headers, Constants.OriginalQty);
        ReceiveQty = Array.IndexOf(headers, Constants.ReceiveQty);
        VarianceQty = Array.IndexOf(headers, Constants.VarianceQty);
        PostDate = Array.IndexOf(headers, Constants.PostDate);
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


        if (missingHeaders.Count > 0) throw new InvalidDataException("Missing columns for Bin Contents conversion.", missingHeaders);
    }

    public int Max() => new List<int>
    {
        ShipDate, ReceiveDate, CartonID, ItemNumber, ItemDesc, ActionNotes, OriginalQty, ReceiveQty, VarianceQty,
        PostDate
    }.Max();
}