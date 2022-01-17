using Uranus.Users.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
