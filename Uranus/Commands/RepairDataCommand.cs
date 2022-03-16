#nullable enable
using System;
using System.Windows.Input;
using Uranus.Interfaces;

namespace Uranus.Commands;

public class RepairDataCommand : ICommand
{
    public IDBInteraction VM { get; set; }

    public RepairDataCommand(IDBInteraction vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RepairData();
    }

#pragma warning disable 67
    public event EventHandler? CanExecuteChanged;
#pragma warning restore 67
}