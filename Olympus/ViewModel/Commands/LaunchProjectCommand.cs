using Olympus.Uranus.Staff;
using Olympus.ViewModel.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            EProject eProject = EnumConverter.StringToProject(parameter as string);
            if (VM.ProjectGroup is null) return false;
            return VM.ProjectGroup.ProjectLauncher.OlympusVM.CurrentProject != eProject;
        }

        public void Execute(object parameter)
        {
            EProject eProject = EnumConverter.StringToProject(parameter as string);
            VM.ProjectGroup.ProjectLauncher.OlympusVM.LoadProject(eProject);
        }
    }
}
