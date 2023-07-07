using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Inventory;

namespace Uranus.Staff.Models;

public enum EQAStatus
{
    OK,
    ItemNotScanned,
    NewItem,
    QtyMismatch,
    ShortPick
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
    public DateTime Date { get; set; }

    public string MispickID { get; set; }  // [CartonID]:[ItemNumber], e.g. 13517596:293505
    [Ignore] public bool HasError => QAStatus is not (EQAStatus.OK or EQAStatus.ShortPick);
    [Ignore] public bool AtFault => HasError && !Blackout && (ErrorType ?? "").ToUpper().Contains("PICK");  // Specifically picker error.

    public bool Blackout { get; set; }
    
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
        if (!AtFault) return null;
        
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
}