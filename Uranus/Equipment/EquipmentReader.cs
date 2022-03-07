using System;
using Uranus.Equipment.Model;
using System.Collections.Generic;

namespace Uranus.Equipment;

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
        List<Machine> machines = new();
        foreach (var machine in Chariot.PullObjectList<Machine>(pullType: EPullType.FullRecursive))
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
                    throw new ArgumentOutOfRangeException();
            }
        }
        return machines;
    }

}