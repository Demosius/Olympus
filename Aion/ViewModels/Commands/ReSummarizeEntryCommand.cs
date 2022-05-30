using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class ReSummarizeEntryCommand : ICommand
{
    public ShiftEntryPageVM VM { get; set; }

    public ReSummarizeEntryCommand(ShiftEntryPageVM vm) { VM = vm; }

    public bool CanExecute(object parameter)
    {
        return VM.SelectedEntry is not null;
    }

    public void Execute(object parameter)
    {
        VM.ReSummarizeEntry();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}