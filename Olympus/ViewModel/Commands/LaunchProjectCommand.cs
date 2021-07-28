using Olympus.Helios.Staff;
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
        public ProjectGroupVM VM { get; set; }

        public LaunchProjectCommand(ProjectGroupVM vm)
        {
            VM = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            EProject eProject = EnumConverter.StringToProject(parameter as string);
            return VM.ProjectLauncher.OlympusVM.CurrentProject != eProject;
        }

        public void Execute(object parameter)
        {
            EProject eProject = EnumConverter.StringToProject(parameter as string);
            VM.ProjectLauncher.OlympusVM.LoadProject(eProject);
        }
    }
}
