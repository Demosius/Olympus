using Cadmus.Annotations;
using Olympus.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Models;

namespace Olympus.ViewModels.Components;

public class ProjectButtonVM : INotifyPropertyChanged
{
    public ProjectGroupVM ProjectGroupVM { get; set; }

    public Project Project { get; set; }

    public LaunchProjectCommand LaunchProjectCommand { get; set; }

    public ProjectButtonVM(ProjectGroupVM projectGroupVM, Project project)
    {
        LaunchProjectCommand = new LaunchProjectCommand(this);
        ProjectGroupVM = projectGroupVM;
        Project = project;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}