using Olympus.Helios.Users.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Users
{
    public class UserDeleter
    {
        public UserChariot Chariot { get; set; }

        public UserDeleter(ref UserChariot chariot)
        {
            Chariot = chariot;
        }
       
    }
}
