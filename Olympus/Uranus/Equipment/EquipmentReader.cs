using Olympus.Uranus.Equipment.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Equipment
{
    public class EquipmentReader
    {
        public EquipmentChariot Chariot { get; set; }

        public EquipmentReader(ref EquipmentChariot chariot)
        {
            Chariot = chariot;
        }

        /* Machines */
        public List<Machine> Machines()
        {
            List<Machine> machines = new List<Machine> { };
            foreach (var machine in Chariot.PullObjectList<Machine>(PullType.FullRecursive))
            {
                switch (machine.Type.LicenceRequired)
                {
                    case Staff.ELicence.LF:
                        machines.Add((Forklift)machine);
                        break;
                    case Staff.ELicence.LO:
                        machines.Add((Stockpicker)machine);
                        break;
                    case Staff.ELicence.WP:
                        machines.Add((WorkingPlatform)machine);
                        break;
                    case null:
                        machines.Add((Rabbit)machine);
                        break;
                    default:
                        break;
                }
            }
            return machines;
        }

    }
}
