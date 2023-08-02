using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Inventory;
using Uranus.Inventory.Models;

namespace Morpheus.ViewModels.Controls.Inventory;

public class StoreVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Store Store { get; set; }

    #region Store Access

    public string Number => Store.Number;

    public int WaveNumber
    {
        get => Store.WaveNumber;
        set
        {
            Store.WaveNumber = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public EVolume Volume
    {
        get => Store.Volume;
        set
        {
            Store.Volume = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Restock
    {
        get => Store.Restock;
        set
        {
            Store.Restock = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string CasePick
    {
        get => Store.CasePick;
        set
        {
            Store.CasePick = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string NR
    {
        get => Store.NR;
        set
        {
            Store.NR = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Overnight
    {
        get => Store.Overnight;
        set
        {
            Store.Overnight = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Road1
    {
        get => Store.Road1;
        set
        {
            Store.Road1 = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Road2
    {
        get => Store.Road2;
        set
        {
            Store.Road2 = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string ExtRoad
    {
        get => Store.ExtRoad;
        set
        {
            Store.ExtRoad = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Special
    {
        get => Store.Special;
        set
        {
            Store.Special = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string RoadCCN
    {
        get => Store.RoadCCN;
        set
        {
            Store.RoadCCN = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int TransitDays
    {
        get => Store.TransitDays;
        set
        {
            Store.TransitDays = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string MBRegion
    {
        get => Store.MBRegion;
        set
        {
            Store.MBRegion = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string RoadRegion
    {
        get => Store.RoadRegion;
        set
        {
            Store.RoadRegion = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int SortingLane
    {
        get => Store.SortingLane;
        set
        {
            Store.SortingLane = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string State
    {
        get => Store.State;
        set
        {
            Store.State = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Region
    {
        get => Store.Region;
        set
        {
            Store.Region = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public EStoreType Type
    {
        get => Store.Type;
        set
        {
            Store.Type = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string Wave
    {
        get => Store.Wave;
        set
        {
            // Get any potential number from the value.
            var sb = new StringBuilder();
            var matches = Regex.Matches(value, @"\d+");
            foreach (Match match in matches) sb.Append(match.Value);

            if (int.TryParse(sb.ToString(), out var num))
            {
                Store.WaveNumber = num % 100;
                _ = Save();
            }
            OnPropertyChanged();
        }
    }


    #endregion

    #region INotifyPropertyChanged Members



    #endregion

    public async Task<int> Save() => await Helios.InventoryUpdater.StoreAsync(Store);

    public StoreVM(Store store, Helios helios)
    {
        Helios = helios;
        Store = store;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}