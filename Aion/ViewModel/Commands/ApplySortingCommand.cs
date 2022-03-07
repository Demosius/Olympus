#nullable enable
using System;
using System.Windows.Input;
using Aion.ViewModel.Interfaces;

namespace Aion.ViewModel.Commands;

public class ApplySortingCommand : ICommand
{
    public IFilters VM { get; set; }

    public ApplySortingCommand(IFilters vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ApplySorting();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}