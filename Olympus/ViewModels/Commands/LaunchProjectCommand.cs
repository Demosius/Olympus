using Olympus.ViewModels.Components;
using Olympus.ViewModels.Utility;
using System;
using System.Windows.Input;
using Uranus.Staff;

namespace Olympus.ViewModels.Commands;

public class LaunchProjectCommand : ICommand
{
    public ProjectButtonVM VM { get; set; }

    public LaunchProjectCommand(ProjectButtonVM vm)
    {
        VM = vm;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        if (!Enum.TryParse((string)(parameter ?? "None"), out EProject eProject)) eProject = EProject.None;
        return VM.ProjectGroupVM.ProjectLauncher.OlympusVM.CurrentProject?.Project != eProject && ProjectFactory.CanLaunch(eProject, App.Charon);
    }

    public void Execute(object? parameter)
    {
        if (!Enum.TryParse((string)(parameter ?? "None"), out EProject eProject)) eProject = EProject.None;
        VM.ProjectGroupVM.ProjectLauncher.OlympusVM.LoadProject(eProject);
    }
}