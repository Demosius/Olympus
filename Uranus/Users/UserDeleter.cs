using System.Threading.Tasks;
using Uranus.Users.Models;

namespace Uranus.Users;

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
        return Chariot.Delete(user) > 0 && Chariot.DeleteByKey<Login>(user.ID);
    }

    public bool User(int id)
    {
        return Chariot.DeleteByKey<User>(id) && Chariot.DeleteByKey<Login>(id);
    }

    // Logins
    public bool Login(Login login)
    {
        return Chariot.Delete(login) > 0 && Chariot.DeleteByKey<User>(login.UserID);
    }
    public bool Login(int userID)
    {
        return Chariot.DeleteByKey<User>(userID) && Chariot.DeleteByKey<Login>(userID);
    }

    // Roles
    public async Task<bool> RoleAsync(Role role) => await RoleAsync(role.Name);

    public async Task<bool> RoleAsync(string roleName)
    {
        var result = false;
        void Action()
        {
            // Can't delete roles that have users attached.
            var userCount =
                Chariot.ExecuteScalar<int>("SELECT COUNT(*) FROM Employee WHERE RoleName = ?;", roleName);
            result = userCount <= 0 && Chariot.DeleteByKey<Role>(roleName);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action));

        return result;
    }
}