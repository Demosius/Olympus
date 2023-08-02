using System.Text.RegularExpressions;
// ReSharper disable StringLiteralTypo

namespace Uranus.Inventory;

public enum EUoM
{
    EACH,
    PACK,
    CASE
}

public enum EAction
{
    Take,
    Place
}

public enum EStatus
{
    Open,       // Still open for grabbing.
    Waiting,    // Operator on the way to get it.
    OnBoard,    // On forklift, in cage, on pallet, etc.
    Complete    // Completed. Should be where it needs to be (behind primaries - not necessarily IN primary).
}

public enum EAccessLevel
{
    Ground,
    PalletRack,
    HighReach
}

public enum EVolume
{
    Unknown,
    Low,
    Medium,
    High
}

public enum EZoneType
{
    Overstock,
    Pick,
    HoldZone,
    Shipping,
    QualityControl
}

public static class EnumConverter
{

    /**************************** CONVERT Data ***************************/
    public static string UoMToString(EUoM uom)
    {
        return uom switch
        {
            EUoM.CASE => "CASE",
            EUoM.PACK => "PACK",
            _ => "EACH"
        };
    }

    public static EUoM StringToUoM(string uom)
    {
        uom = uom.ToUpper();
        return uom switch
        {
            "CASE" => EUoM.CASE,
            "PACK" => EUoM.PACK,
            _ => EUoM.EACH
        };
    }

    public static string ActionTypeToString(EAction actionType)
    {
        return actionType switch
        {
            EAction.Take => "Take",
            EAction.Place => "Place",
            _ => string.Empty
        };
    }

    public static EAction StringToActionType(string actionType)
    {
        return actionType.ToLower() == "take" ? EAction.Take : EAction.Place;
    }

    public static string MoveStatusToString(EStatus moveStatus)
    {
        return moveStatus switch
        {
            EStatus.Open => "Open",
            EStatus.Waiting => "Waiting",
            EStatus.OnBoard => "On Board",
            _ => "Complete"
        };
    }

    private static readonly Regex sWhitespace = new(@"\s+");
    public static string ReplaceWhitespace(string input, string replacement)
    {
        return sWhitespace.Replace(input, replacement);
    }

    public static EStatus StringToMoveStatus(string moveStatus)
    {
        moveStatus = ReplaceWhitespace(moveStatus.ToLower(), "");
        return moveStatus switch
        {
            "open" => EStatus.Open,
            "waiting" => EStatus.Waiting,
            @"onboard" => EStatus.OnBoard,
            _ => EStatus.Complete
        };
    }

    public static string VolumeToString(EVolume volume)
    {
        return volume switch
        {
            EVolume.Low => "LV",
            EVolume.Medium => "MV",
            EVolume.High => "HV",
            _ => string.Empty
        };
    }

    public static EVolume StringToVolume(string volume)
    {
        return ReplaceWhitespace(volume.ToUpper(), "") switch
        {
            "LV" or @"LOWVOLUME" or "LOW" => EVolume.Low,
            "MV" or @"MEDIUMVOLUME" or "MEDIUM" => EVolume.Medium,
            "HV" or @"HIGHVOLUME" or "HIGH" => EVolume.High,
            _ => EVolume.Low,
        };
    }
}