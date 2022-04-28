using Uranus.Users.Model;

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
}