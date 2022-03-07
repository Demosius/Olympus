using Uranus.Users.Model;
using System.Linq;

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
        if (Chariot.Database.Execute("SELECT count(*) FROM Role WHERE Name='Default';") > 0) return;

        Role role = new();

        role.SetDefault();

        _ = Chariot.Create(role);
    }

    public void AssureDBManagerRole()
    {
        if (Chariot.PullObjectList<Role>().Any(role => role.Name == "DatabaseManager")) return;

        Role role = new();
        role.SetMaster();
        _ = Chariot.Create(role);
    }

    public void Role(Role role, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(role, pushType);

    public void User(User user, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(user, pushType);

    public void Login(Login login, EPushType pushType = EPushType.ObjectOnly) => Chariot.Create(login, pushType);
}