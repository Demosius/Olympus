using Olympus.Uranus.Staff.Model;
using Olympus.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.ViewModel.Components
{
    public class ProjectButtonVM : INotifyPropertyChanged
    {
        public ProjectGroupVM ProjectGroup { get; set; }

        private Project project;
        public Project Project 
        {
            get => project; 
            set
            {
                project = value;
                OnPropertyChanged(nameof(Project));
            } 
        }

        public LaunchProjectCommand LaunchProjectCommand { get; set; }

        public ProjectButtonVM() 
        {
            LaunchProjectCommand = new LaunchProjectCommand(this);
        }

        public ProjectButtonVM(ProjectGroupVM projectGroup) : this()
        {
            ProjectGroup = projectGroup;
        }

        public ProjectButtonVM(ProjectGroupVM projectGroup, Project project) : this(projectGroup)
        {
            Project = project;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
