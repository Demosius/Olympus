using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Staff.Model;

namespace Olympus.ViewModel.Components;

public class ProjectLauncherVM : INotifyPropertyChanged
{
    public OlympusVM OlympusVM { get; set; }

    public List<Department> Departments { get; set; }
    public List<Project> AllProjects { get; set; }
    public List<Project> UserProjects { get; set; }

    #region INotifyPropertyChanged Members

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

    private ProjectGroupVM projects;
    public ProjectGroupVM Projects
    {
        get => projects;
        set
        {
            projects = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public ProjectLauncherVM()
    {
        AllProjects = App.Helios.StaffReader.Projects(pullType: EPullType.FullRecursive).ToList();
        Departments = App.Helios.StaffReader.Departments(pullType: EPullType.IncludeChildren);
        UserProjects = App.Charon.UserEmployee is null ? new List<Project>() : App.Charon.UserEmployee.Projects;
        Projects = new ProjectGroupVM(this, AllProjects, "All");

        // Set Icons for projects.
        foreach (var p in AllProjects)
            p.Icon?.SetDirectory(App.Helios.StaffReader.ProjectIconDirectory);

        ProjectGroups = new ObservableCollection<ProjectGroupVM>();

        ProjectGroupVM projectGroup = new(this, AllProjects, "All");
        ProjectGroups.Add(projectGroup);
        projectGroup = new ProjectGroupVM(this, UserProjects, "User");
        ProjectGroups.Add(projectGroup);

        foreach (var dep in Departments.Where(dep => dep.Projects.Any()))
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

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}