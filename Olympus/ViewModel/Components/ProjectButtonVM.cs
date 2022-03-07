using Uranus.Staff.Model;
using Olympus.ViewModel.Commands;
using System.ComponentModel;

namespace Olympus.ViewModel.Components;

public class ProjectButtonVM : INotifyPropertyChanged
{
    private ProjectGroupVM projectGroup;
    public ProjectGroupVM ProjectGroup 
    {
        get => projectGroup;
        set
        {
            projectGroup = value;
            OnPropertyChanged(nameof(ProjectGroup));
        }
    }

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