using Uranus.Inventory.Models;

namespace Cadmus.Models;

public class ReceivingPutAwayLabel
{
    public string TakeZone { get; set; }
    public string TakeBin { get; set; }
    public int CaseQty { get; set; }
    public int PackQty { get; set; }
    public int EachQty { get; set; }
    public int QtyPerCase { get; set; }
    public int QtyPerPack { get; set; }
    public string Barcode { get; set; }
    public int ItemNumber { get; set; }
    public int LabelNumber { get; set; }
    public int LabelTotal { get; set; }
    public string Description { get; set; }

    public ReceivingPutAwayLabel()
    {
        TakeZone = string.Empty;
        TakeBin = string.Empty;
        Barcode = string.Empty;
        Description = string.Empty;
    }

    public ReceivingPutAwayLabel(Move move)
    {
        TakeZone = move.TakeZone?.Code ?? string.Empty;
        TakeBin = move.TakeBin?.Code ?? string.Empty;
        CaseQty = move.TakeCases;
        PackQty = move.TakePacks;
        EachQty = move.TakeEaches;
        QtyPerCase = move.Item?.QtyPerCase ?? 1;
        QtyPerPack = move.Item?.QtyPerPack ?? 1;
        Barcode = move.ItemNumber.ToString();
        ItemNumber = move.ItemNumber;
        LabelNumber = 1;
        LabelTotal = 1;
        Description = move.Item?.Description ?? string.Empty;
    }

    public ReceivingPutAwayLabel(string takeZone, string takeBin, int caseQty, int packQty, int eachQty, int qtyPerCase,
        int qtyPerPack, string barcode, int itemNumber, int labelNumber, int labelTotal, string description)

    {
        TakeZone = takeZone;
        TakeBin = takeBin;
        CaseQty = caseQty;
        PackQty = packQty;
        EachQty = eachQty;
        QtyPerCase = qtyPerCase;
        QtyPerPack = qtyPerPack;
        Barcode = barcode;
        ItemNumber = itemNumber;
        LabelNumber = labelNumber;
        LabelTotal = labelTotal;
        Description = description;
    }

}