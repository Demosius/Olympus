using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.TempTags;

namespace Pantheon.ViewModels.Commands.TempTags;

public class DeleteTagUseCommand : ICommand
{
    public TempTagManagementVM VM { get; set; }

    public DeleteTagUseCommand(TempTagManagementVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanDeleteUse;
    }

    public async void Execute(object? parameter)
    {
        await VM.DeleteTagUse();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}