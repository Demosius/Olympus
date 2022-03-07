using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands;

public class ImportOldDataCommand : ICommand
{
    public AionVM VM { get; set; }

    public ImportOldDataCommand(AionVM vm) { VM = vm; }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        VM.ImportOldData();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}