using Panacea.ViewModels.Commands;

namespace Panacea.Interfaces;

public interface IBinData
{
    public BinsToClipboardCommand BinsToClipboardCommand { get; set; }

    public void BinsToClipboard();
}