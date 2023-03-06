using Pantheon.ViewModels.Commands.Generic;

namespace Pantheon.ViewModels.Interface;

public interface ICreationMode
{
    public ActivateCreationCommand ActivateCreationCommand { get; set; }

    public bool InCreation { get; set; }

    void ActivateCreation();
}