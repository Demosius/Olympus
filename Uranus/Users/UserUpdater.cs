using Uranus.Users.Model;

namespace Uranus.Users
{
    public class UserUpdater
    {
        public UserChariot Chariot { get; set; }

        public UserUpdater(ref UserChariot chariot)
        {
            Chariot = chariot;
        }

        public bool Login(Login login)
        {
            return Chariot.Update(login);
        }

    }
}
