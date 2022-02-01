using Uranus.Staff;
using Olympus.ViewModel.Components;
using System;
using System.Windows.Input;
using Olympus.ViewModel.Utility;

namespace Olympus.ViewModel.Commands
{
    public class LaunchProjectCommand : ICommand
    {
        public ProjectButtonVM VM { get; set; }

        public LaunchProjectCommand(ProjectButtonVM vm)
        {
            VM = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            var eProject = (EProject) Enum.Parse(typeof(EProject), (string) parameter ?? "None");
            if (VM.ProjectGroup is null) return false;
            return VM.ProjectGroup.ProjectLauncher.OlympusVM.CurrentProject?.Project != eProject && ProjectFactory.CanLaunch(eProject, App.Charon);
        }

        public void Execute(object parameter)
        {
            var eProject = EnumConverter.StringToProject(parameter as string);
            VM.ProjectGroup.ProjectLauncher.OlympusVM.LoadProject(eProject);
        }
    }
}
