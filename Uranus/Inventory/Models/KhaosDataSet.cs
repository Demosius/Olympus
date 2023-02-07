using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Inventory.Models;

public class KhaosDataSet
{
    public List<NAVTransferOrder> TransferOrders { get; set; }
    public Dictionary<int, NAVItem> Items { get; set; }
    public Dictionary<string, Store> Stores { get; set; }

}