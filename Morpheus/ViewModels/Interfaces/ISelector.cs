using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface ISelector
{
    public bool CanCreate { get; }
    public bool CanDelete { get; }
    public bool CanConfirm { get; }

    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }

    void Create();
    void Delete();
}