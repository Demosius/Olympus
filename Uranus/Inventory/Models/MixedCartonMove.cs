using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class MixedCartonMove : Move
{
    public MixedCarton MixedCarton { get; set; }
    public List<Move> Moves { get; set; }

    public bool SuccessfullyGenerated { get; set; }

    public MixedCartonMove(MixedCarton mixedCarton)
    {
        MixedCarton = mixedCarton;
        Moves = new List<Move>();
    }

    /// <summary>
    /// Determine MC Move based on list of given moves.
    /// Will check that they are appropriately matched and create a mixed carton type based on these moves.
    /// </summary>
    /// <param name="moves">Group of moves passed by reference, only removing from that group those that are relevant to the MC Move.</param>
    public MixedCartonMove(ref List<Move> moves)
    {
        // Only take moves involving eaches.
        // Group moves by Take and Place bins.
        Moves = moves.Where(m => m.TakeCases == 0 && m.TakePacks == 0)
            .GroupBy(m => $"{m.TakeBinID}:{m.PlaceBinID}")
            .OrderByDescending(g => g.Count())
            .First()
            .ToList();

        if (Moves.Count < 1)
        {
            MixedCarton = new MixedCarton();
            return;
        }

        MixedCarton = GetMixedCarton(Moves);

        SetBaseMoveValues();

        // Remove used moves from move list.
        foreach (var move in Moves)
        {
            moves.Remove(move);
        }

        SuccessfullyGenerated = true;
    }

    /// <summary>
    /// Determine MC Move based on given mc template and list of given moves.
    /// Will check that they are appropriately matched based on the given mc type, and other factors.
    /// </summary>
    /// <param name="mixedCarton">Mixed Carton Template object.</param>
    /// <param name="moves">Group of moves passed by reference, only removing from that group those that are relevant to the MC Move.</param>
    public MixedCartonMove(MixedCarton mixedCarton, ref List<Move> moves)
    {
        MixedCarton = mixedCarton;
        var iNos = MixedCarton.Items.Select(i => i.ItemNumber);

        // Check for validity of moves.
        Moves = moves.Where(m => iNos.Contains(m.ItemNumber) && m.TakeCases == 0 && m.TakePacks == 0)
            .GroupBy(m => $"{m.TakeBinID}:{m.PlaceBinID}")
            .OrderByDescending(g => g.Count())
            .First()
            .ToList();

        if (Moves.Count < 1) return;

        SetBaseMoveValues();

        // Remove used moves from move list.
        foreach (var move in Moves)
        {
            moves.Remove(move);
        }

        SuccessfullyGenerated = true;
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

        TakeBin = Moves.First().TakeBin;
        PlaceBin = Moves.First().PlaceBin;
        TakeBinID = Moves.First().TakeBinID;
        PlaceBinID = Moves.First().PlaceBinID;
        BatchID = Moves.First().BatchID;
        Batch = Moves.First().Batch;
        AssignedOperator = Moves.First().AssignedOperator;
        AssignedOperatorID = Moves.First().AssignedOperatorID;

        IndividualPriority = Moves.Min(m => m.IndividualPriority);
        GroupPriority = Moves.Min(m => m.GroupPriority);
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
    /// Given a list of moves that may include mixed carton moves, attempt to generate a mixed carton movement.
    /// Will only generate one for the largest number of items if there are multiple.
    ///
    /// Will remove moves used in generated MC move, if any.
    /// </summary>
    /// <returns>Null if no MC Moves are available.</returns>
    public static MixedCartonMove? GenerateMixedCartonMove(ref List<Move> moves)
    {
        var move = new MixedCartonMove(ref moves);
        return move.SuccessfullyGenerated ? move : null;
    }

    /// <summary>
    /// Given a Mixed Carton frame and a list of moves that may include moves for that mixed carton, attempt to generate a mixed carton movement.
    /// Will only generate one for the largest number of correct items if there are multiple.
    ///
    /// Will remove moves used in generated MC move, if any.
    /// </summary>
    /// <returns>Null if no relevant MC Moves are available.</returns>
    public static MixedCartonMove? GenerateMixedCartonMove(MixedCarton mc, ref List<Move> moves)
    {
        var move = new MixedCartonMove(mc, ref moves);
        return move.SuccessfullyGenerated ? move : null;
    }


}