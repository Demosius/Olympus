using Hydra.ViewModels.Commands;
using Uranus.Interfaces;

namespace Hydra.Interfaces;

public interface IItemFilters : IFilters
{
    public FilterItemsFromClipboardCommand FilterItemsFromClipboardCommand { get; set; }

    public void FilterItemsFromClipboard();
}