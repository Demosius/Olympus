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
        public static DataSet DataSet()
        {
            EquipmentChariot chariot = new EquipmentChariot(App.Settings.SolLocation);
            return chariot.PullFullDataSet();
        }

        /* Data Tables */
        private static DataTable TableByName(string tableName)
        {
            EquipmentChariot chariot = new EquipmentChariot(App.Settings.SolLocation);
            return chariot.PullFullTable(tableName);
        }

        public static DataTable DataTable(string tableName)
        {
            EquipmentChariot chariot = new EquipmentChariot(App.Settings.SolLocation);
            if (chariot.TableDefinitions.Keys.Contains(tableName) && !tableName.StartsWith("sqlite_"))
                return TableByName(tableName);
            return new DataTable();
        }

        public static DataTable BatteryTable()
        {
            return TableByName("battery");
        }

        public static DataTable BatteryLocationTable()
        {
            return TableByName("battery_location");
        }
        public static DataTable ChargerTable()
        {
            return TableByName("charger");
        }

        public static DataTable ChargerAssignmentTable()
        {
            return TableByName("charger_assignment");
        }

        internal static DataTable ChecklistTable()
        {
            return TableByName("checklist");
        }

        internal static DataTable CompledChecklistTable()
        {
            return TableByName("completed_checklist");
        }

        internal static DataTable FaultTable()
        {
            return TableByName("fault");
        }

        public static DataTable MachineTable()
        {
            return TableByName("machine");
        }

        internal static DataTable TypeTable()
        {
            return TableByName("type");
        }

        /* Machines */
        public static List<SimpleMachine> SimpleMachines()
        {
            EquipmentChariot chariot = new EquipmentChariot(App.Settings.SolLocation);
            return chariot.SimpleMachines();
        }

        public static List<Machine> Machines()
        {
            EquipmentChariot chariot = new EquipmentChariot(App.Settings.SolLocation);
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

        /* Charger */
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
