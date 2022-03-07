using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands;

public class CreateEntryCommand : ICommand
{
    public EntryCreationVM VM { get; set; }

    public CreateEntryCommand(EntryCreationVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return VM.SelectedEmployee?.ID != -1;
    }

    public void Execute(object parameter)
    {
        VM.CreateEntry();
    }
}