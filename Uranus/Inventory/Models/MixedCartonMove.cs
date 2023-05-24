using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class MixedCartonMove : Move
{
    public MixedCarton MixedCarton { get; set; }
    public List<Move> Moves { get; set; }

    public bool SuccessfullyGenerated { get; set; }

    private string? mixedContentDisplay;
    public string MixedContentDisplay
    {
        get => mixedContentDisplay ??= MixedCarton.GetMixedContentDisplay();
        set => mixedContentDisplay = value;
    }

    public MixedCartonMove(MixedCarton mixedCarton)
    {
        MixedCarton = mixedCarton;
        Moves = new List<Move>();
    }

    /// <summary>
    /// Determine MC Move based on list of given moves.
    /// Will check that they are appropriately matched and create a mixed carton type based on these moves.
    /// </summary>
    /// <param name="moves">Group of moves.</param>
    public MixedCartonMove(List<Move> moves)
    {
        // Validate.
        if (!IsValidMoveList(moves))
        {
            MixedCarton = new MixedCarton();
            Moves = new List<Move>();
            return;
        }

        Moves = moves;
        MixedCarton = GetMixedCarton(Moves);

        SetBaseMoveValues();

        SuccessfullyGenerated = true;
    }

    /// <summary>
    /// Determine MC Move based on given mc template and list of given moves.
    /// Will check that they are appropriately matched based on the given mc type, and other factors.
    /// </summary>
    /// <param name="mixedCarton">Mixed Carton Template object.</param>
    /// <param name="moves">Group of moves.</param>
    public MixedCartonMove(MixedCarton mixedCarton, List<Move> moves)
    {
        MixedCarton = mixedCarton;
        Moves = moves;

        if (!IsValidMoveList(moves) || !MixedCarton.IsValidMoveSet(moves)) return;

        SetBaseMoveValues();

        SuccessfullyGenerated = true;
    }

    /// <summary>
    /// Checks that a given list of moves is valid.
    /// Must have more than one move. Only eaches. Matching bin locations.
    /// </summary>
    /// <param name="moves"></param>
    /// <returns></returns>
    private static bool IsValidMoveList(IReadOnlyCollection<Move> moves)
    {
        var primeMove = moves.FirstOrDefault();

        return primeMove is not null &&
               moves.Count > 1 &&
               !moves.Any(m =>
                   m.TakeCases > 0 ||
                   m.TakePacks > 0 ||
                   m.TakeBinID !=
                   primeMove.TakeBinID ||
                   m.PlaceBinID !=
                   primeMove.PlaceBinID);
    }

    /// <summary>
    /// Set up all basic Move values as determined and copied from the listed moves - particularly the first move in list.
    /// Assumes move list is accurate, valid, and usable.
    /// Also assumes MixedCarton Object is accurately set.
    /// </summary>
    private void SetBaseMoveValues()
    {
        ItemNumber = (int)Math.Round(Moves.Average(m => m.ItemNumber));

        Item = new NAVItem(ItemNumber)
        {
            Description = MixedCarton.Name
        };
        var itemCase = new NAVUoM(Item, EUoM.CASE)
        {
            QtyPerUoM = MixedCarton.UnitsPerCarton,
            Cube = MixedCarton.Cube,
            Weight = MixedCarton.Weight,
            Length = MixedCarton.Length,
            Height = MixedCarton.Height,
            Width = MixedCarton.Width
        };

        Item.UoMs.Add(itemCase);
        Item.Case = itemCase;

        var firstMove = Moves.First();

        TakeBin = firstMove.TakeBin;
        PlaceBin = firstMove.PlaceBin;
        TakeBinID = firstMove.TakeBinID;
        PlaceBinID = firstMove.PlaceBinID;
        BatchID = firstMove.BatchID;
        Batch = firstMove.Batch;
        AssignedOperator = firstMove.AssignedOperator;
        AssignedOperatorID = firstMove.AssignedOperatorID;

        IndividualPriority = Moves.Min(m => m.IndividualPriority);
        GroupPriority = Moves.Min(m => m.GroupPriority);

        TakeCases = firstMove.TakeEaches / MixedCarton.Items.First(i => i.ItemNumber == firstMove.ItemNumber).QtyPerCarton;
        PlaceCases = TakeCases;
    }

    /// <summary>
    /// Finds the common values in the descriptions across multiple items.
    /// </summary>
    /// <param name="items">List of items that, in theory, have something in common.</param>
    /// <returns>Common String</returns>
    public static string GetDescription(IEnumerable<NAVItem> items) =>
        Utility.LongestCommonSubstring(items.Select(i => i.Description)).Trim();

    /// <summary>
    /// Given moves assumed to represent MC movement(s), generate a Mixed carton item that appropriately represents the items and the ratios.
    /// </summary>
    /// <param name="moves">Move objects that together represent items moving in mixed cartons - with an assumed consistent ratio.</param>
    /// <returns>Calculated MixedCarton Object.</returns>
    public static MixedCarton GetMixedCarton(IEnumerable<Move> moves)
    {
        var moveList = moves.ToList();

        var items = moveList.Where(m => m.Item is not null).Select(m => m.Item!).ToList();

        var mc = new MixedCarton
        {
            Name = GetDescription(items)
        };

        var mcQty = Utility.HCF(moveList.Select(m => m.TakeEaches));

        foreach (var move in moveList.Where(move => move.Item is not null))
        {
            // Creation of MCI(mc, i) adds itself to both MC and I.
            _ = new MixedCartonItem(mc, move.Item!)
            {
                QtyPerCarton = move.TakeEaches / mcQty
            };
        }

        mc.CalculateValuesFromItems();
        
        return mc;
    }

    /// <summary>
    /// Given a list of moves that may result in a mixed carton move, attempt to generate a mixed carton movement.
    /// Will only generate one if prerequisites are met.
    /// </summary>
    /// <returns>Null if no MC Move is available.</returns>
    public static MixedCartonMove? GenerateMixedCartonMove(List<Move> moves)
    {
        var move = new MixedCartonMove(moves);
        return move.SuccessfullyGenerated ? move : null;
    }

    /// <summary>
    /// Given a list of moves that may result in a mixed carton move - and a specific mc template to match against it.
    /// Attempt to generate a mixed carton movement.
    /// Will only generate one if prerequisites are met.
    /// </summary>
    /// <returns>Null if no MC Move is available.</returns>
    public static MixedCartonMove? GenerateMixedCartonMove(MixedCarton mc, List<Move> moves)
    {
        var move = new MixedCartonMove(mc, moves);
        return move.SuccessfullyGenerated ? move : null;
    }

    /// <summary>
    /// Generate mixed carton move list.
    /// </summary>
    /// <param name="mcTemplates">Reference Mixed Carton Template list. Will add to it as new MC Templates are created.</param>
    /// <param name="moves">Reference Move List. Moves will be removed as they are put into MC Moves.</param>
    /// <returns>List of Mixed Carton Move objects.</returns>
    public static List<MixedCartonMove> GenerateMixedCartonMoveList(ref List<MixedCarton> mcTemplates, ref List<Move> moves)
    {
        var mcMoves = new List<MixedCartonMove>();

        var moveGroups = moves.Where(m => m.TakeCases == 0 && m.TakePacks == 0)
            .GroupBy(m => $"{m.TakeBinID}:{m.PlaceBinID}")
            .Where(g => g.Count() > 1)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (_, moveGroup) in moveGroups)
        {
            MixedCartonMove? mcMove = null;

            // Check first against known MixedCarton templates.
            // Item count should match - save from checking against templates unnecessarily.
            var templates = mcTemplates.Where(mc => mc.Items.Count == moveGroup.Count);
            foreach (var mixedCarton in templates)
            {
                mcMove = GenerateMixedCartonMove(mixedCarton, moveGroup);
                if (mcMove is not null) break;
            }

            // If template wasn't matched, try to create one.
            if (mcMove is null)
            {
                mcMove = GenerateMixedCartonMove(moveGroup);
                if (mcMove is not null) mcTemplates.Add(mcMove.MixedCarton);
            }

            if (mcMove is null) continue;

            mcMoves.Add(mcMove);
            foreach (var move in moveGroup)
            {
                moves.Remove(move);
            }
        }
        
        return mcMoves;
    }
}