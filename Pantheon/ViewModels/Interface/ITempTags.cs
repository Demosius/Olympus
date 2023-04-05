using Pantheon.ViewModels.Commands.Employees;

namespace Pantheon.ViewModels.Interface;

public interface ITempTags
{
    public SelectTempTagCommand SelectTempTagCommand { get; set; }
    public UnassignTempTagCommand UnassignTempTagCommand { get; set; }

    public void SelectTempTag();
    public void UnassignTempTag();
}