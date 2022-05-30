using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class DeleteEntryCommand : ICommand
{
    public EntryCreationVM VM { get; set; }

    public DeleteEntryCommand(EntryCreationVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return VM.SelectedEntry is not null;
    }

    public void Execute(object parameter)
    {
        VM.DeleteSelectedEntry();
    }
}