using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Olympus.Helios.Equipment;

namespace Olympus.Helios
{
    public static class GetEquipment
    {
        /* Machines */
        public static DataTable GetMachineTable()
        {
            EquipmentChariot chariot = new EquipmentChariot(Toolbox.GetSol());
            return chariot.GetMachineTable();
        }

        public static List<Machine> GetMachines()
        {
            EquipmentChariot chariot = new EquipmentChariot(Toolbox.GetSol());
            DataTable data = chariot.GetMachinesTypeChecklist();
            List<Machine> machines = data.AsEnumerable().Select
                (row =>
                {
                    int machineSerial = (int)row["serial_no"];
                    string typeCode = row["type_code"].ToString();
                    DateTime serviceDueDate = DateTime.Parse(row["service_due_date"].ToString());
                    DateTime lastServiceDate = DateTime.Parse(row["last_service_date"].ToString());
                    Checklist checklist = new Checklist
                    (
                        row["checklist_name"].ToString() ?? "",
                        row["type_code"].ToString() ?? "",
                        row["checklist"].ToString() ?? ""
                    );
                    string ownership = row["ownership"].ToString();
                    DateTime lastPreop = DateTime.Parse(row["last_preop_check"].ToString());
                    string licenceCode = row["licence_code"].ToString();
                    switch (licenceCode)
                    {
                        case null:
                            return new Rabbit(machineSerial, typeCode, serviceDueDate, lastServiceDate, checklist, ownership, licenceCode, lastPreop);
                        case "LF":
                            return new Forklift(machineSerial, typeCode, serviceDueDate, lastServiceDate, checklist, ownership, licenceCode, lastPreop);
                        case "LO":
                            return new Stockpicker(machineSerial, typeCode, serviceDueDate, lastServiceDate, checklist, ownership, licenceCode, lastPreop);
                        default:
                            return new Machine(machineSerial, typeCode, serviceDueDate, lastServiceDate, checklist, ownership, licenceCode, lastPreop);
                    }
                }
                ).ToList();
            return machines;
        }

        /* Batteries */
        public static DataTable GetBatteryTable()
        {
            EquipmentChariot chariot = new EquipmentChariot(Toolbox.GetSol());
            return chariot.GetBatteryTable();
        }

        public static DataTable GetBatteryLocationTable()
        {
            EquipmentChariot chariot = new EquipmentChariot(Toolbox.GetSol());
            return chariot.GetBatteryLocationTable();
        }

        /* Charger */
        public static DataTable GetChargerTable()
        {
            EquipmentChariot chariot = new EquipmentChariot(Toolbox.GetSol());
            return chariot.GetChargerTable();
        }

        public static DataTable GetChargerAssignmentTable()
        {
            EquipmentChariot chariot = new EquipmentChariot();
            return chariot.GetChargerAssignmentTable();
        }
    }

    public static class PutEquipment
    {

    }

    public static class PostEquipment
    {

    }

    public static class DeleteEquipment
    {

    }
}
