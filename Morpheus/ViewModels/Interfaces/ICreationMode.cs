using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface ICreationMode
{
    public ActivateCreationCommand ActivateCreationCommand { get; set; }

    public bool InCreation { get; set; }

    void ActivateCreation();
}