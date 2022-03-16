using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

internal class ShowRosterPageCommand : ICommand
{
    public PantheonVM VM { get; set; }

    public ShowRosterPageCommand(PantheonVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        VM.ShowRosterPage();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}