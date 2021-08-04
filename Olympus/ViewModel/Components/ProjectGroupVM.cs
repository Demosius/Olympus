using Olympus.Uranus.Staff.Model;
using Olympus.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.ViewModel.Components
{
    public class ProjectGroupVM : INotifyPropertyChanged
    {
        public ProjectLauncherVM ProjectLauncher { get; set; }

        public ObservableCollection<ProjectButtonVM> Projects { get; set; }
        private string groupName;
        public string GroupName
        {
            get => groupName;
            set
            {
                groupName = value;
                OnPropertyChanged(nameof(GroupName));
            }
        }

        public ProjectGroupVM()
        {
            Projects = new ObservableCollection<ProjectButtonVM>();
        }

        public ProjectGroupVM(ProjectLauncherVM projectLauncher) : this()
        {
            ProjectLauncher = projectLauncher;
        }

        public ProjectGroupVM(ProjectLauncherVM projectLauncher, List<Project> projects, string groupName) : this(projectLauncher)
        {
            foreach (var project in projects)
            {
                Projects.Add(new ProjectButtonVM(this, project));
            }
            GroupName = groupName;
        }

        public ProjectGroupVM(ProjectLauncherVM projectLauncher, Department department) : this(projectLauncher)
        {
            foreach (var project in department.Projects)
            {
                Projects.Add(new ProjectButtonVM(this, project));
            }
            GroupName = department.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
