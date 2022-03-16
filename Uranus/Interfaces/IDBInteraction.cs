using Uranus.Commands;

namespace Uranus.Interfaces;

public interface IDBInteraction
{
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    public void RefreshData();
    public void RepairData();
}