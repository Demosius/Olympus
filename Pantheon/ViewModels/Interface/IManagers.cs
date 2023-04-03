using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interface;

public interface IManagers
{
    public Charon Charon { get; set; }

    public SelectManagerCommand SelectManagerCommand { get; set; }
    public ClearManagerCommand ClearManagerCommand { get; set; }

    public void SelectManager();
    public void ClearManager();
}