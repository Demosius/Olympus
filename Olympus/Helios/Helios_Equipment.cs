using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Olympus.Helios.Equipment;

namespace Olympus.Helios
{
    public static class GetEquipment
    {
        /* Machines */
        public static DataTable GetMachineTable()
        {
            EquipmentChariot chariot = new EquipmentChariot(Toolbox.GetSol());
            return chariot.GetMachineTable();
        }

        public static List<Machine> GetMachines()
        {
            EquipmentChariot chariot = new EquipmentChariot(Toolbox.GetSol());
            DataTable data = chariot.GetMachinesWithType();
            List<Machine> machines = data.AsEnumerable().Select(row => new Machine()).ToList();
            return machines;
        }
    }

    public static class PutEquipment
    {

    }

    public static class PostEquipment
    {

    }

    public static class DeleteEquipment
    {

    }
}
