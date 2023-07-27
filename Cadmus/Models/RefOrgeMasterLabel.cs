using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Morpheus;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Cadmus.Models;

public enum EMoveType
{
    Ground, 
    Racking,
    FullPallet
}

public class RefOrgeMasterLabel
{
    public int Priority { get; set; }
    public string BatchName { get; set; }
    public string OperatorName { get; set; }
    public DateTime Date { get; set; }
    public string TakeBin { get; set; }
    public bool PickAsPacks { get; set; }
    public bool Web { get; set; }
    public int EachQty { get; set; }
    public int PackQty { get; set; }
    public int CaseQty { get; set; }
    public int QtyPerCase { get; set; }
    public int QtyPerPack { get; set; }
    public string PlaceBin { get; set; }
    public string Barcode { get; set; }
    public string? CheckDigits { get; set; }
    public int ItemNumber { get; set; }
    public string? TotalGrab { get; set; }
    public int LabelTotal { get; set; }
    public string ItemDescription { get; set; }
    public string TrueOrderTakeBin { get; set; }
    public string TakeZone { get; set; }
    public EMoveType MoveType { get; set; }

    public Move? Move { get; }

    public bool MixedCarton { get; set; }
    public string MixedContentDisplay { get; set; }

    public RefOrgeMasterLabel()
    {
        BatchName = string.Empty;
        OperatorName = string.Empty;
        TakeBin = string.Empty;
        PickAsPacks = false;
        Web = false;
        TakeZone = string.Empty;
        PlaceBin = string.Empty;
        ItemNumber = 0;
        Barcode = BarcodeUtility.Encode128(ItemNumber.ToString());
        ItemDescription = string.Empty;
        TrueOrderTakeBin = string.Empty;
        MixedContentDisplay = string.Empty;
    }

    public RefOrgeMasterLabel(Move move, int? maxLabels)
    {
        Move = move;

        MixedCarton = move is MixedCartonMove;

        Priority = move.Priority;
        BatchName = move.BatchID;
        OperatorName = move.OperatorName;
        TakeBin = move.TakeBin?.Code ?? move.TakeBinID.Split(':')[2];
        PickAsPacks = move.PlacePacks > 0;
        PlaceBin = move.PlaceBin?.Code ?? move.PlaceBinID;
        ItemNumber = move.ItemNumber;
        CheckDigits = move.PlaceBin?.CheckDigits;
        EachQty = move.TakeEaches;
        PackQty = move.TakePacks;
        CaseQty = move.TakeCases;
        QtyPerCase = move.Item?.QtyPerCase ?? 1;
        QtyPerPack = move.Item?.QtyPerPack ?? 1;
        ItemDescription = move.Item?.Description ?? string.Empty;
        TakeZone = move.TakeZone?.Code ?? move.TakeBinID.Split(':')[1];

        MoveType = move.AccessLevel == EAccessLevel.Ground ? EMoveType.Ground :
            move.FullPallet ? EMoveType.FullPallet : EMoveType.Racking;

        if (OperatorName == string.Empty) OperatorName = MoveType.ToString();

        Web = move.BatchID.Contains("WEB") || move.BatchID == "ECons";
        Date = DateTime.Today;
        Barcode = BarcodeUtility.Encode128(move.ItemNumber.ToString());

        ItemDescription = move.Item?.Description ?? string.Empty;

        const string pattern = @"^(\w{2}|\w{4})(\d{2} \d{3})$";
        TrueOrderTakeBin = Regex.Replace(TakeBin, pattern, "A$2");

        MixedContentDisplay = move is MixedCartonMove mixMove ? mixMove.MixedContentDisplay : string.Empty;
        
        CalculateRequiredLabels(maxLabels);
    }

    /// <summary>
    /// Calculates the total grab qty of this item for this place location.
    ///
    /// Run after all moves created so no data is out of sync.
    /// </summary>
    public void CalculateTotalGrabs()
    {
        var moves = Move?.PlaceBin?.ToMoves.Where(m => m != Move && m.ItemNumber == ItemNumber).ToList() ?? new List<Move>();

        if (moves.Any())
        {
            var cases = moves.Sum(m => m.TakeCases) + Move?.TakeCases;
            var packs = moves.Sum(m => m.TakePacks) + Move?.TakePacks;
            var eaches = moves.Sum(m => m.TakeEaches) + Move?.TakeEaches;
            TotalGrab =
                $"(T:{(cases > 0 ? $" {cases} CASE" : "")}" +
                $"{(packs > 0 ? $"{packs} PACK" : "")}" +
                $"{(eaches > 0 ? $"{eaches} EACH" : "")})";
        }
        else
            TotalGrab = string.Empty;

    }

    public void CalculateRequiredLabels(int? maxLabels = null)
    {
        // Get required number of labels.
        LabelTotal = CaseQty + PackQty;
        if (EachQty <= 0)
        {
            if (maxLabels < LabelTotal) LabelTotal = (int)maxLabels;
            return;
        }

        var eachesPerLabel = QtyPerPack > 1 ? QtyPerPack : QtyPerCase > 1 ? QtyPerCase : 12;

        LabelTotal += (int)Math.Ceiling(EachQty / (double)eachesPerLabel);

        if (maxLabels < LabelTotal) LabelTotal = (int)maxLabels;
    }

}