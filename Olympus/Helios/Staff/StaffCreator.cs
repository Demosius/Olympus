using Olympus.Helios.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff
{
    public class StaffCreator
    {
        public StaffChariot Chariot { get; set; }

        public StaffCreator(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

    }
}
