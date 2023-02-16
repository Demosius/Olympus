﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Morpheus;
using Uranus.Inventory.Models;

namespace Cadmus.Models;

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

    public Move? Move { get; }

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
    }

    public RefOrgeMasterLabel(int priority, string batchName, string operatorName, DateTime date, string takeBin,
        bool pickAsPacks, bool web, int eachQty, int packQty, int caseQty, int qtyPerCase, int qtyPerPack,
        string placeBin, string barcode, string? checkDigits, int itemNumber, string? totalGrab,
        int labelTotal, string itemDescription, string trueOrderTakeBin, string takeZone)
    {
        Priority = priority;
        BatchName = batchName;
        OperatorName = operatorName;
        Date = date;
        TakeBin = takeBin;
        PickAsPacks = pickAsPacks;
        Web = web;
        EachQty = eachQty;
        PackQty = packQty;
        CaseQty = caseQty;
        QtyPerCase = qtyPerCase;
        QtyPerPack = qtyPerPack;
        PlaceBin = placeBin;
        Barcode = barcode;
        CheckDigits = checkDigits;
        ItemNumber = itemNumber;
        TotalGrab = totalGrab;
        LabelTotal = labelTotal;
        ItemDescription = itemDescription;
        TrueOrderTakeBin = trueOrderTakeBin;
        TakeZone = takeZone;
    }

    public RefOrgeMasterLabel(Move move)
    {
        Move = move;

        Priority = move.Priority;
        BatchName = move.BatchID;
        OperatorName = move.OperatorName;
        TakeBin = move.TakeBin?.Code ?? move.TakeBinID;
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
        TakeZone = move.TakeZone?.Code ?? move.TakeLocation ?? string.Empty;

        Web = move.BatchID.Contains("WEB") || move.BatchID == "ECons";
        Date = DateTime.Today;
        Barcode = BarcodeUtility.Encode128(move.ItemNumber.ToString());

        ItemDescription = move.Item?.Description ?? string.Empty; 

        const string pattern = @"^([\w]{2}|[\w]{4})(\d{2} \d{3})$";
        TrueOrderTakeBin = Regex.Replace(TakeBin, pattern, "A$2");

        // Total Grabs
        CalculateTotalGrabs();

        CalculateRequiredLabels();
    }

    public void CalculateTotalGrabs()
    {
        var moves = Move?.PlaceBin?.ToMoves.Where(m => m != Move && m.ItemNumber == ItemNumber).ToList() ?? new List<Move>();

        if (moves.Any())
        {
            var cases = moves.Sum(m => m.TakeCases);
            var packs = moves.Sum(m => m.TakePacks);
            var eaches = moves.Sum(m => m.TakeEaches);
            TotalGrab =
                $"(T:{(cases > 0 ? $" {cases} CASE" : "")}" +
                $"{(packs > 0 ? $"{packs} PACK" : "")}" +
                $"{(eaches > 0 ? $"{eaches} EACH" : "")})";
        }
        else
            TotalGrab = string.Empty;

    }


    public int CalculateRequiredLabels()
    {
        // Get required number of labels.
        var labelCount = CaseQty + PackQty;
        if (EachQty <= 0) return labelCount;

        var eachesPerLabel = QtyPerPack > 1 ? QtyPerPack : QtyPerCase > 1 ? QtyPerCase : 12;

        labelCount += (int)Math.Ceiling(EachQty / (double)eachesPerLabel);

        return labelCount;
    }

}