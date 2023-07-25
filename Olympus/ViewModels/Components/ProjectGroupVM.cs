using Cadmus.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Staff.Models;

namespace Olympus.ViewModels.Components;

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
            OnPropertyChanged();
        }
    }

    private string groupName;
    public string GroupName
    {
        get => groupName;
        set
        {
            groupName = value;
            OnPropertyChanged();
        }
    }

    public ProjectGroupVM(ProjectLauncherVM projectLauncher)
    {
        ProjectLauncher = projectLauncher;
        projects = new ObservableCollection<ProjectButtonVM>();
        groupName = string.Empty;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}