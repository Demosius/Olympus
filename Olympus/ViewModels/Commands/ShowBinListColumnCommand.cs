﻿using Olympus.ViewModels.Components;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class ShowBinListColumnCommand : ICommand
{
    public InventoryUpdaterVM VM { get; set; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public ShowBinListColumnCommand(InventoryUpdaterVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        InventoryUpdaterVM.ShowBLInfo();
    }
}