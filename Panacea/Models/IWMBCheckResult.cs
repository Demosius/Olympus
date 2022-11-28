using Uranus.Inventory.Models;

namespace Panacea.Models;

public class IWMBCheckResult
{
    public NAVItem Item { get; set; }

    public string ZoneType { get; set; }

    public string Zones { get; set; }

    public string Bins { get; set; }

    #region Direct Item access

    public int Number => Item.Number;
    public string Description => Item.Description;

    #endregion

    public IWMBCheckResult(NAVItem item, string zoneType)
    {
        Item = item;
        ZoneType = zoneType;

        Zones = string.Empty;
        Bins = string.Empty;
    }


}