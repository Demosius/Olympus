using Cadmus.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Staff.Models;

namespace Olympus.ViewModels.Components;

public class ProjectLauncherVM : INotifyPropertyChanged
{
    public OlympusVM OlympusVM { get; set; }

    public List<Department> Departments { get; set; }
    public List<Project> AllProjects { get; set; }
    public List<Project> UserProjects { get; set; }

    #region INotifyPropertyChanged Members
    
    public ObservableCollection<ProjectGroupVM> ProjectGroups { get; set; }

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

    public ProjectLauncherVM(OlympusVM olympusVM)
    {
        OlympusVM = olympusVM;

        Departments = new List<Department>();
        AllProjects = new List<Project>();
        UserProjects = new List<Project>();
        ProjectGroups = new ObservableCollection<ProjectGroupVM>();
        projects = new ProjectGroupVM(this, AllProjects, string.Empty);

        Task.Run(SetDataAsync);
    }

    private async Task SetDataAsync()
    {
        var projectTask = App.Helios.StaffReader.ProjectsAsync(pullType: EPullType.FullRecursive);
        var deptTask = App.Helios.StaffReader.DepartmentsAsync(pullType: EPullType.IncludeChildren);

        await Task.WhenAll(projectTask, deptTask).ConfigureAwait(false);

        Departments = await deptTask.ConfigureAwait(false);
        AllProjects = (await projectTask.ConfigureAwait(false)).ToList();

        UserProjects = App.Charon.Employee is null ? new List<Project>() : App.Charon.Employee.Projects;
        Projects = new ProjectGroupVM(this, AllProjects, "All");

        // Set Icons for projects.
        foreach (var p in AllProjects)
            p.Icon?.SetDirectory(App.Helios.StaffReader.ProjectIconDirectory);
        
        ProjectGroups.Clear();

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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}