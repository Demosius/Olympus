using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class ImportOldDataCommand : ICommand
{
    public AionVM VM { get; set; }

    public ImportOldDataCommand(AionVM vm) { VM = vm; }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public async void Execute(object parameter)
    {
        await VM.ImportOldData();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}