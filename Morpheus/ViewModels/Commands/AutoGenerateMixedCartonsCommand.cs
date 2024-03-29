﻿using System;
using System.Windows.Input;
using Morpheus.ViewModels.Controls.Inventory;

namespace Morpheus.ViewModels.Commands;


public class AutoGenerateMixedCartonsCommand : ICommand
{
    public MixedCartonHandlerVM VM { get; set; }

    public AutoGenerateMixedCartonsCommand(MixedCartonHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.AutoGenerateMixedCartons();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}