﻿using System.Text.RegularExpressions;

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
            return uom switch
            {
                EUoM.Case => "CASE",
                EUoM.Pack => "PACK",
                _ => "EACH"
            };
        }

        public static EUoM StringToUoM(string uom)
        {
            uom = uom.ToUpper();
            return uom switch
            {
                "CASE" => EUoM.Case,
                "PACK" => EUoM.Pack,
                _ => EUoM.Each
            };
        }

        public static string ActionTypeToString(EAction actionType)
        {
            return actionType switch
            {
                EAction.Take => "Take",
                EAction.Place => "Place",
                _ => null
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

        private static readonly Regex SWhitespace = new(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return SWhitespace.Replace(input, replacement);
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
                _ => null
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
}