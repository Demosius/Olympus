using Pantheon.ViewModels.Commands.Generic;

namespace Pantheon.ViewModels.Interface;

public interface ICreationMode
{
    public ActivateCreationCommand ActivateCreationCommand { get; set; }

    void ActivateCreation();
}