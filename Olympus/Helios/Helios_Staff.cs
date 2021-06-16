using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Olympus.Helios.Staff;

namespace Olympus.Helios
{
    public class GetStaff
    {

        public static DataSet DataSet()
        {
            StaffChariot chariot = new StaffChariot(Toolbox.GetSol());
            return chariot.PullFullDataSet();
        }

        /* Data Tables */
        private static DataTable TableByName(string tableName)
        {
            StaffChariot chariot = new StaffChariot(Toolbox.GetSol());
            return chariot.PullFullTable(tableName);
        }

        public static DataTable DataTable(string tableName)
        {
            StaffChariot chariot = new StaffChariot(Toolbox.GetSol());
            if (chariot.TableDefinitions.Keys.Contains(tableName) && !tableName.StartsWith("sqlite_"))
                return TableByName(tableName);
            return new DataTable();
        }

        internal static DataTable BorrowEmpTable()
        {
            return TableByName("borrowsEmp");
        }

        internal static DataTable ClanTable()
        {
            return TableByName("clan");
        }

        internal static DataTable DepartmentTable()
        {
            return TableByName("department");
        }

        internal static DataTable EmployeeTable()
        {
            return TableByName("employee");
        }

        internal static DataTable InductionTable()
        {
            return TableByName("induction");
        }

        internal static DataTable LicenceTable()
        {
            return TableByName("licence");
        }

        internal static DataTable InductionReferenceTable()
        {
            return TableByName("inductionReference");
        }

        internal static DataTable OwnsCarTable()
        {
            return TableByName("ownsCar");
        }

        internal static DataTable RoleTable()
        {
            return TableByName("role");
        }

        internal static DataTable LockerTable()
        {
            return TableByName("locker");
        }

        internal static DataTable ShiftTable()
        {
            return TableByName("shift");
        }

        internal static DataTable ShiftRuleTable()
        {
            return TableByName("shift_rule");
        }

        internal static DataTable ShiftEligibilityTable()
        {
            return TableByName("shiftEligibility");
        }

        internal static DataTable TempTagTable()
        {
            return TableByName("temp_tag");
        }

        internal static DataTable UseTagTable()
        {
            return TableByName("usesTag");
        }

        internal static DataTable VehicleTable()
        {
            return TableByName("vehicle");
        }
    }

    public class PutStaff
    {

    }

    public class PostStaff
    {

    }

    public class DeleteStaff
    {

    }
}
