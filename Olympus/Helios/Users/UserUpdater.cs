using Olympus.Helios.Users.Model;
using Olympus.Styx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Users
{
    public class UserUpdater
    {
        public UserChariot Chariot { get; set; }

        public UserUpdater(ref UserChariot chariot)
        {
            Chariot = chariot;
        }

        public void Login(Login login)
        {
            if (App.Charon.CanUpdateUser(login.UserID))
            {
                Chariot.Update(login);
            }
        }

    }
}
