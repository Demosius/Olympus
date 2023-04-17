using Pantheon.ViewModels.Commands.TempTags;

namespace Pantheon.ViewModels.Interface;

public interface ITempTags
{
    public bool CanUnassign { get; }
    public bool CanAssign { get; }

    public SelectTempTagCommand SelectTempTagCommand { get; set; }
    public UnassignTempTagCommand UnassignTempTagCommand { get; set; }
    public AssignTempTagCommand AssignTempTagCommand { get; set; }

    public void SelectTempTag();
    public void UnassignTempTag();
    public void AssignTempTag();
}