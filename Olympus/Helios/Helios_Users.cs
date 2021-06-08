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
