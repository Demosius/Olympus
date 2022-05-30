using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Uranus.Equipment.Models;

namespace Uranus.Equipment;

public class EquipmentReader
{
    public EquipmentChariot Chariot { get; set; }

    public EquipmentReader(ref EquipmentChariot chariot)
    {
        Chariot = chariot;
    }

    /* Machines */
    public List<Machine> Machines(Expression<Func<Machine, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public Dictionary<string, List<Machine>> MachineDictionary(Expression<Func<Machine, bool>>? filter = null, EPullType pullType = EPullType.IncludeChildren)
    {
        return Machines(filter, pullType)
            .GroupBy(m => m.TypeCode)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public List<Machine> Machines(string machineTypeCode, EPullType pullType = EPullType.ObjectOnly)
    {
        return Machines(m => m.TypeCode == machineTypeCode, pullType);
    }

}