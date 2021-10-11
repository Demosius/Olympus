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
    public class NewProjectLauncherVM : INotifyPropertyChanged
    {
        public OlympusVM OlympusVM { get; set; }

        public List<Department> Departments { get; set; }
        public List<Project> AllProjects { get; set; }
        public List<Project> UserProjects { get; set; }

        public ObservableCollection<TabItem> Tabs { get; set; }

        public NewProjectLauncherVM()
        {
            List<Department> deps = App.Helios.StaffReader.Departments(pullType: PullType.IncludeChildren);
            Departments = deps;
            AllProjects = App.Helios.StaffReader.Projects(pullType: PullType.FullRecursive);
            if (App.Charon.UserEmployee is null)
                UserProjects = new List<Project>();
            else
                UserProjects = App.Charon.UserEmployee.Projects;

            Tabs = new ObservableCollection<TabItem>();

            TabItem tab;

            tab = new TabItem { Header = "All", Projects = AllProjects };
            Tabs.Add(tab);
            tab = new TabItem { Header = "User", Projects = UserProjects };
            Tabs.Add(tab);

            foreach (var dep in Departments)
            {
                tab = new TabItem { Header = dep.Name, Projects = dep.Projects };
                Tabs.Add(tab);
            }
        }

        public NewProjectLauncherVM(OlympusVM olympusVM) : this()
        {
            OlympusVM = olympusVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TabItem
    {
        public string Header { get; set; }
        public List<Project> Projects { get; set; }

    }
}
