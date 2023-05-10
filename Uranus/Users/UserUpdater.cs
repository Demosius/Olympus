using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<int> RolesAsync(List<Role> roles)
    {
        // Make sure that any master or default is set to the correct values.
        foreach (var role in roles) role.CheckStandards();

        var lines = 0;

        async void Action() => lines = await Chariot.UpdateTableAsync(roles);

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return lines;
    }
}