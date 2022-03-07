using Uranus.Staff.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Olympus.ViewModel.Components;

public class ProjectGroupVM : INotifyPropertyChanged
{
    public ProjectLauncherVM ProjectLauncher { get; set; }

    private ObservableCollection<ProjectButtonVM> projects;
    public ObservableCollection<ProjectButtonVM> Projects
    {
        get => projects; 
        set
        {
            projects = value;
            OnPropertyChanged(nameof(Projects));
        }
    }
        
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
        foreach (var projectButton in projects.Select(project => new ProjectButtonVM(this, project)))
        {
            Projects.Add(projectButton);
        }

        GroupName = groupName;
    }

    public ProjectGroupVM(ProjectLauncherVM projectLauncher, Department department) : this(projectLauncher)
    {
        foreach (var projectButton in department.Projects.Select(project => new ProjectButtonVM(this, project)))
        {
            Projects.Add(projectButton);
        }

        GroupName = department.Name;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}