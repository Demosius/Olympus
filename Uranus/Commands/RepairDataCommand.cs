#nullable enable
using System;
using System.Windows.Input;
using Uranus.Interfaces;

namespace Uranus.Commands;

public class RepairDataCommand : ICommand
{
    public IDBRepair VM { get; set; }

    public RepairDataCommand(IDBRepair vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RepairDataAsync();
    }

#pragma warning disable 67
    public event EventHandler? CanExecuteChanged;
#pragma warning restore 67
}