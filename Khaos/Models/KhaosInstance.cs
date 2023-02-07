using System.Collections.Generic;
using Uranus;
using Uranus.Inventory.Models;

namespace Khaos.Models;

public class KhaosInstance
{
    public List<NAVItem> Items { get; set; }
    public List<NAVTransferOrder> TransferOrders { get; set; }
    public List<NAVBin> Bins { get; set; }
    public List<NAVStock> NAVStocks { get; set; }
    public List<NAVUoM> UoMs { get; set; }

    public bool UseBufferBatches { get; set; }
    public bool SplitMakeBulk { get; set; }
    public bool SplitProductType { get; set; }
    public bool CapCube { get; set; }
    public bool UseCountCap { get; set; }
    public bool UseCountTarget { get; set; }
    public float CubeCap { get; set; }
    public int GroupCap { get; set; }
    public float CubeTarget { get; set; }
    public int GroupTarget { get; set; }

    public static IEnumerable<string> GetInstanceNames(Helios helios)
    {
        return new List<string>();
    }

    public static KhaosInstance GetInstance(string instanceName)
    {
        return new KhaosInstance();
    }
}