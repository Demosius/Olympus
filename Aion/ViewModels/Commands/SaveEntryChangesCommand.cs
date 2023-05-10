using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class SaveEntryChangesCommand : ICommand
{
    public ShiftEntryPageVM VM { get; set; }

    public SaveEntryChangesCommand(ShiftEntryPageVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public async void Execute(object parameter)
    {
        await VM.SaveEntryChanges();
    }
}