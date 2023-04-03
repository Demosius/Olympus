using Pantheon.ViewModels.Commands.Generic;

namespace Pantheon.ViewModels.Interface;

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