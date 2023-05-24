using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
    public async Task<List<Machine>> MachinesAsync(Expression<Func<Machine, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) =>
        await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public async Task<Dictionary<string, List<Machine>>> MachineDictionary(Expression<Func<Machine, bool>>? filter = null, EPullType pullType = EPullType.IncludeChildren)
    {
        return (await MachinesAsync(filter, pullType).ConfigureAwait(false))
            .GroupBy(m => m.TypeCode)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<List<Machine>> MachinesAsync(string machineTypeCode, EPullType pullType = EPullType.ObjectOnly)
    => await MachinesAsync(m => m.TypeCode == machineTypeCode, pullType).ConfigureAwait(false);

}