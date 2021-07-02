using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Equipment
{
    public class EquipmentChariot : MasterChariot
    {
        public EquipmentChariot()
        {
            FilePath = Path.Combine(Environment.CurrentDirectory, "Sol", "Equipment", "Equipment.sqlite");
            Connect();
        }

        public EquipmentChariot(string solLocation)
        {
            FilePath = Path.Combine(solLocation, "Equipment", "Equipment.sqlite");
            Connect();
        }

        /******************************* Get Data *****************************/
        /* Batteries */

        /* Chargers */

        /* Checklists */

        /* Machine */
        public List<SimpleMachine> SimpleMachines()
        {
            try
            {
                List<SimpleMachine> list = new List<SimpleMachine> { };
                Conn.Open();
                string query = "SELECT * FROM [bin];";
                SQLiteCommand command = new SQLiteCommand(query, Conn);
                SQLiteDataReader reader = command.ExecuteReader();
                string mc;

                while (reader.Read())
                {
                    mc = reader["max_cube"].ToString();
                    list.Add
                    (
                        new SimpleMachine
                        (
                            serialNumber: reader["serial_no"].ToString(),
                            typeCode: reader["type_code"].ToString(),
                            serviceDueDate: DateTime.Parse(reader["service_due_date"].ToString()),
                            lastServiceDate: DateTime.Parse(reader["last_service_date"].ToString()),
                            checklistName: reader["checklist_name"].ToString(),
                            ownership: reader["ownership"].ToString(),
                            lastPreopCheck: DateTime.Parse(reader["last_preop_check"].ToString())
                        )
                    );
                }

                Conn.Close();
                return list;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return new List<SimpleMachine> { };
            }
        }

        public DataTable GetMachinesTypeChecklist()
        {
            string query = 
                $@"
                    SELECT machine.*, type.*, cheklist.* FROM machine 
                    LEFT JOIN type ON machine.type_code = type.code
                    LEFT JOIN checklist ON machine.checklist_name = checklist.name
                ;";
            return PullTableWithQuery(query);
        }

        /******************************* Put Data *****************************/

        /****************************** Post Data *****************************/

        /***************************** Delete Data ****************************/

        /****************************Table Definitions*************************/
        private static readonly string BatteryDefinition =
            @"create table battery
            (
                serial_no text not null
                    constraint battery_pk
                        primary key
            );

            create unique index battery_serial_uindex
                on battery (serial_no);";

        private static readonly string BatteryLocationDefinition =
            @"create table battery_location
            (
                battery_serial_no text not null
                    constraint battery_location_pk
                        primary key
                    references battery
                        on update cascade on delete cascade,
                machine_serial_no text
                                       references machine
                                           on update cascade on delete set null
            );

            create unique index battery_location_battery_serial_no_uindex
                on battery_location (battery_serial_no);

            create unique index battery_location_machine_serial_no_uindex
                on battery_location (machine_serial_no);";

        private static readonly string ChargerDefinition =
            @"create table charger
            (
                serial_no text not null
                    constraint charger_pk
                        primary key
            );

            create unique index charger_serial_no_uindex
                on charger (serial_no);";

        private static readonly string ChargerAssignmentDefinition =
            @"create table charger_assignment
            (
                charger_serial_no text not null
                    constraint charger_assignment_pk
                        primary key
                    references charger
                        on update cascade on delete cascade,
                machine_serial_no text
                                       references machine
                                           on update cascade on delete set null
            );

            create unique index charger_assignment_charger_serial_no_uindex
                on charger_assignment (charger_serial_no);

            create unique index charger_assignment_machine_serial_no_uindex
                on charger_assignment (machine_serial_no);";

        private static readonly string ChecklistDefinition =
            @"create table checklist
            (
                name      text not null
                    constraint checklist_pk
                        primary key,
                type_code text not null
                    references type
                        on update cascade on delete restrict,
                checklist text not null
            );

            create unique index checklist_name_uindex
                on checklist (name);";

        private static readonly string CompletedChecklistDefinition =
            @"-- auto-generated definition
            create table completed_checklist
            (
                id                integer not null
                    constraint completed_checklist_pk
                        primary key autoincrement,
                machine_serial_no text    not null
                    references machine
                        on update cascade,
                timestamp         text    not null,
                fault_found       boolean default FALSE not null,
                pass              boolean default TRUE not null,
                employee_number   int     not null,
                checklist         text    not null
            );

            create unique index completed_checklist_id_uindex
                on completed_checklist (id);";

        private static readonly string FaultDefinition =
            @"-- auto-generated definition
            create table fault
            (
                id                     integer not null
                    constraint fault_pk
                        primary key autoincrement,
                fault                  text    not null,
                cause_failure          boolean default FALSE not null,
                completed_checklist_id integer not null
                    references completed_checklist
                        on update cascade on delete cascade
            );

            create unique index fault_id_uindex
                on fault (id);
";

        private static readonly string MachineDefinition =
            @"-- auto-generated definition
            create table machine
            (
                serial_no         text not null
                    constraint machine_pk
                        primary key,
                type_code         text not null
                    references type
                        on update cascade on delete restrict,
                service_due_date  text not null,
                last_service_date text,
                checklist_name    text not null
                    references checklist
                        on update cascade on delete restrict,
                ownership         text default 'owned' not null,
                last_preop_check  text default null
            );

            create unique index machine_serial_no_uindex
                on machine (serial_no);
";

        private static readonly string TypeDefinition =
            @"create table type
            (
                code         text not null
                    constraint type_pk
                        primary key,
                licence_code text
            );

            create unique index type_code_uindex
                on type (code);";

        public override Dictionary<string, string> TableDefinitions
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "battery", BatteryDefinition },
                    { "battery_location", BatteryLocationDefinition },
                    { "charger", ChargerDefinition },
                    { "charger_assignment", ChargerAssignmentDefinition },
                    { "checklist", ChecklistDefinition },
                    { "completed_checklist", CompletedChecklistDefinition },
                    { "fault", FaultDefinition },
                    { "machine", MachineDefinition },
                    { "type", TypeDefinition }
                };
            }
        }
    }
}
