using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class SetDateRangeCommand : ICommand
{
    public DateRangeVM VM { get; set; }

    public SetDateRangeCommand(DateRangeVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return (VM.MinDate.Date != VM.InitialMin.Date || VM.MaxDate.Date != VM.InitialMax.Date);
    }

    public void Execute(object parameter)
    {
        var w = (Window) parameter;
        VM.SetDateRange();
        w.Close();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}