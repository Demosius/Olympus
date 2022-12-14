using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Uranus.Users.Models;

namespace Uranus.Users;

public class UserReader
{
    public UserChariot Chariot { get; set; }

    public UserReader(ref UserChariot chariot)
    {
        Chariot = chariot;
    }

    public bool UserExists(int userID) => Chariot.Database?.ExecuteScalar<int>("SELECT count(*) FROM User WHERE ID=?;", userID) > 0;

    public Login? Login(int userID) => Chariot.PullObject<Login>(userID);

    public User? User(int userID) => Chariot.PullObject<User>(userID, EPullType.FullRecursive);

    public Role? Role(string roleName) => Chariot.PullObject<Role>(roleName, EPullType.FullRecursive);

    public int UserCount() => Chariot.PullObjectList<User>(pullType: EPullType.ObjectOnly).Count; //Chariot.Database.Execute("SELECT count(*) FROM User;");

    public IEnumerable<User> Users(Expression<Func<User, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public IEnumerable<Role> Roles(Expression<Func<Role, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

}