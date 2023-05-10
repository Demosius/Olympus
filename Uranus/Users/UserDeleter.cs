﻿using System.Linq;
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
    public async Task<bool> RoleAsync(Role role)
    {
        // Can't delete roles that have users attached.
        var users = (await Chariot.PullObjectListAsync<User>(pullType: EPullType.ObjectOnly))
            .Where(u => u.RoleName == role.Name).ToList();
        return users.Count <= 0 && Chariot.Delete(role);
    }

    public async Task<bool> RoleAsync(string roleName)
    {
        // Can't delete roles that have users attached.
        var users = (await Chariot.PullObjectListAsync<User>(pullType: EPullType.ObjectOnly))
            .Where(u => u.RoleName == roleName).ToList();
        return users.Count <= 0 && Chariot.DeleteByKey<Role>(roleName);
    }
}