using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Staff
{
    public class StaffChariot : MasterChariot
    {
        public StaffChariot()
        {
            FilePath = "./Sol/Staff/Staff.sqlite";
            Connect();
        }

        public StaffChariot(string solLocation)
        {
            FilePath = solLocation + "/Staff/Staff.sqlite";
            Connect();
        }

        /******************************* Get Data *****************************/
        /* Employees */
        public DataTable GetEmployeeTable()
        {
            return PullFullTable("employee");
        }

        public DataTable GetBorrowEmpTable()
        {
            return PullFullTable("borrowEmp");
        }

        public DataTable GetRoleTable()
        {
            return PullFullTable("role");
        }

        public DataTable GetLicenceTable()
        {
            return PullFullTable("licence");
        }

        /* Departments */
        public DataTable GetDepartmentTable()
        {
            return PullFullTable("department");
        }

        /* Clans */
        public DataTable GetClanTable()
        {
            return PullFullTable("clan");
        }

        /* Shifts */
        public DataTable GetShiftTable()
        {
            return PullFullTable("shift");
        }

        public DataTable GetShiftRuleTable()
        {
            return PullFullTable("shift_rule");
        }

        public DataTable GetShiftEligibilityTable()
        {
            return PullFullTable("shiftEligibility");
        }

        /* Tags */
        public DataTable GetTempTagTable()
        {
            return PullFullTable("temp_tag");
        }

        public DataTable GetUsesTagTable()
        {
            return PullFullTable("usesTag");
        }

        /* Vehicles */
        public DataTable GetVehicleTable()
        {
            return PullFullTable("vehicle");
        }

        public DataTable GetOwnsCarTable()
        {
            return PullFullTable("ownsCar");
        }

        /* Inductions */
        public DataTable GetInductionTable()
        {
            return PullFullTable("induction");
        }

        public DataTable GetInductionReferenceTable()
        {
            return PullFullTable("inductionReference");
        }

        /* Lockers */
        public DataTable GetLockerTable()
        {
            return PullFullTable("locker");
        }

        /******************************* Put Data *****************************/
        /****************************** Post Data *****************************/
        /***************************** Delete Data ****************************/

        /***************************Table Definitions**************************/
        private static readonly string BorrowsEmpDefinition =
            @"create table borrowsEmp
            (
                employee_number int  not null
                    references employee
                        on update cascade on delete cascade,
                department_name text not null
                    references department
                        on update cascade on delete cascade,
                constraint emp_dept_borrowing_pk
                    primary key (employee_number, department_name)
            );";

        private static readonly string ClanDefinition =
            @"create table clan
            (
                name            text not null
                    constraint clan_pk
                        primary key,
                department_name text
                    references department
                        on update cascade on delete cascade,
                leader          int
                                     references employee
                                         on update cascade on delete set null
            );

            create unique index clan_name_uindex
                on clan (name);";

        private static readonly string DepartmentDefinition =
            @"create table department
            (
                name text not null
                    constraint department_pk
                        primary key,
                head int
                            references employee
                                on update cascade on delete set null
            );

            create unique index department_name_uindex
                on department (name);";

        private static readonly string EmployeeDefinition =
            @"create table employee
            (
                number          int  not null
                    constraint employee_pk
                        primary key,
                first_name      text not null,
                last_name       text not null,
                display_name    text not null,
                pay_rate        real default null,
                rf_id           text,
                id              text,
                department_name int  default null
                                     references department
                                         on update cascade on delete set null,
                role_name       text not null
                    references role
                        on update cascade on delete restrict,
                locker_id       int  default null
                                     references locker
                                         on update cascade on delete set null,
                phone           text default null,
                email           text default null,
                address         text default null
            );

            create unique index employee_display_name_uindex
                on employee (display_name);

            create unique index employee_id_uindex
                on employee (id);

            create unique index employee_locker_id_uindex
                on employee (locker_id);

            create unique index employee_number_uindex
                on employee (number);

            create unique index employee_rf_id_uindex
                on employee (rf_id);";

        private static readonly string InductionDefinition =
            @"create table induction
            (
                type        text not null
                    constraint induction_pk
                        primary key,
                description text,
                period      text
            );

            create unique index induction_type_uindex
                on induction (type);";

        private static readonly string InductionReferenceDefinition =
            @"create table inductionReference
            (
                employee_number int  not null
                    references employee
                        on update cascade on delete cascade,
                induction_type  text not null
                    references induction
                        on update cascade on delete cascade,
                issue_date      text not null,
                expiry_date     text,
                constraint induction_pk
                    primary key (employee_number, induction_type)
            );";

        private static readonly string LicenceDefinition =
            @"create table licence
            (
                number          text not null
                    constraint licence_pk
                        primary key,
                issue_date      text not null,
                expiry_date     text not null,
                LF              int default false not null,
                LO              int default false not null,
                WP              int default false not null,
                front_image     BLOB,
                back_image      BLOB,
                employee_number int  not null
                    references employee
                        on update cascade on delete set null
            );

            create unique index licence_number_uindex
                on licence (number);";

        private static readonly string LockerDefinition =
            @"create table locker
            (
                id              int not null
                    constraint locker_pk
                        primary key,
                location        text,
                employee_number int default null
                                    references employee
                                        on update cascade on delete set null
            );

            create unique index locker_employee_number_uindex
                on locker (employee_number);

            create unique index locker_id_uindex
                on locker (id);";

        private static readonly string OwnsCarDefinition =
            @"create table ownsCar
            (
                employee_number int  not null
                    references employee
                        on update cascade on delete cascade,
                vehicle_rego    text not null
                    references vehicle
                        on update cascade on delete cascade,
                constraint employee_vehicle_pk
                    primary key (employee_number, vehicle_rego)
            );";

        private static readonly string RoleDefinition =
            @"create table role
            (
                name            text not null
                    constraint role_pk
                        primary key,
                department_name text
                    references department
                        on update cascade on delete restrict,
                level           int default 0 not null,
                reports_to      text
                    references role
                        on update cascade on delete restrict
            );

            create unique index role_name_uindex
                on role (name);";

        private static readonly string ShiftDefinition =
            @"create table shift
            (
                name            text not null,
                department_name text not null
                    references department
                        on update cascade on delete cascade,
                start_time      text not null,
                end_time        text not null,
                breaks          text default '{""breaks"":[{""name"":""smoko"",""start"":""07:30"",""length"": 20},{""name"":""lunch"",""start"": ""12:15"",""length"":40},{""name"":""welfare"",""start"":""14:30"",""length"":10}]}' not null,
                constraint shift_pk
                    primary key(department_name, name)
            );";
        
        private static readonly string ShiftRuleDefinition =
            @"create table shift_rule
            (
                id              integer not null
                    constraint empShiftRule_pk
                        primary key autoincrement,
                employee_number int     not null
                    references employee
                        on update cascade on delete cascade,
                rule            text    not null
            );

            create unique index empShiftRule_id_uindex
                on shift_rule (id);
";
       
        private static readonly string ShiftEligibilityDefinition =
            @"create table shiftEligibility
            (
                employee_number int not null
                    references employee
                        on update cascade on delete cascade,
                shift_name      text
                    constraint shiftEligibility_shift_name_fk
                        references shift (name)
                        on update cascade on delete cascade,
                constraint shiftEligibility_pk
                    primary key (employee_number, shift_name)
            );";
        
        private static readonly string TempTagDefinition =
            @"create table temp_tag
            (
                rf_id          text not null
                    constraint temp_tag_pk
                        primary key,
                employee_rf_id text
            );

            create unique index temp_tag_employee_rf_id_uindex
                on temp_tag (employee_rf_id);

            create unique index temp_tag_rf_id_uindex
                on temp_tag (rf_id);";
        
        private static readonly string UsesTagDefinition =
            @"create table usesTag
            (
                id             integer not null
                    constraint usesTag_pk
                        primary key autoincrement,
                employee_rf_id text    not null
                    constraint usesTag_employee_rf_id_fk
                        references employee (rf_id)
                        on update cascade on delete cascade,
                temp_tag_rf_id text    not null
                    references temp_tag
                        on update cascade on delete cascade,
                start_date     text default '1900-01-01 00:00' not null,
                end_date       text
            );

            create unique index usesTag_id_uindex
                on usesTag (id);";
        
        private static readonly string VehicleDefinition =
            @"create table vehicle
            (
                rego        text not null
                    constraint vehicle_pk
                        primary key,
                colour      text not null,
                make        text not null,
                model       text not null,
                description text default null not null
            );

            create unique index vehicle_rego_uindex
                on vehicle (rego);";

        public override Dictionary<string, string> TableDefinitions
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "borrowsEmp", BorrowsEmpDefinition },
                    { "clan", ClanDefinition },
                    { "department", DepartmentDefinition },
                    { "employee", EmployeeDefinition },
                    { "induction", InductionDefinition },
                    { "inductionReference", InductionReferenceDefinition },
                    { "licence", LicenceDefinition },
                    { "locker", LockerDefinition },
                    { "ownsCar", OwnsCarDefinition },
                    { "role", RoleDefinition },
                    { "shift", ShiftDefinition },
                    { "shift_rule", ShiftRuleDefinition },
                    { "shiftEligibility", ShiftEligibilityDefinition },
                    { "temp_tag", TempTagDefinition },
                    { "usesTag", UsesTagDefinition },
                    { "vehicle", VehicleDefinition }
                };
            }
        }
    }
}
