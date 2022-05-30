using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class ConfirmEntryCreationCommand : ICommand
{
    public EntryCreationVM VM { get; set; }

    public ConfirmEntryCreationCommand(EntryCreationVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        var w = parameter as Window;
        VM.ConfirmAll();
        w?.Close();
    }
}