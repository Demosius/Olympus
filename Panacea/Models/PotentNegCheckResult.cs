using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranus.Inventory.Models;

namespace Panacea.Models;

public class PotentNegCheckResult
{
    public Stock Stock { get; set; }

    #region Direct Stock access

    public string Zone => Stock.Zone?.Code ?? Stock.ZoneID;
    public string Bin => Stock.Bin?.Code ?? Stock.BinID;
    public int Item => Stock.ItemNumber;
    public string Description => Stock.Item?.Description ?? string.Empty;
    public int PickQty => Stock.BasePickQty;
    public int ReplenQty => Stock.BasePutAwayQty;   // TODO: Replace with reference to active Replen documents when available from Vulcan.

    #endregion

    public PotentNegCheckResult(Stock stock)
    {
        Stock = stock;
    }
}