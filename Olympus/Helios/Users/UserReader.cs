using Olympus.Helios.Users.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Users
{
    public class UserReader
    {
        public UserChariot Chariot { get; set; }

        public UserReader(ref UserChariot chariot)
        {
            Chariot = chariot;
        }

    }
}
