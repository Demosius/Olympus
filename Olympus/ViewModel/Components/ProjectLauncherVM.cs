using Uranus.Staff.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Uranus;
using System.Collections.ObjectModel;

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
            AllProjects = App.Helios.StaffReader.Projects(pullType: EPullType.FullRecursive);
            Departments = App.Helios.StaffReader.Departments(pullType: EPullType.IncludeChildren);
            UserProjects = App.Charon.UserEmployee is null ? new() : App.Charon.UserEmployee.Projects;

            // Set Icons for projects.
            foreach (var p in AllProjects)
                p.Icon.SetImageFilePath(App.Helios.StaffReader);

            ProjectGroups = new();

            ProjectGroupVM projectGroup = new(this, AllProjects, "All");
            ProjectGroups.Add(projectGroup);
            projectGroup = new(this, UserProjects, "User");
            ProjectGroups.Add(projectGroup);

            foreach (var dep in Departments.Where(dep => dep.Projects.Any()))
            {
                projectGroup = new(this, dep);
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
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
