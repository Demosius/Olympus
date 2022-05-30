using System.Collections.Generic;
using System.Linq;
using Uranus.Users.Models;

namespace Uranus.Users;

public class UserUpdater
{
    public UserChariot Chariot { get; set; }

    public UserUpdater(ref UserChariot chariot)
    {
        Chariot = chariot;
    }

    public int Login(Login login) => Chariot.Update(login);

    public int User(User user) => Chariot.Update(user);

    public int Roles(IEnumerable<Role> roles)
    {
        // Avoid multiple enum
        var enumerable = roles as Role[] ?? roles.ToArray();
        // Make sure that any master or default is set to the correct values.
        foreach (var role in enumerable) role.CheckStandards();

        var lines = 0;

        Chariot.Database?.RunInTransaction(() => lines = enumerable.Sum(role => Chariot.InsertOrUpdate(role)));

        return lines;
    }
}