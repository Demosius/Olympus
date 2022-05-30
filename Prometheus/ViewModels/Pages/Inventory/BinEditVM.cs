using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Prometheus.ViewModels.Pages.Inventory;

internal class BinEditVM : INotifyPropertyChanged
{

    public List<string> ExistingBinIDs { get; set; }

    private BinVM? parentVM;
    public BinVM? ParentVM
    {
        get => parentVM;
        set
        {
            parentVM = value;
            if (parentVM is not null) ExistingBinIDs = parentVM.Bins.Select(b => b.ID).ToList();
            OnPropertyChanged(nameof(ParentVM));
        }
    }

    private NAVBin? bin;
    public NAVBin? Bin
    {
        get => bin;
        set
        {
            bin = value;
            OnPropertyChanged(nameof(Bin));
        }
    }


    public BinEditVM()
    {
        ExistingBinIDs = new List<string>();
    }

    /// <summary>
    /// Saves the bin changes into the parent bin list, and loads it into the database.
    /// </summary>
    public static void SaveChanges()
    {

    }


    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}