using System;
using System.Windows.Input;
using Uranus.Interfaces;

namespace Uranus.Commands;

public class ApplySortingCommand : ICommand
{
    public ISorting VM { get; set; }

    public ApplySortingCommand(ISorting vm) { VM = vm; }

#pragma warning disable 67
    public event EventHandler? CanExecuteChanged;
#pragma warning restore 67

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ApplySorting();
    }
}