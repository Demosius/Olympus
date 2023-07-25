using System.Linq;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Panacea.Models;

public class BWMICheckResult
{
    public NAVBin Bin { get; set; }

    #region Direct Bin access

    public EZoneType ZoneType => Bin.Zone?.ZoneType ?? EZoneType.Overstock;
    public string Zone => Bin.ZoneCode;

    #endregion

    public string ItemString { get; set; }

    public BWMICheckResult(NAVBin bin)
    {
        Bin = bin;

        ItemString = string.Join("|", Bin.Stock.Keys.ToList());
    }
}