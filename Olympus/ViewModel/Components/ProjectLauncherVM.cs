using Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranus;
using System.Collections.ObjectModel;
using System.Windows;

namespace Olympus.ViewModel.Components
{
    public class ProjectLauncherVM : INotifyPropertyChanged
    {
        public OlympusVM OlympusVM { get; set; }

        public List<Department> Departments { get; set; }
        public List<Project> AllProjects { get; set; }
        public List<Project> UserProjects { get; set; }

        private ObservableCollection<ProjectGroupVM> projectGroups;
        public ObservableCollection<ProjectGroupVM> ProjectGroups
        {
            get => projectGroups;
            set
            {
                projectGroups = value;
                OnPropertyChanged(nameof(ProjectGroups));
            }
        }

        public ProjectLauncherVM()
        {
            /*if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                //Departments = new();
                //AllProjects = new();
                //UserProjects = new();
            }
            else
            {

            }*/

            AllProjects = App.Helios.StaffReader.Projects(pullType: PullType.FullRecursive);
            Departments = App.Helios.StaffReader.Departments(pullType: PullType.IncludeChildren);
            if (App.Charon.UserEmployee is null)
                UserProjects = new();
            else
                UserProjects = App.Charon.UserEmployee.Projects;

            ProjectGroupVM projectGroup;

            ProjectGroups = new();

            projectGroup = new ProjectGroupVM(this, AllProjects, "All");
            ProjectGroups.Add(projectGroup);
            projectGroup = new ProjectGroupVM(this, UserProjects, "User");
            ProjectGroups.Add(projectGroup);

            foreach (var dep in Departments)
            {
                projectGroup = new ProjectGroupVM(this, dep);
                ProjectGroups.Add(projectGroup);
            }
        }

        public ProjectLauncherVM(OlympusVM olympusVM) : this()
        {
            OlympusVM = olympusVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
