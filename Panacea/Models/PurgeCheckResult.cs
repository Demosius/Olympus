using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Panacea.Models;

public class PurgeCheckResult
{
    public Stock Stock { get; set; }

    #region Direct Stock access.

    public EZoneType ZoneType => Stock.Zone?.ZoneType ?? EZoneType.Overstock;
    public string Zone => Stock.Zone?.Code ?? Stock.ZoneID;
    public int Item => Stock.ItemNumber;
    public string Bin => Stock.Bin?.Code ?? Stock.BinID;
    public string Description => Stock.Item?.Description ?? "";
    public string UoM => Stock.GetUoMString();
    public int BaseQty => Stock.BaseQty;
    public bool NonCommitted => Stock.NonCommitted;

    #endregion

    public PurgeCheckResult(Stock stock)
    {
        Stock = stock;
    }
}