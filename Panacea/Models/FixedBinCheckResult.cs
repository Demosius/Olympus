using System.Collections.Generic;
using System.Linq;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Panacea.Models;

public class FixedBinCheckResult
{
    public NAVItem Item { get; set; }

    public string FixedBins { get; set; }

    public List<SubStock> FixedSubStock { get; set; }

    public bool PassCheck { get; set; }

    #region direct item access

    public int Number => Item.Number;
    public string Description => Item.Description;
    public bool HasCases => Item.HasCases;
    public bool HasPacks => Item.HasPacks;

    #endregion

    public FixedBinCheckResult(NAVItem coreItem, ICollection<string> fixedZones)
    {
        Item = coreItem;
        FixedSubStock = new List<SubStock>();

        foreach (var stock in Item.StockDict.Values
                     .Where(stock => fixedZones.Contains(stock.Bin?.ZoneCode ?? "")))
        {
            if (stock.Cases?.Fixed ?? false) FixedSubStock.Add(stock.Cases);
            if (stock.Packs?.Fixed ?? false) FixedSubStock.Add(stock.Packs);
            if (stock.Eaches?.Fixed ?? false) FixedSubStock.Add(stock.Eaches);
        }

        FixedBins = string.Join(" || ", FixedSubStock);
    }

    /// <summary>
    /// Compare UoMs to expected checks against the fixed bin list, and changes the PassCheck result to appropriately show whether the item passes.
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void RunChecks(bool cases, bool packs, bool eaches, bool exclusiveEaches)
    {
        PassCheck = CompareUoMs(cases, packs, eaches, exclusiveEaches);
    }

    /// <summary>
    /// Compares the fixed bins to against the expected UoMs to determine check result.
    /// </summary>
    /// <param name="cases"></param>
    /// <param name="packs"></param>
    /// <param name="eaches"></param>
    /// <param name="exclusiveEaches"></param>
    /// <returns>bool: true if all checks are valid.</returns>
    private bool CompareUoMs(bool cases, bool packs, bool eaches, bool exclusiveEaches)
    {
        if (cases && Item.HasCases && FixedSubStock.All(s => s.UoM != EUoM.CASE)) return false;

        if (packs && Item.HasPacks && FixedSubStock.All(s => s.UoM != EUoM.PACK)) return false;

        if (eaches && FixedSubStock.All(s => s.UoM != EUoM.EACH)) return false;

        if (exclusiveEaches && !Item.HasCases && !Item.HasPacks && FixedSubStock.All(s => s.UoM != EUoM.EACH)) return false;

        return true;
    }
}