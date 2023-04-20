using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interfaces;

public interface IRoles
{
    public Charon Charon { get; set; }

    public SelectRoleCommand SelectRoleCommand { get; set; }
    public ClearRoleCommand ClearRoleCommand { get; set; }

    public void SelectRole();
    public void ClearRole();
}