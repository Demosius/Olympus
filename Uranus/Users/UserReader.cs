using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Uranus.Users.Models;

namespace Uranus.Users;

public class UserReader
{
    public UserChariot Chariot { get; set; }

    public UserReader(ref UserChariot chariot)
    {
        Chariot = chariot;
    }

    public bool UserExists(int userID) => Chariot.ExecuteScalar<int>("SELECT count(*) FROM User WHERE ID=?;", userID) > 0;

    public Login? Login(int userID) => Chariot.PullObject<Login>(userID);

    public User? User(int userID) => Chariot.PullObject<User>(userID, EPullType.FullRecursive);

    public Role? Role(string roleName) => Chariot.PullObject<Role>(roleName, EPullType.FullRecursive);

    public int UserCount() => Chariot.ExecuteScalar<int>("SELECT count(*) FROM User;"); /*Chariot.PullObjectListAsync<User>(pullType: EPullType.ObjectOnly).Count;*/

    public async Task<IEnumerable<User>> UsersAsync(Expression<Func<User, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public async Task<IEnumerable<Role>> RolesAsync(Expression<Func<Role, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

}