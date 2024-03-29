﻿using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interfaces;

namespace Pantheon.ViewModels.Commands.Employees;

public class FindNewImageCommand : ICommand
{
    public IImageSelector VM { get; set; }

    public FindNewImageCommand(IImageSelector vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.FindNewImageAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}