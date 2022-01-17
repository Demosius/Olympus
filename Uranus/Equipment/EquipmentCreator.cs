using Uranus.Equipment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Equipment
{
    public class EquipmentCreator
    {
        public EquipmentChariot Chariot { get; set; }

        public EquipmentCreator(ref EquipmentChariot chariot)
        {
            Chariot = chariot;
        }

    }
}
