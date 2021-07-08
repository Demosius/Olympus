using Olympus.Helios.Users.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Users
{
    public class UserCreator
    {
        public UserChariot Chariot { get; set; }

        public UserCreator(ref UserChariot chariot)
        {
            Chariot = chariot;
        }

    }
}
