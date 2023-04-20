using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface IConfirm
{
    public ConfirmCommand ConfirmCommand { get; set; }
    public ConfirmAndCloseCommand ConfirmAndCloseCommand { get; set; }

    public bool Confirm();
}