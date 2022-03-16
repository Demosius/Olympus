using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

internal class ShowEmployeePageCommand : ICommand
{
    public PantheonVM VM { get; set; }

    public ShowEmployeePageCommand(PantheonVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        VM.ShowEmployeePage();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}