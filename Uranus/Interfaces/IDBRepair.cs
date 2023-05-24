using System.Threading.Tasks;
using Uranus.Commands;

namespace Uranus.Interfaces;

public interface IDBRepair
{
    public RepairDataCommand RepairDataCommand { get; set; }

    public Task RepairDataAsync();
}