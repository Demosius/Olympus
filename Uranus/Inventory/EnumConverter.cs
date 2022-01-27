using System.Text.RegularExpressions;

namespace Uranus.Inventory
{
    public enum EUoM
    {
        Each,
        Pack,
        Case
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
        Low,
        Medium,
        High
    }


    public static class EnumConverter
    {

        /**************************** CONVERT Data ***************************/
        public static string UoMToString(EUoM uom)
        {
            if (uom == EUoM.Case)
                return "CASE";
            if (uom == EUoM.Pack)
                return "PACK";
            return "EACH";
        }

        public static EUoM StringToUoM(string uom)
        {
            uom = uom.ToUpper();
            if (uom == "CASE")
                return EUoM.Case;
            if (uom == "PACK")
                return EUoM.Pack;
            return EUoM.Each;
        }

        public static string ActionTypeToString(EAction actionType)
        {
            if (actionType == EAction.Take)
                return "Take";
            if (actionType == EAction.Place)
                return "Place";
            return null;
        }

        public static EAction StringToActionType(string actionType)
        {
            if (actionType.ToLower() == "take")
                return EAction.Take;
            return EAction.Place;
        }

        public static string MoveStatusToString(EStatus moveStatus)
        {
            if (moveStatus == EStatus.Open)
                return "Open";
            if (moveStatus == EStatus.Waiting)
                return "Waiting";
            if (moveStatus == EStatus.OnBoard)
                return "On Board";
            return "Complete";
        }

        private static readonly Regex SWhitespace = new(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return SWhitespace.Replace(input, replacement);
        }

        public static EStatus StringToMoveStatus(string moveStatus)
        {
            moveStatus = ReplaceWhitespace(moveStatus.ToLower(), "");
            if (moveStatus == "open")
                return EStatus.Open;
            if (moveStatus == "waiting")
                return EStatus.Waiting;
            if (moveStatus == "onboard")
                return EStatus.OnBoard;
            return EStatus.Complete;
        }

        public static string VolumeToString(EVolume volume)
        {
            if (volume == EVolume.Low)
                return "LV";
            if (volume == EVolume.Medium)
                return "MV";
            if (volume == EVolume.High)
                return "HV";
            return null;
        }

        public static EVolume StringToVolume(string volume)
        {
            return ReplaceWhitespace(volume.ToUpper(), "") switch
            {
                "LV" or "LOWVOLUME" or "LOW" => EVolume.Low,
                "MV" or "MEDIUMVOLUME" or "MEDIUM" => EVolume.Medium,
                "HV" or "HIGHVOLUME" or "HIGH" => EVolume.High,
                _ => EVolume.Low,
            };
        }
    }
}
