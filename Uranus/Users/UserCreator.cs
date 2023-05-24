using System.Threading.Tasks;
using Uranus.Users.Models;

namespace Uranus.Users;

public class UserCreator
{
    public UserChariot Chariot { get; set; }

    public UserCreator(ref UserChariot chariot)
    {
        Chariot = chariot;
    }

    public void AssureDefaultRole()
    {
        if (Chariot.ExecuteScalar<int>("SELECT count(*) FROM Role WHERE Name='Default';") > 0) return;

        Role role = new();

        role.SetDefault();

        _ = Chariot.Create(role);
    }

    public async Task AssureDBManagerRoleAsync()
    {
        void Action()
        {
            if (Chariot.ExecuteScalar<int>("SELECT count(*) FROM Role WHERE Name='DatabaseManager';") > 0)
                return;

            Role role = new();
            role.SetMaster();
            Chariot.Create(role);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
    }

    public void Role(Role role, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(role, pushType);

    public void User(User user, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(user, pushType);

    public void Login(Login login, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(login, pushType);
}