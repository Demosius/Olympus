using Olympus.Uranus.Users.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Users
{
    public class UserDeleter
    {
        public UserChariot Chariot { get; set; }

        public UserDeleter(ref UserChariot chariot)
        {
            Chariot = chariot;
        }

        // Users
        public bool User(User user)
        {
            if (!App.Charon.CanDeleteUser(user.ID)) return false;
            return Chariot.Delete(user) && Chariot.DeleteByKey<Login>(user.ID);
        }
        public bool User(int id)
        {
            if (!App.Charon.CanDeleteUser(id)) return false;
            return Chariot.DeleteByKey<User>(id) && Chariot.DeleteByKey<Login>(id);
        }

        // Logins
        public bool Login(Login login)
        {
            if (!App.Charon.CanDeleteUser(login.UserID)) return false;
            return Chariot.Delete(login) && Chariot.DeleteByKey<User>(login.UserID);
        }
        public bool Login(int userID)
        {
            if (!App.Charon.CanDeleteUser(userID)) return false;
            return Chariot.DeleteByKey<User>(userID) && Chariot.DeleteByKey<Login>(userID);
        }

        // Roles
        public bool Role(Role role)
        {
            if (!App.Charon.CanDeleteUserRole()) return false;
            // Can't delete roles that have users attached.
            List<User> users = Chariot.PullObjectList<User>(PullType.ObjectOnly).Where(u => u.RoleName == role.Name).ToList();
            if (users.Count > 0)
                return false;
            return Chariot.Delete(role);
        }

        public bool Role(string roleName)
        {
            if (!App.Charon.CanDeleteUserRole()) return false;
            // Can't delete roles that have users attached.
            List<User> users = Chariot.PullObjectList<User>(PullType.ObjectOnly).Where(u => u.RoleName == roleName).ToList();
            if (users.Count > 0)
                return false;
            return Chariot.DeleteByKey<Role>(roleName);
        }




    }
}
