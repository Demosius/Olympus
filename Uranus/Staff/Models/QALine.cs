using System;
using System.Text.RegularExpressions;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Inventory;
// ReSharper disable StringLiteralTypo

namespace Uranus.Staff.Models;

public enum EQAStatus
{
    OK,
    ItemNotScanned,
    NewItem,
    QtyMismatch,
    ShortPick
}

public enum EErrorCategory
{
    None,
    Picker,
    Replen,
    Stocking,
    Receive,
    HeatMap,
    OtherDept,
    Supplier,
    System,
    OtherExternal,
    QAError
}

public enum EErrorAllocation
{
    Warehouse,
    External
}

public class QALine
{
    [PrimaryKey] public string ID { get; set; } // [CartonID]:[ItemNumber]:[UoM], e.g. 13517596:293505:EACH
    [ForeignKey(typeof(QACarton))] public string CartonID { get; set; }
    public int ItemNumber { get; set; }
    public string ItemDescription { get; set; }
    public int PickQty { get; set; }
    public EUoM UoM { get; set; }
    public int QtyPerUoM { get; set; }
    public int PickQtyBase { get; set; }
    public int QAQty { get; set; }
    public int VarianceQty { get; set; }
    public EQAStatus QAStatus { get; set; }
    public string BinCode { get; set; }
    public string PickerRFID { get; set; }
    public string ErrorType { get; set; }
    [Indexed] public DateTime Date { get; set; }
    public bool Seek { get; set; }
    public bool Fixed { get; set; }
    public string MispickID { get; set; }  // [CartonID]:[ItemNumber], e.g. 13517596:293505
    public bool External { get; set; }

    [Ignore] public bool HasError => QAStatus is not (EQAStatus.OK or EQAStatus.ShortPick);
    [Ignore] public bool QAError => HasError && ErrorType.ToUpper().Contains("QA");
    [Ignore] public bool PickerError => HasError && ErrorType.ToUpper().Contains("PICK");
    [Ignore] public bool ReplenError => HasError && ErrorType.ToUpper().Contains("REPLEN");
    [Ignore] public bool StockingError => HasError && ErrorType.ToUpper().Contains("STOCK");
    [Ignore] public bool ReceiveError => HasError && ErrorType.ToUpper().Contains("RECEIV");
    [Ignore] public bool HeatMapError => HasError && ErrorType.ToUpper().Contains("HEATMAP");
    [Ignore] public bool SupplierError => HasError && (ErrorType.ToUpper().Contains("SUPPLIER") || ErrorType.ToUpper().Contains("VENDOR"));
    [Ignore] public bool SystemError => HasError && ErrorType.ToUpper().Contains("SYSTEM");

    [Ignore]
    public EErrorCategory ErrorCategory =>
        !HasError ? EErrorCategory.None :
        QAError ? EErrorCategory.QAError :
        PickerError ? EErrorCategory.Picker :
        ReplenError ? EErrorCategory.Replen :
        StockingError ? EErrorCategory.Stocking :
        ReceiveError ? EErrorCategory.Receive :
        HeatMapError ? EErrorCategory.HeatMap :
        SupplierError ? EErrorCategory.Supplier :
        SystemError ? EErrorCategory.System :
        External ? EErrorCategory.OtherExternal :
        EErrorCategory.OtherDept;

    [Ignore]
    public EErrorAllocation ErrorAllocation =>
        ErrorCategory is EErrorCategory.OtherExternal or EErrorCategory.Supplier or EErrorCategory.System
            ? EErrorAllocation.External
            : EErrorAllocation.Warehouse;


    private int TrueQpUoM => NZ(QtyPerUoM, NZ(PickQtyBase / NZ(PickQty)));
    [Ignore] public int UnitVariance => TrueQpUoM * VarianceQty;
    [Ignore] public int ErrorQty => Math.Abs(VarianceQty);
    [Ignore] public int ErrorUnitQty => Math.Abs(UnitVariance);
    [Ignore] public int QAQtyBase => TrueQpUoM * QAQty;

    private bool? ptl;
    [Ignore] public bool PTL => ptl ??= Regex.IsMatch(BinCode, @"^\w\d{3}$", RegexOptions.IgnoreCase);
    [Ignore] public bool PTV => !PTL;
    [Ignore] public bool RFT => PTV;

    [Ignore] public Employee? Picker { get; set; }
    [Ignore] public string PickerName => Picker?.FullName ?? string.Empty;
    [Ignore] public Employee? QABy => QACarton?.QAOperator;
    [Ignore] public Mispick? Mispick { get; set; }

    [ManyToOne(nameof(CartonID), nameof(Models.QACarton.QALines), CascadeOperations = CascadeOperation.CascadeRead)]
    public QACarton? QACarton { get; set; }

    public QALine()
    {
        ID = Guid.NewGuid().ToString();
        CartonID = string.Empty;
        ItemDescription = string.Empty;
        BinCode = string.Empty;
        PickerRFID = string.Empty;
        MispickID = string.Empty;
        Date = DateTime.Today;
        ErrorType = string.Empty;
    }

    public static string GetID(string cartonID, int itemNumber, EUoM uom) => $"{cartonID}:{itemNumber}:{uom}";
    public static string GetMispickID(string cartonID, int itemNumber) => $"{cartonID}:{itemNumber}";

    public void SetID()
    {
        ID = GetID(CartonID, ItemNumber, UoM);
        MispickID = GetMispickID(CartonID, ItemNumber);
    }

    public Mispick? GenerateMispick()
    {
        if (!PickerError) return null;

        MispickID = GetMispickID(CartonID, ItemNumber);

        var mispick = new Mispick
        {
            ID = MispickID,
            ShipmentDate = Date,
            PostedDate = Date,
            ReceivedDate = Date,
            CartonID = CartonID,
            ItemNumber = ItemNumber,
            ItemDescription = ItemDescription,
            OriginalQty = PickQty,
            ReceivedQty = QAQty,
            VarianceQty = VarianceQty,
            QAAllocated = true,
            ErrorDate = Date,
            AssignedRF_ID = PickerRFID,
            AssignedDematicID = Picker?.DematicID ?? ""
        };

        Mispick = mispick;
        return mispick;
    }

    private static int NZ(int n, int alternate = 1) => n == 0 ? alternate : n;
}