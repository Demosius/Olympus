using Olympus.Helios.Users.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Users
{
    public class UserReader
    {
        public UserChariot Chariot { get; set; }

        public UserReader(ref UserChariot chariot)
        {
            Chariot = chariot;
        }

        public bool UserExists(int userID) => Chariot.Database.Execute("SELECT count(*) FROM User WHERE ID=?;", userID) > 0;
        
        public Login Login(int userID) => Chariot.PullObject<Login>(userID);
        
        public User User(int userID) => Chariot.PullObject<User>(userID, PullType.FullRecursive);

        public Role Role(string roleName) => Chariot.PullObject<Role>(roleName, PullType.FullRecursive);

        public int UserCount() => Chariot.PullObjectList<User>(PullType.ObjectOnly).Count; //Chariot.Database.Execute("SELECT count(*) FROM User;");

    }
}
