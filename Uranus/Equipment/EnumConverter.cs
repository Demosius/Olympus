using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Equipment
{
    /*public enum EMachineType
    {
        OS,         // Rabbit
        LF,         // Standard forlkift
        HRLF,       // High reach forklift
        LFSit,      // Standard sit down forklift
        HRLFSit,    // High Reach sit down forklift
        LO,         //
        HRLO
    }

    public

    public static class EnumConverter
    {

        *//**************************** CONVERT Data ***************************//*
        public static string MachTypeDescritpion(EMachineType machineType)
        {
            if (machineType == EMachineType.Rabbit)
                return "Rabbit";
            if (machineType == EMachineType.Forklift)
                return "Forklift";
            if (machineType == EMachineType.HighReachForkLift)
                return "High ReachFork Lift";
            if (machineType == EMachineType.SitDownForlift)
                return "Sit-down Forlift";
            if (machineType == EMachineType.HighReachSitDownForklift)
                return "High Reach Sit-down Forklift";
            if (machineType == EMachineType.StockPicker)
                return "StockPicker";
            if (machineType == EMachineType.HighReachStockPicker)
                return "High Reach StockPicker";
            return "None";
        }

        public static EMachineType StringToMachType(string machineType)
        {
            machineType = machineType.ToUpper();
            if (machineType == "RABBIT")
                return EMachineType.Rabbit;
            if (machineType == "FORKLIFT")
                return EMachineType.Forklift;
            if (machineType == "HIGHREACHFORKLIFT")
                return EMachineType.HighReachForkLift;
            if (machineType == "SITDOWNFORKLIFT")
                return EMachineType.SitDownForlift;
            if (machineType == "HIGHREACHSITDOWNFORKLIFT")
                return EMachineType.HighReachSitDownForklift;
            if (machineType == "STOCKPICKER")
                return EMachineType.StockPicker;
            if (machineType == "HIGHREACHSTOCKPICKER")
                return EMachineType.HighReachStockPicker;
            return EMachineType.Rabbit;
        }

    }*/
}
