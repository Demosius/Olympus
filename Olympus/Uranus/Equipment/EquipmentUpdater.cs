using Olympus.Uranus.Equipment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Equipment
{
    public class EquipmentUpdater
    {
        public EquipmentChariot Chariot { get; set; }

        public EquipmentUpdater(ref EquipmentChariot chariot)
        {
            Chariot = chariot;
        }

    }
}
