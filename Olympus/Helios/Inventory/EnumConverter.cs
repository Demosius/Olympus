using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory
{
    public enum EUoM
    {
        EACH,
        PACK,
        CASE
    }

    public enum ActionType
    {
        Take,
        Place
    }

    public enum MoveStatus
    {
        Open,       // Still open for grabbing.
        Waiting,    // Operator on the way to get it.
        OnBoard,    // On forklift, in cage, on pallet, etc.
        Complete    // Completed. Should be where it needs to be (behind primaries - not necessarily IN primary).
    }

    public static class EnumConverter
    {

        /**************************** CONVERT Data ***************************/
        public static string UoMToString(EUoM uom)
        {
            if (uom == EUoM.CASE)
                return "CASE";
            if (uom == EUoM.PACK)
                return "PACK";
            return "EACH";
        }

        public static EUoM StringToUoM(string uom)
        {
            uom = uom.ToUpper();
            if (uom == "CASE")
                return EUoM.CASE;
            if (uom == "PACK")
                return EUoM.PACK;
            return EUoM.EACH;
        }

        public static string ActionTypeToString(ActionType actionType)
        {
            if (actionType == ActionType.Take)
                return "Take";
            if (actionType == ActionType.Place)
                return "Place";
            return null;
        }

        public static ActionType StringToActionType(string actionType)
        {
            if (actionType.ToLower() == "take")
                return ActionType.Take;
            return ActionType.Place;
        }

        public static string MoveStatusToString(MoveStatus moveStatus)
        {
            if (moveStatus == MoveStatus.Open)
                return "Open";
            if (moveStatus == MoveStatus.Waiting)
                return "Waiting";
            if (moveStatus == MoveStatus.OnBoard)
                return "On Board";
            return "Complete";
        }

        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        public static MoveStatus StringToMoveStatus(string moveStatus)
        {
            moveStatus = ReplaceWhitespace(moveStatus.ToLower(), "");
            if (moveStatus == "open")
                return MoveStatus.Open;
            if (moveStatus == "waiting")
                return MoveStatus.Waiting;
            if (moveStatus == "onboard")
                return MoveStatus.OnBoard;
            return MoveStatus.Complete;
        }
    }
}
