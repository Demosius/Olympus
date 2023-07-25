using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

/// <summary>
/// Tool for matching stock and/or moves (etc.) to mixed carton templates.
/// </summary>
public class MixedCartonIdentificationTool
{
    public List<MixedCarton> MixedCartons { get; set; }
    public List<MixedCarton> Templates => MixedCartons;

    public Dictionary<int, int> ItemMinimums { get; set; }  // <item number, minimum required>

    public MixedCartonIdentificationTool(List<MixedCarton> mixedCartons)
    {
        MixedCartons = mixedCartons;
        ItemMinimums = new Dictionary<int, int>();
        SetItemMinimums();
    }

    private void SetItemMinimums()
    {
        ItemMinimums = MixedCartons
            .SelectMany(t => t.Items)
            .GroupBy(t => t.ItemNumber)
            .ToDictionary(
                g => g.Key,
                g => g.Select(mci => mci.QtyPerCarton)
                    .Min());
    }

    public List<Stock> GetValidStock(List<Stock> stock, out List<Stock> invalidStock)
    {
        var validStock = new List<Stock>();
        invalidStock = new List<Stock>();

        foreach (var s in stock)
        {
            if (s is MixedCartonStock || !s.EachesOnly || !ItemMinimums.TryGetValue(s.ItemNumber, out var minValue) || s.BaseQty < minValue)
                invalidStock.Add(s);
            else
                validStock.Add(s);
        }

        return validStock;
    }

    /// <summary>
    /// Identify most likely template to match stock.
    /// </summary>
    /// <param name="stock"></param>
    /// <param name="createNewMixedCarton"></param>
    /// <returns>Null if no stock to match.</returns>
    public MixedCarton? GetMixedCartonFromStock(List<Stock> stock, bool createNewMixedCarton = false)
    {
        if (stock.Count <= 1) return null;

        var potential = Templates.Where(t => t.IsValidStock(stock)).ToList();

        if (!potential.Any())
        {
            if (!createNewMixedCarton) return null;

            var mc = MixedCarton.GetMixedCarton(stock);
            if (mc is not null) AddMixedCarton(mc);
            return mc;
        }
        if (potential.Count == 1) return potential.First();

        var potentModList = potential.Select(mc => new { mc, rem = mc.CalculateRemaining(stock) }).ToList();

        var min = potentModList.Min(t => t.rem);

        return potentModList.First(t => t.rem == min).mc;
    }

    private void AddMixedCarton(MixedCarton mc)
    {
        MixedCartons.Add(mc);
        SetItemMinimums();
    }
}