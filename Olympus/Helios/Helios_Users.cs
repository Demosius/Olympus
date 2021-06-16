using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Olympus.Helios.Users;

namespace Olympus.Helios
{
    public static class GetUsers
    {
        public static DataSet DataSet()
        {
            UserChariot chariot = new UserChariot(Toolbox.GetSol());
            return chariot.PullFullDataSet();
        }

        /* Data Tables */
        private static DataTable TableByName(string tableName)
        {
            UserChariot chariot = new UserChariot(Toolbox.GetSol());
            return chariot.PullFullTable(tableName);
        }

        public static DataTable DataTable(string tableName)
        {
            UserChariot chariot = new UserChariot(Toolbox.GetSol());
            if (chariot.TableDefinitions.Keys.Contains(tableName) && !tableName.StartsWith("sqlite_"))
                return TableByName(tableName);
            return new DataTable();
        }

        internal static DataTable LoginTable()
        {
            return TableByName("login");
        }

        internal static DataTable RoleTable()
        {
            return TableByName("role");
        }

        internal static DataTable UserTable()
        {
            return TableByName("user");
        }
    }

    public static class PutUsers
    {
        public static bool AddUser(int userID, string password)
        {
            Console.WriteLine(userID + password);
            return false;
        }
    }

    public static class PostUsers
    {

    }

    public static class DeleteUsers
    {

    }
}
