using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class ShowEntriesCommand : ICommand
{
    public AionVM VM { get; set; }

    public ShowEntriesCommand(AionVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return VM.CurrentPage != VM.ShiftEntryPage || VM.CurrentPage is null;
    }

    public void Execute(object parameter)
    {
        VM.ShowEntryPage();
    }
}