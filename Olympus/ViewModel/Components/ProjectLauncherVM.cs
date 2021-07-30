using Olympus.Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympus.Uranus;
using System.Collections.ObjectModel;

namespace Olympus.ViewModel.Components
{
    public class ProjectLauncherVM : INotifyPropertyChanged
    {
        public OlympusVM OlympusVM { get; set; }

        public List<Department> Departments { get; set; }
        public List<Project> AllProjects { get; set; }
        public List<Project> UserProjects { get; set; }
        public ObservableCollection<ProjectGroupVM> ProjectGroups { get; set; }

        public ProjectLauncherVM()
        {
            List<Department> deps = App.Helios.StaffReader.Departments(PullType.IncludeChildren);
            Departments = deps;
            AllProjects = App.Helios.StaffReader.Projects();
            if (App.Charon.UserEmployee is null)
                UserProjects = new List<Project> { };
            else
                UserProjects = App.Charon.UserEmployee.Projects;
            ProjectGroups = new ObservableCollection<ProjectGroupVM>
            {
                new ProjectGroupVM(this, AllProjects, "All"),
                new ProjectGroupVM(this, UserProjects, "User")
            };
            foreach (var dep in Departments)
            {
                ProjectGroups.Add(new ProjectGroupVM(this, dep));
            }
        }

        public ProjectLauncherVM(OlympusVM olympusVM)
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
