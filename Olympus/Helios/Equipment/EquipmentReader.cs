using Olympus.Helios.Equipment.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Equipment
{
    public class EquipmentReader
    {
        public EquipmentChariot Chariot { get; set; }

        public EquipmentReader(ref EquipmentChariot chariot)
        {
            Chariot = chariot;
        }

    }
}
