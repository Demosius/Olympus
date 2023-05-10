using System.Threading.Tasks;
using Pantheon.ViewModels.Commands.TempTags;

namespace Pantheon.ViewModels.Interfaces;

public interface ITempTags
{
    public bool CanUnassign { get; }
    public bool CanAssign { get; }

    public SelectTempTagCommand SelectTempTagCommand { get; set; }
    public UnassignTempTagCommand UnassignTempTagCommand { get; set; }
    public AssignTempTagCommand AssignTempTagCommand { get; set; }

    public Task SelectTempTagAsync();
    public void UnassignTempTag();
    public Task AssignTempTagAsync();
}