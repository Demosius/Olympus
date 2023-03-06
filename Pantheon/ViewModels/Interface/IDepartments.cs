using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interface;

public interface IDepartments
{
    public Charon Charon { get; set; }

    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public ClearDepartmentCommand ClearDepartmentCommand { get; set; }

    public void SelectDepartment();
    public void ClearDepartment();
}