using Olympus.Helios.Equipment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Equipment
{
    public class EquipmentDeleter
    {
        public EquipmentChariot Chariot { get; set; }

        public EquipmentDeleter(ref EquipmentChariot chariot)
        {
            Chariot = chariot;
        }
       
    }
}
