using Panacea.ViewModels.Commands;

namespace Panacea.Interfaces;

public interface IItemData
{
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }

    public void ItemsToClipboard();
}