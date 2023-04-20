using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interfaces;

public interface IDepartments
{
    public Charon Charon { get; set; }

    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public ClearDepartmentCommand ClearDepartmentCommand { get; set; }

    public void SelectDepartment();
    public void ClearDepartment();
}