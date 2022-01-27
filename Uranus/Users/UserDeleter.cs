using Uranus.Users.Model;
using System.Linq;

namespace Uranus.Users
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
            return Chariot.Delete(user) && Chariot.DeleteByKey<Login>(user.ID);
        }
        public bool User(int id)
        {
            return Chariot.DeleteByKey<User>(id) && Chariot.DeleteByKey<Login>(id);
        }

        // Logins
        public bool Login(Login login)
        {
            return Chariot.Delete(login) && Chariot.DeleteByKey<User>(login.UserID);
        }
        public bool Login(int userID)
        {
            return Chariot.DeleteByKey<User>(userID) && Chariot.DeleteByKey<Login>(userID);
        }

        // Roles
        public bool Role(Role role)
        {
            // Can't delete roles that have users attached.
            var users = Chariot.PullObjectList<User>(pullType: EPullType.ObjectOnly).Where(u => u.RoleName == role.Name).ToList();
            if (users.Count > 0)
                return false;
            return Chariot.Delete(role);
        }

        public bool Role(string roleName)
        {
            // Can't delete roles that have users attached.
            var users = Chariot.PullObjectList<User>(pullType: EPullType.ObjectOnly).Where(u => u.RoleName == roleName).ToList();
            if (users.Count > 0)
                return false;
            return Chariot.DeleteByKey<Role>(roleName);
        }




    }
}
