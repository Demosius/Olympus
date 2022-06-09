using Hydra.ViewModels.Commands;
using Styx.Interfaces;
using Uranus.Interfaces;

namespace Hydra.Interfaces;

public interface IItemDataVM : IDBInteraction, IDataSource, IItemFilters
{
    public ActivateAllItemsCommand ActivateAllItemsCommand { get; set; }
    public DeActivateAllItemsCommand DeActivateAllItemsCommand { get; set; }
    public ExclusiveItemActivationCommand ExclusiveItemActivationCommand { get; set; }

    public void ActivateAllItems();
    public void DeActivateAllItems();
    public void ExclusiveItemActivation();
}